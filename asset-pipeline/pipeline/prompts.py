"""Ghép prompt từ bible + manifest. Đây là nơi DUY NHẤT tạo prompt — giữ mọi asset cùng một phong cách."""
from .core import Context


def _clean(s):
    return " ".join(str(s).split())


def anim_layout(frames):
    """(cols, rows, aspect) cho dải animation trên một ảnh."""
    if frames <= 4:
        return frames, 1, "16:9" if frames >= 3 else "4:3"
    cols = (frames + 1) // 2
    return cols, 2, "16:9"


def image_job(ctx: Context, a):
    """Trả về (prompt, refs[list[(Path, vai trò)]], aspect_ratio)."""
    art = ctx.art(a)
    key = art["background_key"]
    refs = []
    style = ctx.style_ref(ctx.style_name(a))
    if style and a["type"] != "style":
        refs.append((style, "reference for art style, palette, lighting and rendering ONLY — match that look but do not copy its layout or content"))

    ent = ctx.entities.get(a.get("entity")) if a.get("entity") else None
    if ent:
        if ent.get("based_on") and ctx.entity_ref(ent["based_on"]):
            refs.append((ctx.entity_ref(ent["based_on"]), f"base design of {ctx.entities[ent['based_on']]['name']} — keep face, proportions and silhouette"))
        if a["type"] != "anchor" and ctx.entity_ref(a["entity"]):
            refs.append((ctx.entity_ref(a["entity"]), f"character design sheet of {ent['name']} — keep design, colors and proportions identical"))

    zone = ctx.zones.get(a.get("zone")) if a.get("zone") else None
    parts = [_clean(art["style_prompt"]), _clean(art["view"]) + "."]
    t = a["type"]
    aspect = "1:1"

    if t == "style":
        parts.append(_clean(a["prompt"]))
        parts.append(f"Game premise for mood: {_clean(ctx.style['game']['premise'])}")
    elif t == "anchor":
        parts.append(
            f"Character design reference sheet of {ent['name']}: {_clean(ent['description'])} "
            f"Show the full body twice: side view facing right in a neutral standing pose, and a front view. "
            f"Same scale for both, generous spacing, solid flat {key} background, no ground, no shadow."
        )
        aspect = "16:9"
    elif t == "sprite":
        parts.append(f"Single sprite of {ent['name']}: {_clean(ent['description'])} {_clean(a.get('action', ''))}")
        parts.append(f"Centered, full body visible, solid flat {key} background, no ground, no shadow.")
    elif t == "anim":
        cols, rows, aspect = anim_layout(a["frames"])
        grid = f"in a single row of {cols} frames" if rows == 1 else f"in a grid of {rows} rows x {cols} columns, read left-to-right then top-to-bottom"
        parts.append(
            f"Animation sprite sheet of {ent['name']} ({_clean(ent['description'])}) performing: {_clean(a['action'])}. "
            f"Exactly {a['frames']} sequential frames {grid}, every frame the same character at the same scale and "
            f"the same ground line. Each frame sits fully inside its own equal-size cell with wide empty margins; "
            f"no part of a frame (including the weapon or effects) touches or crosses into a neighbouring cell. "
            f"Solid flat {key} background, no ground, no shadow, no frame numbers."
        )
    elif t == "parts":
        parts.append(
            f"{_clean(a['prompt'])} Character: {ent['name']} — {_clean(ent['description'])} "
            f"Solid flat {key} background everywhere between the parts, no labels, no numbers, no arrows, no shadows."
        )
        aspect = "16:9"
    elif t == "tileset":
        parts.append(
            f"Zone: {_clean(zone['description'])} {_clean(a['prompt'])} "
            f"Arranged as an exact grid of {a['cols']} columns x {a['rows']} rows of square tiles, no gaps, no borders between tiles. "
            f"Tiles contain only the solid material; any empty area inside a tile (above a ground edge, around broken pieces) "
            f"is solid flat {key}. No sky, no scenery, no characters. This is a tileset sheet, not a scene."
        )
        aspect = "3:2" if a["cols"] / a["rows"] >= 1.4 else "4:3"
    elif t == "background":
        parts.append(f"Zone: {_clean(zone['description'])} {_clean(a['prompt'])}. Wide 2D side-scrolling game background, no characters.")
        if a.get("transparent"):
            parts.append(f"Empty areas must be solid flat {key}.")
        aspect = "16:9"
    else:
        raise ValueError(f"{t} không phải loại ảnh")

    if refs:
        roles = "; ".join(f"reference image {i + 1} is the {role}" for i, (_, role) in enumerate(refs))
        parts.append(f"Use the attached images: {roles}.")
    parts.append("Constraints: " + _clean(art["negative_prompt"]))
    return "\n".join(parts), refs, aspect


def music_job(ctx: Context, a):
    au = ctx.style["audio"]
    zone = ctx.zones[a["zone"]]
    prompt = f"{_clean(a['prompt'])}. Mood: {_clean(zone['music_mood'])}. Style: {_clean(au['music_style'])}. Instrumental, seamless loopable."
    return prompt, _clean(au["music_negative"])


def voice_job(ctx: Context, a):
    ent = ctx.entities[a["entity"]]
    v = ent["voice"]
    text = f"{ctx.style['audio']['voice']['style_prompt']} ({v['direction']}) {a['line']}"
    return text, v["name"], v.get("fx")
