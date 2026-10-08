---
name: task
description: Thực hiện một task trong docs/tasks/backlog.md của DevilBlade từ đầu đến cuối (làm, kiểm thử, cập nhật trạng thái, commit). Dùng khi người dùng gõ /task, /task T-012, /task status hoặc bảo làm task tiếp theo.
argument-hint: "[T-xxx | next | status]"
---

# Làm task

## Chọn task
- `$ARGUMENTS` = `status`: in bảng tóm tắt (đếm todo/doing/done/blocked theo mốc, task kế tiếp làm được) rồi dừng.
- `$ARGUMENTS` = `T-xxx`: làm đúng task đó. Phụ thuộc chưa xong thì báo và hỏi có làm phụ thuộc trước không.
- Trống hoặc `next`: chọn task `doing` còn dở, nếu không có thì task `todo` đầu tiên có mọi phụ thuộc đã `done`, ưu tiên mốc nhỏ nhất.
- Chưa có `docs/tasks/backlog.md` thì bảo người dùng chạy `/story` rồi `/roadmap`.

**Mỗi lần chỉ làm một task.**

## Thực hiện
1. Đổi trạng thái task thành `doing` trong backlog (cả bảng tóm tắt).
2. Đọc phần liên quan trong `docs/design/story.md` để làm đúng tinh thần cốt truyện.
3. Task có chi phí API: báo ước tính và xin xác nhận trước khi `pipeline gen`.
4. Làm theo "Việc cần làm", dùng đúng công cụ:

| Việc | Dùng |
|---|---|
| Asset hình/âm thanh | skill dự án `new-asset` (bắt buộc, không tạo asset cách khác) |
| Sửa/thêm màn | sửa LEVEL DATA / Skin trong `Level1Builder.cs` (hoặc builder màn mới cùng kiểu), không sửa tay `.unity` |
| Build, test | skill dự án `build-game`, `run-tests` |
| Cài package Unity | `unity:unity-package-management` |
| Pixel-perfect, camera pixel | `unity:2d-pixel-perfect` |
| Cắt/pivot sprite | `unity:sprite-editor` (nhưng sprite sheet của pipeline đã tự cắt qua `.sheet.json`) |
| Tilemap, Rule Tile, palette | `unity:tilemap-palette-create`, `unity:tilemap-ruletile-createfromsegment`, `unity:sprite-segment-3x3grid` |
| Menu, HUD, hộp thoại | `unity:ui-ugui` (dự án đang dùng uGUI) |
| Bloom, vignette, màu sắc | `unity:urp-postprocessing` |
| Mixer, tối ưu âm thanh | `unity:audio-setup-mixers`, `unity:optimize-audio` |
| Gom sprite giảm draw call | `unity:manage-sprite-atlas` |
| Đa ngôn ngữ | `unity:localization` |
| Lệnh Unity CLI, Editor headless | `unity:unity-cli` |

   Chỉ gọi skill Unity khi task thật sự cần; làm theo hướng dẫn trong skill nhưng ưu tiên quy ước của `CLAUDE.md` khi khác nhau (ví dụ: chạy Editor bằng `Start-Process -PassThru`, scene dựng bằng code).
5. Kiểm chứng đúng "Tiêu chí xong": chạy `run-tests` (thêm/sửa test nếu task thêm cơ chế hoặc màn), xem ảnh chụp khi task có phần hình ảnh. Test hỏng thì sửa trước khi đánh dấu xong.

## Kết thúc
1. Cập nhật task: `done` + "Ghi chú" (kết quả, chi phí API thực tế, hạn chế còn lại). Làm không xong thì `blocked` + lý do.
2. Phát sinh việc mới thì thêm task mới (ID tiếp theo) thay vì làm luôn.
3. Commit một commit cho task: `T-xxx: <tiêu đề>`. Không push trừ khi người dùng bảo.
4. Báo người dùng: đã làm gì, kiểm chứng ra sao, chi phí, task kế tiếp đề xuất.
