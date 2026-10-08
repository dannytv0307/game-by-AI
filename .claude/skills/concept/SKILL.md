---
name: concept
description: Nhận art concept của người dùng (mô tả bằng lời và/hoặc ảnh phác thảo, ảnh tham khảo) cho một nhân vật, quái, boss hoặc vùng đất của DevilBlade; viết tài liệu concept docs/design/characters|zones/<id>.md và chuyển thành mô tả trong asset-pipeline/bible/entities.yaml. Dùng khi người dùng muốn mô tả ngoại hình, đưa ảnh concept, hoặc thiết kế lại một nhân vật.
argument-hint: "<entity id hoặc tên nhân vật/vùng đất>"
---

# Art concept cho nhân vật / vùng đất

Concept là cầu nối giữa cốt truyện (`docs/design/story.md`) và pipeline tạo ảnh. Luồng:
**lời mô tả + ảnh của người dùng → `docs/design/.../<id>.md` → `entities.yaml` → anchor → mọi asset khác.**

## Bước 1. Xác định đối tượng
- Lấy id từ `$ARGUMENTS`. Dùng id gốc không có hậu tố `_hd` (ví dụ `hero`, `imp`, `ruined_village`); thư mục concept `docs/concepts/<id>/` dùng chung cho entity `<id>_hd`.
- Đọc `docs/design/art_direction.md` (định hướng mỹ thuật chung; mọi concept phải theo). Nếu `$ARGUMENTS` là phong cách chung chứ không phải một id, cập nhật file này thay vì tạo concept nhân vật.
- Đọc mô tả hiện có trong `asset-pipeline/bible/entities.yaml` (cả `<id>` và `<id>_hd`), phần liên quan trong `docs/design/story.md`, và `docs/design/characters/<id>.md` nếu đã có.

## Bước 2. Thu thập concept
- **Ảnh**: người dùng bỏ ảnh vào `docs/concepts/<id>/` (png/jpg/webp; ảnh vẽ tay, phác thảo, ảnh tham khảo đều được), hoặc đưa đường dẫn để mình chép vào đó. Xem từng ảnh bằng Read và mô tả lại những gì thấy để người dùng xác nhận mình hiểu đúng.
  - Pipeline tự gửi tối đa 3 ảnh đầu (theo tên file) kèm lệnh tạo **anchor** (và style tile với `_style/`, nền với thư mục vùng đất). Đặt tên `01_...`, `02_...` để chọn ảnh quan trọng nhất.
  - Ảnh chỉ định hướng thiết kế; phong cách vẽ vẫn theo `style.yaml` / `style_hd.yaml`.
- **Lời mô tả**: hỏi (AskUserQuestion, có lựa chọn gợi ý) những gì còn thiếu:
  1. Vai trò trong cốt truyện và tính cách (ảnh hưởng dáng đứng, nét mặt).
  2. Hình khối (silhouette): to/nhỏ, gầy/vạm vỡ, điểm nhận diện từ xa.
  3. Trang phục, giáp, vũ khí, chi tiết đặc trưng.
  4. Màu chủ đạo, kiểm tra quy tắc: đỏ/cam/vàng lửa chỉ cho yếu tố quỷ, máu, nộ khí; người và kiến trúc dùng thép lạnh, xám.
  5. Cách di chuyển và tấn công (quyết định animation và tư thế cần tạo).
  6. Giọng nói (nếu có thoại).
  Vùng đất thì hỏi: địa hình, kiến trúc, thời tiết/ánh sáng, mối nguy, không khí.

## Bước 3. Viết tài liệu concept
`docs/design/characters/<id>.md` (nhân vật, quái, boss) hoặc `docs/design/zones/<id>.md` (vùng đất):

```markdown
# <Tên> (`<id>`)
- Loại: nhân vật chính | đồng minh | quái | boss | vùng đất · Xuất hiện: màn ...
## Vai trò & tính cách
## Hình khối & tỉ lệ (chiều cao so với Kael)
## Ngoại hình chi tiết (đầu, thân, trang phục, vũ khí, dấu hiệu quỷ)
## Màu chủ đạo
## Chuyển động & đòn đánh  → danh sách tư thế/animation cần tạo
## Giọng nói
## Ảnh concept: docs/concepts/<id>/ (liệt kê và ghi mỗi ảnh lấy ý gì)
## Mô tả cho AI (tiếng Anh, 60–120 từ, chỉ ngoại hình, không phong cách vẽ)
```

## Bước 4. Đưa vào pipeline
- Đề xuất đoạn `description` (lấy từ "Mô tả cho AI") cho `entities.yaml`, cả bản `<id>` và `<id>_hd` nếu nhân vật có hai bản. Thực thể mới thì thêm đầy đủ (`name`, `kind`, `height_px`, `voice` nếu có) và thêm mục anchor vào `manifest.yaml`.
- **Nếu nhân vật đã có anchor được duyệt**: đổi mô tả hoặc thêm ảnh concept nghĩa là phải tạo lại anchor và mọi asset đã duyệt của nhân vật đó (tốn tiền). Báo rõ danh sách asset bị ảnh hưởng và chi phí ước tính, chỉ sửa khi người dùng đồng ý.
- Kiểm tra bằng `pipeline prompt <id>_anchor` (miễn phí): prompt có mô tả mới và danh sách REFS có ảnh concept.
- Không tự chạy `gen`. Gợi ý bước tiếp: `/new-asset` để tạo anchor, hoặc thêm task vào `docs/tasks/backlog.md` nếu đang làm theo kế hoạch.
