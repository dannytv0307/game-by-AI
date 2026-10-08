"""Hậu kỳ pixel art: tách nền, thu nhỏ về lưới pixel thật, khóa bảng màu, cắt frame, ghép sprite sheet."""
import io
import math

import numpy as np
from PIL import Image


# ---------------- màu ----------------
def hex_to_rgb(h):
    h = h.lstrip("#")
    return np.array([int(h[i:i + 2], 16) for i in (0, 2, 4)], dtype=np.float64)


def _srgb_to_oklab(rgb):
    c = rgb / 255.0
    c = np.where(c <= 0.04045, c / 12.92, ((c + 0.055) / 1.055) ** 2.4)
    l = 0.4122214708 * c[..., 0] + 0.5363325363 * c[..., 1] + 0.0514459929 * c[..., 2]
    m = 0.2119034982 * c[..., 0] + 0.6806995451 * c[..., 1] + 0.1073969566 * c[..., 2]
    s = 0.0883024619 * c[..., 0] + 0.2817188376 * c[..., 1] + 0.6299787005 * c[..., 2]
    l, m, s = np.cbrt(l), np.cbrt(m), np.cbrt(s)
    return np.stack([
        0.2104542553 * l + 0.7936177850 * m - 0.0040720468 * s,
        1.9779984951 * l - 2.4285922050 * m + 0.4505937099 * s,
        0.0259040371 * l + 0.7827717662 * m - 0.8086757660 * s,
    ], axis=-1)


class Palette:
    def __init__(self, hexes):
        self.rgb = np.stack([hex_to_rgb(h) for h in hexes])
        self.lab = _srgb_to_oklab(self.rgb)

    def index(self, rgb):
        """rgb: (H,W,3) -> (H,W) chỉ số màu palette gần nhất (OKLab, không dither). Xử lý theo khối để tiết kiệm RAM."""
        rgb = np.asarray(rgb, dtype=np.float64)
        out = np.empty(rgb.shape[:2], dtype=np.int32)
        for y in range(0, rgb.shape[0], 64):
            lab = _srgb_to_oklab(rgb[y:y + 64])
            d = ((lab[..., None, :] - self.lab[None, None, :, :]) ** 2).sum(-1)
            out[y:y + 64] = d.argmin(-1)
        return out

    def quantize(self, rgb):
        return self.rgb[self.index(rgb)].astype(np.uint8)


# ---------------- tách nền ----------------
def load(data_or_path):
    if isinstance(data_or_path, (bytes, bytearray)):
        return Image.open(io.BytesIO(data_or_path)).convert("RGB")
    return Image.open(data_or_path).convert("RGB")


def key_mask(img, key_hex, tol=90, strict=False, sample="corners"):
    """Mask foreground (True) bằng khoảng cách tới màu nền.
    sample: "corners" = trung vị 4 góc (sprite); "top" = 2 góc trên (lớp nền parallax);
            "edges" = màu phổ biến nhất trên viền ảnh, chấp nhận cả nền không phải magenta (tileset)."""
    a = np.asarray(img, dtype=np.float64)
    target = hex_to_rgb(key_hex)
    if sample == "edges":
        border = np.concatenate([a[0], a[-1], a[:, 0], a[:, -1]])
        q = (border // 16).astype(int)
        vals, cnt = np.unique(q, axis=0, return_counts=True)
        top = vals[cnt.argmax()]
        if cnt.max() < 0.3 * len(border):
            return np.ones(a.shape[:2], dtype=bool)
        bg = np.median(border[(q == top).all(1)], axis=0)
        tol = min(tol, 40)  # nền tối gần màu tile → ngưỡng chặt
        keyish = True
    else:
        pts = [a[:8, :8], a[:8, -8:]] + ([a[-8:, :8], a[-8:, -8:]] if sample == "corners" else [])
        bg = np.median(np.concatenate([p.reshape(-1, 3) for p in pts]), axis=0)
        # mô hình hay dùng hồng nhạt (255,155,250) thay cho magenta chuẩn — vẫn coi là đúng nền
        keyish = np.linalg.norm(bg - target) < 120 or (bg[0] > 200 and bg[2] > 200 and bg[1] < 200)
    if not keyish:
        if strict:  # tileset/nền: góc ảnh là nội dung thật → không tách nền
            print(f"  ! không thấy nền {key_hex} → giữ nguyên toàn bộ ảnh")
            return np.ones(a.shape[:2], dtype=bool)
        print(f"  ! nền thực tế {bg.astype(int)} khác {key_hex}, dùng màu góc")
    dist = np.linalg.norm(a - bg, axis=-1)
    fg = dist > tol
    # Viền pha nhiễm màu nền (hồng/magenta): chỉ xét trong dải 3px sát nền để không ăn vào màu tím/đỏ của quỷ
    near_bg = ~_erode(fg, 3)
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    spill = near_bg & (r - g > 50) & (b - g > 50) & (np.abs(r - b) < 90) & (dist < tol * 2.2)
    return fg & ~spill


def _erode(mask, r=1):
    m = mask.copy()
    for _ in range(r):
        p = np.pad(m, 1, constant_values=False)
        m = p[1:-1, 1:-1] & p[:-2, 1:-1] & p[2:, 1:-1] & p[1:-1, :-2] & p[1:-1, 2:]
    return m


def _bbox(mask):
    ys, xs = np.nonzero(mask)
    if len(ys) == 0:
        return None
    return xs.min(), ys.min(), xs.max() + 1, ys.max() + 1


def _remove_specks(alpha, min_px=3):
    """Xóa cụm pixel lẻ loi (< min_px) sau khi thu nhỏ."""
    h, w = alpha.shape
    seen = np.zeros_like(alpha, dtype=bool)
    out = alpha.copy()
    for y in range(h):
        for x in range(w):
            if alpha[y, x] and not seen[y, x]:
                stack, comp = [(y, x)], []
                seen[y, x] = True
                while stack:
                    cy, cx = stack.pop()
                    comp.append((cy, cx))
                    for ny, nx in ((cy + 1, cx), (cy - 1, cx), (cy, cx + 1), (cy, cx - 1)):
                        if 0 <= ny < h and 0 <= nx < w and alpha[ny, nx] and not seen[ny, nx]:
                            seen[ny, nx] = True
                            stack.append((ny, nx))
                if len(comp) < min_px:
                    for cy, cx in comp:
                        out[cy, cx] = False
    return out


def downscale_rgba(rgb_img, mask, scale, palette, size=None):
    """Thu nhỏ theo "mode": khóa palette ở độ phân giải gốc, mỗi pixel đích lấy màu xuất hiện nhiều nhất
    trong khối tương ứng. Không trộn màu => giữ nét cứng; màu outline được ưu tiên để viền không bị mất."""
    h, w = mask.shape
    nw, nh = size or (max(1, round(w * scale)), max(1, round(h * scale)))
    idx = palette.index(np.asarray(rgb_img))
    k = len(palette.rgb)
    ys = np.linspace(0, h, nh + 1).astype(int)
    xs = np.linspace(0, w, nw + 1).astype(int)
    color = np.zeros((nh, nw), dtype=np.int32)
    alpha = np.zeros((nh, nw), dtype=bool)
    for j in range(nh):
        for i in range(nw):
            bm = mask[ys[j]:ys[j + 1], xs[i]:xs[i + 1]]
            if bm.size == 0 or bm.mean() < 0.5:
                continue
            counts = np.bincount(idx[ys[j]:ys[j + 1], xs[i]:xs[i + 1]][bm], minlength=k)
            alpha[j, i] = True
            # palette[0] là màu outline: ưu tiên khi chiếm >= 30% khối
            color[j, i] = 0 if counts[0] >= 0.3 * counts.sum() else counts.argmax()
    alpha = _remove_specks(alpha)
    out = np.zeros((nh, nw, 4), dtype=np.uint8)
    out[..., :3] = palette.rgb[color].astype(np.uint8)
    out[..., 3] = alpha * 255
    return out


def process_sprite(raw, key_hex, palette, target_h):
    img = load(raw)
    m = _erode(key_mask(img, key_hex), 2)
    bb = _bbox(m)
    if not bb:
        raise RuntimeError("Không tìm thấy đối tượng trên nền")
    x0, y0, x1, y1 = bb
    crop, cm = img.crop(bb), m[y0:y1, x0:x1]
    return downscale_rgba(crop, cm, target_h / (y1 - y0), palette)


# ---------------- animation ----------------
def _label(mask):
    """Gán nhãn thành phần liên thông (4-hướng). Trả về (labels, số nhãn)."""
    h, w = mask.shape
    lab = np.zeros((h, w), dtype=np.int32)
    n = 0
    for y, x in zip(*np.nonzero(mask)):
        if lab[y, x]:
            continue
        n += 1
        lab[y, x] = n
        stack = [(y, x)]
        while stack:
            cy, cx = stack.pop()
            for ny, nx in ((cy + 1, cx), (cy - 1, cx), (cy, cx + 1), (cy, cx - 1)):
                if 0 <= ny < h and 0 <= nx < w and mask[ny, nx] and not lab[ny, nx]:
                    lab[ny, nx] = n
                    stack.append((ny, nx))
    return lab, n


def _upsample(cm, step, mask):
    h, w = mask.shape
    full = np.repeat(np.repeat(cm, step, 0), step, 1)[:h, :w]
    full = np.pad(full, ((0, h - full.shape[0]), (0, w - full.shape[1])))
    return full & mask


def _split_by_figures(lab, n, step, mask):
    """Không giả định bố cục: mảnh lớn = một nhân vật (frame), mảnh nhỏ (tia lửa, kiếm rời) gắn vào nhân vật gần nhất.
    Bỏ qua đường kẻ lưới mô hình tự vẽ. Sắp xếp theo hàng rồi theo cột."""
    hs, ws = lab.shape
    comps = []
    for i in range(1, n + 1):
        ys, xs = np.nonzero(lab == i)
        bw, bh = xs.max() - xs.min() + 1, ys.max() - ys.min() + 1
        if (bw > 0.6 * ws or bh > 0.6 * hs) and len(xs) < 0.05 * bw * bh:
            continue  # đường kẻ lưới
        comps.append({"i": i, "area": len(xs), "bb": (xs.min(), ys.min(), xs.max(), ys.max()),
                      "c": (xs.mean(), ys.mean())})
    if not comps:
        return []
    big = max(c["area"] for c in comps)
    seeds = [c for c in comps if c["area"] >= 0.3 * big]
    groups = {id(s): [s] for s in seeds}
    for c in comps:
        if c in seeds:
            continue
        def gap(s):
            x0, y0, x1, y1 = s["bb"]
            cx, cy = c["c"]
            return max(x0 - cx, 0, cx - x1) + max(y0 - cy, 0, cy - y1)
        groups[id(min(seeds, key=gap))].append(c)
    # thứ tự đọc: gom hàng theo tâm y (sai lệch < nửa chiều cao trung vị), rồi trái → phải
    med_h = np.median([s["bb"][3] - s["bb"][1] for s in seeds])
    seeds.sort(key=lambda s: s["c"][1])
    rows, cur = [], [seeds[0]]
    for s in seeds[1:]:
        if abs(s["c"][1] - cur[-1]["c"][1]) < med_h / 2:
            cur.append(s)
        else:
            rows.append(cur); cur = [s]
    rows.append(cur)
    ordered = [s for r in rows for s in sorted(r, key=lambda s: s["c"][0])]
    out = []
    for s in ordered:
        cm = np.isin(lab, [c["i"] for c in groups[id(s)]])
        out.append(_upsample(cm, step, mask))
    return out


def split_frames(mask, frames, cols, rows, step=4):
    """Tách frame. Mô hình không phải lúc nào cũng vẽ đúng bố cục yêu cầu (có lúc 2x2, 2x4, thêm đường kẻ),
    nên thử 2 cách: (1) theo nhân vật — không giả định bố cục; (2) theo lưới yêu cầu — xử lý trường hợp
    kiếm của frame này chạm frame bên cạnh. Ưu tiên cách cho đúng số frame; nếu không, dùng cách (1).
    Trả về list mask (cùng kích thước ảnh) theo thứ tự đọc."""
    small = mask[::step, ::step]
    lab, n = _label(small)
    by_fig = _split_by_figures(lab, n, step, mask)
    if len(by_fig) == frames:
        return by_fig
    widths = [(_bbox(f)[2] - _bbox(f)[0]) for f in by_fig]
    merged = len(by_fig) < frames and widths and (len(widths) == 1 or max(widths) > 1.6 * np.median(widths))
    if merged:  # có frame dính nhau → thử chia theo lưới yêu cầu
        by_grid = _split_by_grid(lab, n, step, mask, frames, cols, rows)
        if len(by_grid) == frames:
            return by_grid
    print(f"  ! mô hình vẽ {len(by_fig)} frame (yêu cầu {frames}) — dùng {len(by_fig)} frame")
    return by_fig


def _split_by_grid(lab, n, step, mask, frames, cols, rows):
    """Mỗi mảnh được gán vào ô lưới chứa trọng tâm của nó; mảnh dài vắt qua nhiều ô bị cắt theo ranh giới ô."""
    h, w = mask.shape
    small = lab > 0
    cells = [np.zeros_like(small) for _ in range(cols * rows)]
    cell_w, cell_h = w / cols / step, h / rows / step
    for i in range(1, n + 1):
        comp = lab == i
        ys, xs = np.nonzero(comp)
        if xs.max() - xs.min() > 1.3 * cell_w or ys.max() - ys.min() > 1.3 * cell_h:
            # mảnh dính sang frame khác (vd. kiếm chạm đầu frame bên cạnh) → cắt theo ranh giới lưới
            cidx = np.minimum((xs / cell_w).astype(int), cols - 1) + cols * np.minimum((ys / cell_h).astype(int), rows - 1)
            for k in np.unique(cidx):
                sel = cidx == k
                cells[k][ys[sel], xs[sel]] = True
            continue
        c = min(int(xs.mean() / cell_w), cols - 1)
        r = min(int(ys.mean() / cell_h), rows - 1)
        cells[r * cols + c] |= comp
    return [_upsample(cm, step, mask) for cm in cells[:frames] if cm.any()]


def _foot_x(alpha):
    """Tâm ngang của phần chân (15% dưới cùng) — dùng để căn nhân vật, bỏ qua kiếm/hiệu ứng chìa ra."""
    ys, xs = np.nonzero(alpha)
    cut = ys.max() - max(1, int((ys.max() - ys.min()) * 0.15))
    return int(np.median(xs[ys >= cut]))


def process_anim(raw, key_hex, palette, target_h, frames, cols, rows):
    """→ (sheet RGBA ndarray, meta dict). Mọi frame dùng CHUNG một hệ số scale, căn theo chân và mặt đất."""
    img = load(raw)
    m = _erode(key_mask(img, key_hex), 2)
    masks = split_frames(m, frames, cols, rows)
    if not masks:
        raise RuntimeError("Không tách được frame nào")
    boxes = [_bbox(fm) for fm in masks]
    heights = sorted(b[3] - b[1] for b in boxes)
    scale = target_h / heights[len(heights) // 2]  # chiều cao trung vị = chiều cao nhân vật
    small = [downscale_rgba(img.crop(b), fm[b[1]:b[3], b[0]:b[2]], scale, palette) for b, fm in zip(boxes, masks)]
    small = [s for s in small if s[..., 3].any()]
    feet = [_foot_x(s[..., 3] > 0) for s in small]
    half = max(max(fx, s.shape[1] - fx) for fx, s in zip(feet, small))
    cw = int(math.ceil((2 * half + 4) / 8) * 8)
    ch = int(math.ceil((max(s.shape[0] for s in small) + 4) / 8) * 8)
    sheet = np.zeros((ch, cw * len(small), 4), dtype=np.uint8)
    for i, (s, fx) in enumerate(zip(small, feet)):
        sh, sw = s.shape[:2]
        x = i * cw + cw // 2 - fx
        sheet[ch - sh - 2:ch - 2, x:x + sw] = s  # chân đặt cách đáy 2px; pivot tại chân
    meta = {"frames": len(small), "cell_w": cw, "cell_h": ch, "pivot": [0.5, round(2 / ch, 4)], "requested_frames": frames}
    return sheet, meta

# ---------------- tileset / background ----------------
def process_tileset(raw, palette, tile, cols, rows, key_hex):
    img = load(raw)
    m = key_mask(img, key_hex, sample="edges")
    return downscale_rgba(img, m, None, palette, size=(cols * tile, rows * tile))


def process_background(raw, palette, width, height, key_hex=None):
    img = load(raw)
    m = key_mask(img, key_hex, strict=True, sample="top") if key_hex else np.ones((img.height, img.width), dtype=bool)
    crop, cm, _ = _cover(img, m, width, height)
    return downscale_rgba(crop, cm, None, palette, size=(width, height))

def _cover(img, mask, width, height):
    w, h = img.size
    s = max(width / w, height / h)
    cw, ch = round(width / s), round(height / s)
    x0, y0 = (w - cw) // 2, (h - ch) // 2
    crop = img.crop((x0, y0, x0 + cw, y0 + ch))
    cm = mask[y0:y0 + ch, x0:x0 + cw] if mask is not None else None
    return crop, cm, width / cw


def save_rgba(arr, path, preview_scale=0):
    path.parent.mkdir(parents=True, exist_ok=True)
    im = Image.fromarray(arr, "RGBA")
    im.save(path)
    if preview_scale:
        prev = path.with_name(path.stem + "_preview.png")
        bg = Image.new("RGBA", im.size, (40, 40, 48, 255))
        bg.alpha_composite(im)
        bg.resize((im.width * preview_scale, im.height * preview_scale), Image.NEAREST).save(prev)
        return prev
