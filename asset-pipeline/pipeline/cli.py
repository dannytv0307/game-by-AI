"""CLI của pipeline asset DevilBlade.

  python -m pipeline status                     # asset nào đã duyệt / còn thiếu / thiếu anchor
  python -m pipeline prompt <id>                # xem prompt + refs sẽ gửi (không tốn tiền)
  python -m pipeline gen <id> [<id>...] [-n 2] [--draft] [--yes]
  python -m pipeline reprocess <id>             # chạy lại hậu kỳ trên ảnh gốc (không gọi API)
  python -m pipeline review <id>                # mô hình nghe & chấm ứng viên music/voice
  python -m pipeline approve <id> [--pick 1]    # đưa ứng viên vào refs/ hoặc dự án Unity
  python -m pipeline sfx [--all | <id>...]      # SFX tổng hợp (miễn phí, duyệt luôn)
"""
import argparse
import json
import shutil
import time

from . import audio, hd, pixel, prompts, sfx, vertex
from .core import Context, sha1, stable_seed

IMAGE_TYPES = {"style", "anchor", "sprite", "anim", "tileset", "background", "parts"}


def _check_prereq(ctx, a):
    """Thứ tự bắt buộc: style tile (của đúng phong cách) -> anchor của entity -> các asset của entity."""
    style = ctx.style_name(a)
    if a["type"] != "style" and a["type"] in IMAGE_TYPES and not ctx.style_ref(style):
        tile = "style_tile" if style == "pixel" else f"style_tile_{style}"
        raise SystemExit(f"Chưa có style tile cho phong cách '{style}'. Gen và approve '{tile}' trước.")
    ent = ctx.entities.get(a.get("entity") or "")
    if ent and ent.get("style", "pixel") != style:
        raise SystemExit(f"Entity '{a['entity']}' thuộc phong cách '{ent.get('style', 'pixel')}', asset lại là '{style}'.")
    if a["type"] == "anchor" and ent and ent.get("based_on") and not ctx.entity_ref(ent["based_on"]):
        raise SystemExit(f"Cần anchor của '{ent['based_on']}' trước.")
    if a["type"] in ("sprite", "anim", "parts") and not ctx.entity_ref(a["entity"]):
        raise SystemExit(f"Chưa có anchor refs/{a['entity']}.png. Gen và approve '{a['entity']}_anchor' trước.")


def _run_dir(ctx, aid):
    d = ctx.cand_dir / aid / time.strftime("%Y%m%d-%H%M%S")
    d.mkdir(parents=True, exist_ok=True)
    return d


def gen_image(ctx, a, n, draft, allow_over):
    _check_prereq(ctx, a)
    prompt, refs, aspect = prompts.image_job(ctx, a)
    model = ctx.config["models"]["image_draft" if draft else "image"]
    # HD cần độ phân giải cao hơn (giá cao hơn); pixel dùng mặc định của mô hình
    size = ctx.config["models"].get("image_size_hd") if ctx.style_name(a) == "hd" and not draft else None
    ctx.spend(n, allow_over)
    out = _run_dir(ctx, a["id"])
    (out / "prompt.txt").write_text(prompt, encoding="utf-8")
    for k in range(1, n + 1):
        print(f"[{a['id']}] ứng viên {k}/{n} — {model}{' ' + size if size else ''}")
        raw = vertex.generate_image(ctx.config, model, prompt, [p for p, _ in refs], aspect, size)
        (out / f"{k}_raw.png").write_bytes(raw)
        meta = postprocess_image(ctx, a, out, k)
        ctx.log(id=a["id"], type=a["type"], model=model, prompt=prompt, aspect=aspect,
                refs=[{"path": str(p.name), "sha1": sha1(p)} for p, _ in refs],
                output=str(out / f"{k}_raw.png"), sha1=sha1(out / f"{k}_raw.png"), meta=meta)
    print(f"→ ứng viên tại {out}")


def postprocess_image(ctx, a, out, k):
    """Ảnh gốc {k}_raw.png -> {k}.png (+ _preview, + {k}.json cho anim). Chạy lại được mà không tốn API."""
    if ctx.style_name(a) == "hd":
        return _postprocess_hd(ctx, a, out, k)
    art = ctx.style["art"]
    pal = pixel.Palette(art["palette"])
    key = art["background_key"]
    ent = ctx.entities.get(a.get("entity") or "")
    raw = (out / f"{k}_raw.png").read_bytes()
    t = a["type"]
    meta = {}
    try:
        if t == "anchor":
            # anchor giữ ảnh gốc làm tham chiếu cho mô hình; đây chỉ là preview bản pixel để duyệt
            pixel.save_rgba(pixel.process_sprite(raw, key, pal, ent["height_px"] * 2), out / f"{k}.png", 6)
        elif t == "sprite":
            pixel.save_rgba(pixel.process_sprite(raw, key, pal, ent["height_px"]), out / f"{k}.png", 8)
        elif t == "anim":
            cols, rows, _ = prompts.anim_layout(a["frames"])
            arr, meta = pixel.process_anim(raw, key, pal, ent["height_px"], a["frames"], cols, rows)
            pixel.save_rgba(arr, out / f"{k}.png", 6)
        elif t == "tileset":
            pixel.save_rgba(pixel.process_tileset(raw, pal, art["pixel"]["tile"], a["cols"], a["rows"], key), out / f"{k}.png", 3)
        elif t == "background":
            arr = pixel.process_background(raw, pal, a["width"], a["height"], key if a.get("transparent") else None)
            pixel.save_rgba(arr, out / f"{k}.png", 2)
    except Exception as e:  # ảnh gốc vẫn được giữ để xem lại
        print(f"  ! hậu kỳ lỗi: {e}")
        meta["postprocess_error"] = str(e)
    if meta:
        (out / f"{k}.json").write_text(json.dumps(meta, indent=2), encoding="utf-8")
    return meta


def _postprocess_hd(ctx, a, out, k):
    art = ctx.art(a)
    key = art["background_key"]
    ent = ctx.entities.get(a.get("entity") or "")
    raw = (out / f"{k}_raw.png").read_bytes()
    t = a["type"]
    meta = {}
    try:
        if t in ("anchor", "sprite"):
            hd.save(hd.process_sprite(raw, key, ent["height_px"] * (2 if t == "anchor" else 1)), out / f"{k}.png")
        elif t == "anim":
            cols, rows, _ = prompts.anim_layout(a["frames"])
            arr, meta = hd.process_anim(raw, key, ent["height_px"], a["frames"], cols, rows)
            hd.save(arr, out / f"{k}.png")
        elif t == "parts":
            pieces = hd.process_parts(raw, key)
            pdir = out / f"{k}_parts"
            for i, (bb, arr) in enumerate(pieces):
                hd.save(arr, pdir / f"part_{i:02d}.png", checker_preview=False)
            meta = {"parts": [{"file": f"part_{i:02d}.png", "bbox": [int(v) for v in bb]} for i, (bb, _) in enumerate(pieces)]}
            # bản xem nhanh: toàn bộ tấm đã tách nền
            hd.save(hd.soft_key(pixel.load(raw), key).astype("uint8"), out / f"{k}.png")
            print(f"  → tách được {len(pieces)} bộ phận")
        elif t == "background":
            hd.save(hd.process_background(raw, a["width"], a["height"]), out / f"{k}.png", checker_preview=False)
        elif t == "style":
            pass  # style tile dùng ảnh gốc làm tham chiếu
    except Exception as e:
        print(f"  ! hậu kỳ lỗi: {e}")
        meta["postprocess_error"] = str(e)
    if meta:
        (out / f"{k}.json").write_text(json.dumps(meta, indent=2), encoding="utf-8")
    return meta


def cmd_reprocess(ctx, args):
    a = ctx.asset(args.id)
    run = _latest_run(ctx, args.id)
    for raw in sorted(run.glob("*_raw.png")):
        k = int(raw.name.split("_")[0])
        postprocess_image(ctx, a, run, k)
        print(f"✔ {run / f'{k}.png'}")


def gen_music(ctx, a, n, allow_over):
    prompt, neg = prompts.music_job(ctx, a)
    ctx.spend(n, allow_over)
    out = _run_dir(ctx, a["id"])
    (out / "prompt.txt").write_text(f"{prompt}\nNEGATIVE: {neg}", encoding="utf-8")
    model = ctx.config["models"]["music"]
    for k in range(1, n + 1):
        seed = stable_seed(a["id"]) + k
        print(f"[{a['id']}] ứng viên {k}/{n} — {model} seed={seed}")
        x, sr = audio.read_wav_bytes(vertex.generate_music(ctx.config, model, prompt, neg, seed))
        x = audio.normalize_rms(audio.make_loop(x, sr), ctx.style["audio"]["music_target_rms_db"])
        audio.write_wav(out / f"{k}.wav", x, sr)
        ctx.log(id=a["id"], type="music", model=model, prompt=prompt, negative=neg, seed=seed, output=str(out / f"{k}.wav"))
    print(f"→ ứng viên tại {out}")


def gen_voice(ctx, a, n, allow_over):
    text, voice, fx = prompts.voice_job(ctx, a)
    ctx.spend(n, allow_over)
    out = _run_dir(ctx, a["id"])
    (out / "prompt.txt").write_text(f"{text}\nVOICE: {voice} FX: {fx}", encoding="utf-8")
    model = ctx.config["models"]["tts"]
    for k in range(1, n + 1):
        print(f"[{a['id']}] ứng viên {k}/{n} — {model} voice={voice}")
        x, sr = audio.pcm16_to_array(vertex.generate_speech(ctx.config, model, text, voice)), 24000
        x = audio.trim_silence(x, sr)
        if fx == "demon":
            x = audio.demon_fx(x, sr)
        audio.write_wav(out / f"{k}.wav", audio.normalize_rms(x, ctx.style["audio"]["voice_target_rms_db"]), sr)
        ctx.log(id=a["id"], type="voice", model=model, prompt=text, voice=voice, fx=fx, output=str(out / f"{k}.wav"))
    print(f"→ ứng viên tại {out}")


def cmd_gen(ctx, args):
    n = args.n or ctx.config["budget"]["candidates_default"]
    for aid in args.ids:
        a = ctx.asset(aid)
        if a["type"] in IMAGE_TYPES:
            gen_image(ctx, a, n, args.draft, args.yes)
        elif a["type"] == "music":
            gen_music(ctx, a, n, args.yes)
        elif a["type"] == "voice":
            gen_voice(ctx, a, n, args.yes)
        elif a["type"] == "sfx":
            cmd_sfx(ctx, argparse.Namespace(ids=[aid], all=False))


def _latest_run(ctx, aid):
    runs = sorted((ctx.cand_dir / aid).glob("*/")) if (ctx.cand_dir / aid).exists() else []
    if not runs:
        raise SystemExit(f"Chưa có ứng viên cho '{aid}'. Chạy gen trước.")
    return runs[-1]


def cmd_approve(ctx, args):
    a = ctx.asset(args.id)
    run = _latest_run(ctx, args.id) if not args.run else ctx.cand_dir / args.id / args.run
    k = args.pick
    dest = ctx.dest_path(a)
    dest.parent.mkdir(parents=True, exist_ok=True)
    if a["type"] in ("style", "anchor"):
        src = run / f"{k}_raw.png"   # tham chiếu cho mô hình = ảnh gốc độ phân giải cao
    elif a["type"] in ("music", "voice"):
        src = run / f"{k}.wav"
    else:
        src = run / f"{k}.png"
    if not src.exists():
        raise SystemExit(f"Không thấy {src}")
    shutil.copy2(src, dest)
    parts_dir = run / f"{k}_parts"
    if parts_dir.exists():  # các bộ phận đã tách để rig
        shutil.copytree(parts_dir, dest.with_name(dest.stem.removesuffix("_parts") + "_pieces"), dirs_exist_ok=True)
    side = run / f"{k}.json"
    if side.exists():  # thông tin cắt sprite sheet cho Unity importer
        shutil.copy2(side, dest.with_suffix(".sheet.json"))
    ctx.log(id=a["id"], type=a["type"], action="approve", source=str(src), dest=str(dest), sha1=sha1(dest))
    print(f"✔ {a['id']} → {dest}")
    if a["type"] in ("style", "anchor"):
        print("  ! Anchor thay đổi: các asset đã duyệt trước đó của entity này nên được gen lại để khớp.")


def cmd_sfx(ctx, args):
    ids = [i for i, a in ctx.assets.items() if a["type"] == "sfx"] if args.all else args.ids
    for aid in ids:
        a = ctx.asset(aid)
        seed = stable_seed(aid)
        x, sr = sfx.render(a["preset"], seed)
        x = audio.normalize_peak(x, ctx.style["audio"]["sfx_target_peak_db"])
        dest = ctx.dest_path(a)
        audio.write_wav(dest, x, sr)
        ctx.log(id=aid, type="sfx", preset=a["preset"], seed=seed, dest=str(dest), sha1=sha1(dest))
        print(f"✔ {aid} → {dest}")


def cmd_review(ctx, args):
    """Chấm điểm các ứng viên âm thanh của lần gen gần nhất bằng mô hình nghe được (không thay việc nghe thử)."""
    a = ctx.asset(args.id)
    run = _latest_run(ctx, args.id)
    if a["type"] == "music":
        prompt, neg = prompts.music_job(ctx, a)
        ask = (f"You are reviewing a candidate music track for a video game. Brief: {prompt} Must avoid: {neg}. "
               'Reply JSON: {"score": 1-10 fit to brief, "has_vocals": bool, "tempo_bpm_estimate": int, '
               '"mood": str, "issues": [str]}')
    elif a["type"] == "voice":
        text, voice, fx = prompts.voice_job(ctx, a)
        ask = (f"You are reviewing a voice line for a video game. Intended line: \"{a['line']}\". "
               f"Direction: {ctx.entities[a['entity']]['voice']['direction']}. "
               'Reply JSON: {"transcript": str, "line_correct": bool, "score": 1-10 fit to direction, "issues": [str]}')
    else:
        raise SystemExit("review chỉ dành cho music/voice")
    model = ctx.config["models"].get("judge", "gemini-2.5-flash")
    for wav in sorted(run.glob("[0-9].wav")):
        ctx.spend(1, args.yes)
        verdict = vertex.judge_audio(ctx.config, model, wav.read_bytes(), ask)
        (run / f"{wav.stem}.review.json").write_text(verdict, encoding="utf-8")
        print(f"{wav.name}: {verdict}")


def cmd_prompt(ctx, args):
    a = ctx.asset(args.id)
    if a["type"] in IMAGE_TYPES:
        p, refs, aspect = prompts.image_job(ctx, a)
        print(p, f"\n\nASPECT: {aspect}\nREFS: {[str(r[0].name) for r in refs]}")
    elif a["type"] == "music":
        print(*prompts.music_job(ctx, a), sep="\nNEGATIVE: ")
    elif a["type"] == "voice":
        print(prompts.voice_job(ctx, a))


def cmd_status(ctx, args):
    print(f"style tile: {'✔' if ctx.style_ref() else '✘'}")
    for e in ctx.entities:
        print(f"anchor {e:14s}: {'✔' if ctx.entity_ref(e) else '✘'}")
    print()
    for aid, a in ctx.assets.items():
        if a["type"] in ("style", "anchor"):
            continue
        done = ctx.dest_path(a).exists()
        cands = (ctx.cand_dir / aid).exists()
        print(f"{'✔' if done else ('…' if cands else '✘')} {a['type']:10s} {aid}")


def main():
    ap = argparse.ArgumentParser(prog="pipeline")
    sub = ap.add_subparsers(dest="cmd", required=True)
    g = sub.add_parser("gen"); g.add_argument("ids", nargs="+"); g.add_argument("-n", type=int)
    g.add_argument("--draft", action="store_true"); g.add_argument("--yes", action="store_true")
    p = sub.add_parser("approve"); p.add_argument("id"); p.add_argument("--pick", type=int, default=1); p.add_argument("--run")
    s = sub.add_parser("sfx"); s.add_argument("ids", nargs="*"); s.add_argument("--all", action="store_true")
    pr = sub.add_parser("prompt"); pr.add_argument("id")
    rp = sub.add_parser("reprocess"); rp.add_argument("id")
    rv = sub.add_parser("review"); rv.add_argument("id"); rv.add_argument("--yes", action="store_true")
    sub.add_parser("status")
    args = ap.parse_args()
    ctx = Context()
    {"gen": cmd_gen, "approve": cmd_approve, "sfx": cmd_sfx, "prompt": cmd_prompt, "status": cmd_status,
     "reprocess": cmd_reprocess, "review": cmd_review}[args.cmd](ctx, args)
