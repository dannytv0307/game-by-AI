"""Nạp cấu hình, bible, manifest; ghi log nguồn gốc; kiểm soát ngân sách gọi API."""
import hashlib
import json
import time
from pathlib import Path

import yaml

ROOT = Path(__file__).resolve().parent.parent


def _load(rel):
    with open(ROOT / rel, encoding="utf-8-sig") as f:
        return yaml.safe_load(f)


class Context:
    def __init__(self):
        self.config = _load("config.yaml")
        # "pixel" là phong cách chính (cũng chứa phần audio); "hd" là nhánh thử nghiệm chân thật
        self.styles = {"pixel": _load("bible/style.yaml"), "hd": _load("bible/style_hd.yaml")}
        self.style = self.styles["pixel"]
        ents = _load("bible/entities.yaml")
        self.entities = ents.get("entities", {})
        self.zones = ents.get("zones", {})
        self.assets = {a["id"]: a for a in _load("manifest.yaml")["assets"]}
        p = self.config["paths"]
        self.refs_dir = ROOT / p["refs"]
        self.cand_dir = ROOT / p["candidates"]
        self.log_path = ROOT / p["log"]
        self.art_root = (ROOT / p["unity_art_root"]).resolve()
        self.art_hd_root = (ROOT / p["unity_art_hd_root"]).resolve()
        self.audio_root = (ROOT / p["unity_audio_root"]).resolve()
        self.calls = 0

    # ---- phong cách ----
    @staticmethod
    def style_name(a):
        return a.get("style", "hd")  # pixel đã gỡ khỏi game (2026-10-08)

    def art(self, a):
        return self.styles[self.style_name(a)]["art"]

    # ---- refs (anchors) ----
    def _style_file(self, style):
        return self.refs_dir / ("_style.png" if style == "pixel" else f"_style_{style}.png")

    def style_ref(self, style="pixel"):
        f = self._style_file(style)
        return f if f.exists() else None

    def entity_ref(self, entity):
        f = self.refs_dir / f"{entity}.png"
        return f if f.exists() else None

    def concept_refs(self, name, limit=3):
        """Concept art người dùng đưa vào docs/concepts/<entity|zone|_style>/ (png/jpg/webp), tối đa `limit` ảnh."""
        d = ROOT.parent / "docs" / "concepts" / name
        if not d.is_dir() and name.endswith("_hd"):  # hero_hd dùng chung concept với hero
            d = d.with_name(name[:-3])
        if not d.is_dir():
            return []
        files = sorted(p for p in d.iterdir() if p.suffix.lower() in (".png", ".jpg", ".jpeg", ".webp"))
        return files[:limit]

    def asset(self, asset_id):
        if asset_id not in self.assets:
            raise SystemExit(f"Không có asset '{asset_id}' trong manifest.yaml")
        return self.assets[asset_id]

    def dest_path(self, a):
        """Đích cuối cùng của một asset sau khi duyệt."""
        if a["type"] == "style":
            return self._style_file(self.style_name(a))
        if a["type"] == "anchor":
            return self.refs_dir / f"{a['entity']}.png"
        if a["type"] in ("music", "voice", "sfx"):
            return self.audio_root / a["out"]
        return (self.art_hd_root if self.style_name(a) == "hd" else self.art_root) / a["out"]

    # ---- ngân sách ----
    def spend(self, n, allow_over):
        limit = self.config["budget"]["max_calls_per_run"]
        if self.calls + n > limit and not allow_over:
            raise SystemExit(f"Vượt giới hạn {limit} lần gọi API cho một lệnh. Thêm --yes nếu chắc chắn.")
        self.calls += n

    # ---- log nguồn gốc ----
    def log(self, **entry):
        self.log_path.parent.mkdir(parents=True, exist_ok=True)
        entry["ts"] = time.strftime("%Y-%m-%dT%H:%M:%S")
        with open(self.log_path, "a", encoding="utf-8") as f:
            f.write(json.dumps(entry, ensure_ascii=False) + "\n")


def sha1(path):
    return hashlib.sha1(Path(path).read_bytes()).hexdigest()[:12]


def stable_seed(text):
    return int(hashlib.sha1(text.encode()).hexdigest()[:8], 16) % 2_000_000_000
