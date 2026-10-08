"""Ghép ảnh mockup trong game cho bản thử HD: nền HD + Kael (idle, chém) + dạng quỷ, ở đúng tỉ lệ khi chơi.
Chạy: .venv\\Scripts\\python tools\\hd_mockup.py  → ../docs/hd_trial/*.png"""
import json
from pathlib import Path

from PIL import Image, ImageDraw, ImageEnhance

ROOT = Path(__file__).resolve().parent.parent
ART = ROOT.parent / "DevilBlade" / "Assets" / "ArtHD"
OUT = ROOT.parent / "docs" / "hd_trial"


def sprite(rel):
    return Image.open(ART / rel).convert("RGBA")


def fit_h(im, h):
    return im.resize((round(im.width * h / im.height), h), Image.LANCZOS)


def first_frame(rel):
    sheet = sprite(rel)
    meta = json.loads((ART / rel).with_suffix(".sheet.json").read_text(encoding="utf-8"))
    return sheet.crop((0, 0, meta["cell_w"], meta["cell_h"]))


def place(canvas, im, x, ground):
    canvas.alpha_composite(im, (int(x - im.width / 2), int(ground - im.height)))


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    bg = sprite("Backgrounds/VillageHD/village_hd_bg.png")
    # nền tối hơn tiền cảnh để nhân vật nổi bật (giống cách làm ở bản pixel)
    bg = ImageEnhance.Brightness(bg).enhance(0.7)
    W, H = bg.size
    ground = int(H * 0.9)

    # 1) Cảnh chơi: Kael đứng — Kael chém — quỷ chém
    scene = bg.copy()
    place(scene, fit_h(first_frame("Characters/HeroHD/hero_hd_idle.png"), 360), W * 0.2, ground)
    place(scene, fit_h(sprite("Characters/HeroHD/hero_hd_attack_pose.png"), 380), W * 0.45, ground)
    place(scene, fit_h(sprite("Characters/HeroDemonHD/hero_demon_hd_attack_pose.png"), 520), W * 0.76, ground)
    scene.convert("RGB").save(OUT / "mockup_gameplay.png")

    # 2) Biến hình: người → đang biến → quỷ
    tr = bg.copy()
    place(tr, fit_h(first_frame("Characters/HeroHD/hero_hd_idle.png"), 360), W * 0.2, ground)
    place(tr, fit_h(sprite("Characters/HeroHD/hero_hd_transform_pose.png"), 420), W * 0.5, ground)
    place(tr, fit_h(sprite("Characters/HeroDemonHD/hero_demon_hd_attack_pose.png"), 520), W * 0.8, ground)
    tr.convert("RGB").save(OUT / "mockup_transform.png")

    # 3) Bộ phận để rig (nền tối, có số thứ tự)
    pieces = sorted((ART / "Characters/HeroHD/hero_hd_pieces").glob("part_*.png"))
    cell = 260
    cols = 5
    sheet = Image.new("RGBA", (cols * cell, ((len(pieces) + cols - 1) // cols) * cell), (24, 22, 28, 255))
    d = ImageDraw.Draw(sheet)
    for i, p in enumerate(pieces):
        im = Image.open(p).convert("RGBA")
        im.thumbnail((cell - 30, cell - 30), Image.LANCZOS)
        x, y = (i % cols) * cell, (i // cols) * cell
        sheet.alpha_composite(im, (x + (cell - im.width) // 2, y + 20 + (cell - 30 - im.height) // 2))
        d.text((x + 6, y + 4), p.stem, fill=(255, 210, 120, 255))
    sheet.convert("RGB").save(OUT / "rig_parts.png")
    print("Đã lưu:", *[p.name for p in OUT.glob("*.png")])


if __name__ == "__main__":
    main()
