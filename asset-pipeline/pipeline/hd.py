"""Hậu kỳ cho phong cách HD (chân thật): tách nền mềm, khử viền màu nền, cắt sát, thu nhỏ chất lượng cao.
Khác pixel: giữ alpha mềm (tóc, khói), không khóa bảng màu."""
import math

import numpy as np
from PIL import Image

from .pixel import _bbox, _erode, _foot_x, _label, _upsample, hex_to_rgb, load, split_frames


def soft_key(img, key_hex, inner=45.0, outer=120.0, band=6):
    """→ RGBA ndarray (float 0..255). Alpha tăng dần theo khoảng cách màu tới nền (đo từ 4 góc).
    Trong dải `band` px sát nền: khử màu nền ám vào viền (despill)."""
    a = np.asarray(img, dtype=np.float64)
    pts = [a[:8, :8], a[:8, -8:], a[-8:, :8], a[-8:, -8:]]
    bg = np.median(np.concatenate([p.reshape(-1, 3) for p in pts]), axis=0)
    dist = np.linalg.norm(a - bg, axis=-1)
    alpha = np.clip((dist - inner) / (outer - inner), 0, 1)
    near = ~_erode(alpha > 0.98, band)
    rgb = a.copy()
    key = hex_to_rgb(key_hex)
    k = int(np.argmax(key))  # kênh trội của màu nền (xanh lá => 1)
    others = [i for i in range(3) if i != k]
    limit = np.maximum(rgb[..., others[0]], rgb[..., others[1]])
    # Bảng màu HD không có màu trùng kênh nền (không có xanh lá) => khử ám toàn ảnh, không chỉ ở viền:
    # vệt chém, khói, lửa bán trong suốt vẽ đè lên nền đều bị nhuộm màu nền.
    rgb[..., k] = np.minimum(rgb[..., k], limit)
    del near
    return np.dstack([rgb, alpha * 255])


def _resize_rgba(arr, size):
    """Thu nhỏ RGBA bằng LANCZOS trên ảnh premultiplied để viền không bị quầng sáng/tối."""
    a = arr[..., 3:4] / 255.0
    pre = np.dstack([arr[..., :3] * a, arr[..., 3:4]]).astype(np.float32)
    chans = [np.asarray(Image.fromarray(pre[..., i]).resize(size, Image.LANCZOS)) for i in range(4)]
    out_a = np.clip(chans[3], 0, 255)
    rgb = np.dstack(chans[:3]) / np.maximum(out_a[..., None] / 255.0, 1e-4)
    return np.dstack([np.clip(rgb, 0, 255), out_a]).astype(np.uint8)


def _crop(rgba, bb, pad=4):
    x0, y0, x1, y1 = bb
    h, w = rgba.shape[:2]
    return rgba[max(0, y0 - pad):min(h, y1 + pad), max(0, x0 - pad):min(w, x1 + pad)]


def process_sprite(raw, key_hex, target_h):
    rgba = soft_key(load(raw), key_hex)
    bb = _bbox(rgba[..., 3] > 20)
    if not bb:
        raise RuntimeError("Không tìm thấy nhân vật trên nền")
    c = _crop(rgba, bb)
    s = target_h / c.shape[0]
    return _resize_rgba(c, (max(1, round(c.shape[1] * s)), target_h))


def process_anim(raw, key_hex, target_h, frames, cols, rows):
    """Giống pixel.process_anim nhưng giữ alpha mềm và resize chất lượng cao."""
    rgba = soft_key(load(raw), key_hex)
    mask = rgba[..., 3] > 20
    masks = split_frames(mask, frames, cols, rows)
    if not masks:
        raise RuntimeError("Không tách được frame nào")
    boxes = [_bbox(m) for m in masks]
    heights = sorted(b[3] - b[1] for b in boxes)
    scale = target_h / heights[len(heights) // 2]
    small = []
    for b, m in zip(boxes, masks):
        c = _crop(rgba * np.dstack([np.ones(mask.shape + (3,)), m[..., None]]), b, pad=0)
        small.append(_resize_rgba(c, (max(1, round(c.shape[1] * scale)), max(1, round(c.shape[0] * scale)))))
    # căn theo chân (không theo khung bao) để kiếm/áo choàng chìa ra không làm nhân vật trượt qua lại giữa các frame
    feet = [_foot_x(s[..., 3] > 128) for s in small]
    half = max(max(fx, s.shape[1] - fx) for fx, s in zip(feet, small))
    cw = int(math.ceil((2 * half + 8) / 8) * 8)
    ch = int(math.ceil((max(s.shape[0] for s in small) + 8) / 8) * 8)
    sheet = np.zeros((ch, cw * len(small), 4), dtype=np.uint8)
    for i, (s, fx) in enumerate(zip(small, feet)):
        sh, sw = s.shape[:2]
        x = i * cw + cw // 2 - fx
        sheet[ch - sh - 4:ch - 4, x:x + sw] = s
    return sheet, {"frames": len(small), "cell_w": cw, "cell_h": ch, "pivot": [0.5, round(4 / ch, 4)], "requested_frames": frames}


def sprite_pivot_meta(arr):
    """Sprite đơn: pivot đặt tại chân (đo từ 15% dưới cùng) → *.sheet.json một frame cho importer Unity."""
    h, w = arr.shape[:2]
    return {"frames": 1, "cell_w": w, "cell_h": h, "pivot": [round(_foot_x(arr[..., 3] > 128) / w, 4), 0.0]}


def process_parts(raw, key_hex, min_area_ratio=0.004):
    """Tách tấm "rig parts" thành từng mảnh riêng (theo thành phần liên thông). → list[(bbox, RGBA)] theo thứ tự đọc."""
    rgba = soft_key(load(raw), key_hex)
    mask = rgba[..., 3] > 30
    step = 4
    lab, n = _label(mask[::step, ::step])
    total = mask.size / (step * step)
    pieces = []
    for i in range(1, n + 1):
        comp = lab == i
        if comp.sum() < min_area_ratio * total:
            continue
        full = _upsample(comp, step, mask)
        bb = _bbox(full)
        part = rgba.copy()
        part[..., 3] *= full
        pieces.append((bb, _crop(part, bb).astype(np.uint8)))
    pieces.sort(key=lambda p: (p[0][1] // 200, p[0][0]))
    return pieces


def process_tileset(raw, key_hex, tile, cols, rows):
    """Lưới tile HD: tách nền nếu mô hình dùng đúng nền khóa (đo ở viền ảnh), rồi resize về cols*tile x rows*tile."""
    img = load(raw)
    a = np.asarray(img, dtype=np.float64)
    border = np.concatenate([a[0], a[-1], a[:, 0], a[:, -1]])
    keyed = np.mean(np.linalg.norm(border - hex_to_rgb(key_hex), axis=1) < 90) > 0.3
    rgba = soft_key(img, key_hex) if keyed else np.dstack([a, np.full(a.shape[:2], 255.0)])
    return _resize_rgba(rgba, (cols * tile, rows * tile))


def process_background(raw, width, height):
    img = load(raw)
    w, h = img.size
    s = max(width / w, height / h)
    cw, ch = round(width / s), round(height / s)
    x0, y0 = (w - cw) // 2, (h - ch) // 2
    img = img.crop((x0, y0, x0 + cw, y0 + ch)).resize((width, height), Image.LANCZOS)
    a = np.asarray(img)
    return np.dstack([a, np.full(a.shape[:2], 255, np.uint8)])


def save(arr, path, checker_preview=True):
    """Lưu RGBA; kèm _preview.png ghép trên nền tối để xem viền tách nền."""
    path.parent.mkdir(parents=True, exist_ok=True)
    im = Image.fromarray(arr.astype(np.uint8), "RGBA")
    im.save(path)
    if checker_preview:
        bg = Image.new("RGBA", im.size, (24, 22, 28, 255))
        bg.alpha_composite(im)
        bg.save(path.with_name(path.stem + "_preview.png"))
