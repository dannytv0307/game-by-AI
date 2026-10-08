# DevilBlade

Game hành động 2D pixel art (platformer + chiến đấu), dự án prototype một người chơi, chạy trên Windows.
Nhân vật chính Kael là chiến binh mang dòng máu quỷ, chiến đấu chống loài quỷ. Khi tích đủ nộ khí, Kael **hóa quỷ**.

## Cấu trúc

- `DevilBlade/`: dự án Unity 6000.3.25f1 (LTS), template Universal 2D (URP 2D Renderer), có thêm Cinemachine 3.
- `asset-pipeline/`: pipeline Python tạo đồ họa và âm thanh bằng Vertex AI (project GCP `hp-ecommerce-v2`).
- Cả hai nằm chung một repo git. File PNG/WAV dùng Git LFS (xem `.gitattributes`).

## Quy tắc đồng nhất asset (BẮT BUỘC)

Mọi asset đồ họa và âm thanh phải đi qua `asset-pipeline/`. Không tạo prompt tay, không chép ảnh từ chỗ khác vào `Assets/Art` hoặc `Assets/Audio`.

1. **Nguồn sự thật duy nhất**:
   - `asset-pipeline/bible/style.yaml`: phong cách, **bảng màu khóa cứng**, PPU, phong cách nhạc và giọng.
   - `asset-pipeline/bible/entities.yaml`: ngoại hình và giọng của từng nhân vật, mô tả từng vùng đất.
   - `asset-pipeline/manifest.yaml`: danh sách mọi asset.

   Muốn thêm asset thì thêm một mục vào manifest, không gọi API trực tiếp.
2. **Ảnh tham chiếu (anchor)** nằm trong `asset-pipeline/refs/`:
   - `_style.png` là style tile, được gửi kèm **mọi** lần tạo ảnh.
   - `<entity>.png` là tấm thiết kế nhân vật, được gửi kèm mọi asset của nhân vật đó. `hero_demon` dùng thêm anchor của `hero`.

   Thứ tự bắt buộc: style → anchor → các asset khác. CLI tự chặn nếu làm sai thứ tự.
3. **Đổi mô tả nhân vật hoặc đổi anchor** thì phải tạo lại các asset đã duyệt của nhân vật đó.
4. **Bảng màu**: mọi sprite được ép về đúng các màu trong `style.yaml > art.palette`. Màu đầu tiên luôn là màu outline. Thêm màu thì thêm vào palette rồi chạy `reprocess`. Không chỉnh tay PNG đầu ra.
5. **PPU = 32, tile = 32px**: phải khớp với `DevilBladeAssetImporter.cs` (`PixelsPerUnit`, `TileSize`). Không đổi khi đã có asset.
6. **Màu đỏ, cam, vàng lửa chỉ dùng cho yếu tố quỷ, máu và nộ khí.** Loài người và kiến trúc dùng tông thép lạnh và xám.
7. Mọi lần gọi API được ghi vào `asset-pipeline/logs/generations.jsonl` (prompt, model, hash ảnh tham chiếu). Không xóa file này.

## Lệnh pipeline

Chạy từ thư mục `asset-pipeline/`. Trên Windows, đặt `PYTHONIOENCODING=utf-8` trước khi chạy.

```
.venv\Scripts\python.exe -m pipeline status                  # asset nào đã duyệt / thiếu
.venv\Scripts\python.exe -m pipeline prompt <id>             # xem prompt + refs, không tốn tiền
.venv\Scripts\python.exe -m pipeline gen <id>... [-n 2] [--draft] [--yes]
.venv\Scripts\python.exe -m pipeline reprocess <id>          # chạy lại hậu kỳ trên ảnh gốc, không gọi API
.venv\Scripts\python.exe -m pipeline review <id>             # AI nghe & chấm music/voice
.venv\Scripts\python.exe -m pipeline approve <id> --pick N   # đưa ứng viên N vào refs/ hoặc Unity
.venv\Scripts\python.exe -m pipeline sfx --all               # SFX tổng hợp bằng code (miễn phí)
```

- Ứng viên nằm ở `asset-pipeline/out/candidates/<id>/<timestamp>/`, gồm `N_raw.png` (ảnh gốc), `N.png` (bản pixel) và `N_preview.png` (bản phóng to để xem).
- **Luôn xem ảnh preview trước khi approve.** Mô hình hay vẽ sai số frame hoặc sai bố cục. Pipeline tự xử lý được phần lớn trường hợp nhưng vẫn cần kiểm tra bằng mắt.
- Chi phí tham khảo: Gemini 3 Pro Image khoảng 0,13 USD/ảnh. Mỗi lệnh bị giới hạn 12 lần gọi API, muốn vượt phải thêm `--yes`.

## Unity

- `Assets/Editor/AssetImport/DevilBladeAssetImporter.cs` tự cấu hình mọi file trong `Assets/Art` và `Assets/Audio`:
  - Sprite: Point filter, không nén.
  - Sprite sheet: cắt theo file `*.sheet.json` đi kèm. GUID của sprite cố định, nên gen lại không làm mất tham chiếu.
  - Tileset: cắt theo lưới 32px.
  - Audio: Music dùng streaming, SFX giải nén khi load, Voice nén trong bộ nhớ.
- **Màn 1 được dựng hoàn toàn bằng code**: `Assets/Editor/LevelBuilder/Level1Builder.cs`.
  - Bố cục màn chơi (nền đất, platform, quái, checkpoint, boss) nằm trong phần `LEVEL DATA` của file này. Muốn sửa màn thì sửa ở đó rồi dựng lại.
  - Không sửa tay `Assets/Scenes/Level1.unity`, vì file sẽ bị ghi đè mỗi lần dựng.
  - Đơn vị: 1 ô = 1 unit = 32px. Nhảy cao tối đa khoảng 3,5 ô, nên chênh lệch độ cao giữa các bậc phải ≤ 3 ô.
- Gameplay nằm trong `Assets/Scripts/` (asmdef `DevilBlade.Runtime`):
  - `PlayerController`: di chuyển, chém, nộ khí, hóa quỷ.
  - `EnemyBase` / `ImpEnemy` / `HellhoundEnemy` / `DemonKnightBoss`.
  - `GameManager`, `HUD`, `AudioManager`.
  - Animation dùng `SpriteAnimator` đọc thẳng sprite sheet, không dùng AnimatorController.
- Test PlayMode ở `Assets/Tests/PlayMode/`, gồm cả bot chạy hết màn để kiểm tra màn đi qua được. `Level1CaptureTests` chụp ảnh lúc chơi vào `DevilBlade/Logs/Screens/`.
- Các method chạy headless:
  - `DevilBlade.EditorTools.Level1Builder.BuildHeadless`: dựng lại scene.
  - `DevilBlade.EditorTools.GameBuild.RebuildAndBuildHeadless`: dựng lại scene và build ra `DevilBlade/Builds/Windows/DevilBlade.exe`.
  - `DevilBlade.EditorTools.LevelScreenshots.CaptureHeadless`: chụp toàn màn.
  - Chạy test: `-runTests -testPlatform PlayMode -testResults <file.xml>`.
- Khi chạy Editor bằng PowerShell: dùng `Start-Process -PassThru` rồi `$p.WaitForExit()`. Đừng dùng `-Wait`, vì lệnh đó chờ cả tiến trình con (licensing client) nên sẽ treo rất lâu.
- Cài package qua `Assets/Editor/ProjectBootstrap/PackageInstaller.cs` (C# Client API). Không sửa tay `Packages/manifest.json`.
- Chạy headless: `"C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe" -batchmode -projectPath DevilBlade -executeMethod <Method> -logFile <log>`. Không truyền `-quit` khi gọi các method bất đồng bộ.

## Cấu hình Claude Code (`.claude/`)

- `settings.json`: quyền dùng chung. Lệnh pipeline đọc/hậu kỳ được tự cho phép; `gen`, `approve` (tốn tiền hoặc ghi đè asset) và `git push` luôn hỏi trước; cấm force push và xóa `generations.jsonl`.
- `hooks/guard-assets.js`: chặn sửa tay `Assets/Art|ArtHD|Audio`, scene `Level1*.unity`, `Packages/manifest.json`, `refs/*.png` và `generations.jsonl`. Cần Node.js.
- Skills: `/build-game`, `/run-tests`, `/new-asset`.
- Cấu hình riêng từng máy để ở `.claude/settings.local.json` (đã gitignore).
