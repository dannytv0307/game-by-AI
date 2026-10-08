---
name: new-asset
description: Quy trình thêm hoặc tạo lại asset đồ họa/âm thanh qua asset-pipeline (Vertex AI) cho DevilBlade mà vẫn giữ đồng nhất phong cách. Dùng khi người dùng muốn nhân vật, quái, tileset, nền, nhạc, giọng nói hoặc SFX mới.
---

# Thêm asset qua pipeline

Đọc phần "Quy tắc đồng nhất asset" trong `CLAUDE.md` trước. Không bao giờ gọi API trực tiếp hay chép ảnh vào `Assets/`.
Mọi lệnh chạy từ `asset-pipeline/` bằng `.venv/Scripts/python.exe -m pipeline ...`.

1. **Mô tả**: nhân vật/vùng đất mới thì thêm vào `bible/entities.yaml` (bản pixel và bản `_hd` nếu cần). Phong cách lấy từ `bible/style.yaml` (pixel) hoặc `bible/style_hd.yaml` (HD); không viết lại phong cách trong prompt.
2. **Manifest**: thêm mục vào `manifest.yaml` (id, type: anchor/sprite/anim/parts/tileset/background/music/voice, entity, style, kích thước/khung hình).
3. **Thứ tự**: style tile → anchor của entity → asset khác. Entity mới cần anchor được duyệt trước (CLI tự chặn nếu sai).
4. **Xem prompt miễn phí**: `pipeline prompt <id>`, kiểm tra prompt và danh sách ảnh tham chiếu.
5. **Tạo**: `pipeline gen <id> -n 2` (thêm `--draft` cho bản nháp rẻ). Báo trước chi phí ước tính (~0,13 USD/ảnh Pro, giới hạn 12 lần gọi/lệnh).
6. **Duyệt bằng mắt**: mở `out/candidates/<id>/<timestamp>/N_preview.png` (pixel) hoặc `N.png` (HD) bằng Read. Kiểm tra số frame, bố cục, màu (đỏ/cam/vàng chỉ cho yếu tố quỷ), viền nền xanh còn sót.
   - Nhạc/giọng: `pipeline review <id>` để AI nghe và chấm.
   - Sửa hậu kỳ không tốn tiền: `pipeline reprocess <id>`.
7. **Approve**: `pipeline approve <id> --pick N`, copy vào `refs/` (anchor) hoặc `DevilBlade/Assets/Art|ArtHD|Audio`.
8. **Gắn vào game**: sprite tự import theo `DevilBladeAssetImporter.cs`; tham chiếu trong `Level1Builder.cs` (Skin) rồi dựng lại scene, chạy skill `run-tests`.
9. SFX tổng hợp bằng code, miễn phí: `pipeline sfx --all`.

Đổi mô tả hoặc anchor của một nhân vật thì phải tạo lại mọi asset đã duyệt của nhân vật đó.
