# Art concept

Bỏ ảnh concept (vẽ tay, phác thảo, ảnh tham khảo; png/jpg/webp) vào thư mục con theo id:

| Thư mục | Dùng khi tạo |
|---|---|
| `docs/concepts/<entity>/` (vd `hero/`, `imp/`) | anchor của nhân vật đó, cả bản Pixel và HD (`hero_hd` dùng chung `hero/`) |
| `docs/concepts/<zone>/` (vd `ruined_village/`) | ảnh nền của vùng đất |
| `docs/concepts/_style/` | style tile |

- Pipeline gửi tối đa 3 ảnh đầu tiên theo tên file. Đặt tên `01_...`, `02_...` để chọn thứ tự.
- Ảnh chỉ định hướng thiết kế (hình khối, trang phục, chi tiết). Phong cách vẽ vẫn theo `asset-pipeline/bible/style*.yaml`.
- Kèm lời mô tả: chạy `/concept <id>` để Claude viết `docs/design/characters/<id>.md` và cập nhật `entities.yaml`.
- Thêm hoặc đổi ảnh ở đây sau khi anchor đã duyệt thì phải tạo lại anchor và asset của nhân vật đó.
