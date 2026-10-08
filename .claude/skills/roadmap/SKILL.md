---
name: roadmap
description: Biến tài liệu cốt truyện docs/design/story.md và hiện trạng dự án thành roadmap theo mốc (docs/design/roadmap.md) và danh sách task chi tiết (docs/tasks/backlog.md) để làm lần lượt bằng /task. Dùng khi người dùng muốn lập kế hoạch, chia việc, hoặc cập nhật kế hoạch sau khi đổi cốt truyện.
argument-hint: "[phạm vi, ví dụ: 'chỉ màn 2' hoặc để trống để lập toàn bộ]"
---

# Lập roadmap và task

## Bước 1. Thu thập
- `docs/design/story.md` (bắt buộc; chưa có thì bảo người dùng chạy `/story` trước).
- `docs/design/characters/*.md`, `docs/design/zones/*.md` (concept, từ `/concept`). Nhân vật/vùng đất chưa có concept thì tạo task loại `story` "concept <id>" đứng trước task anchor của nó.
- Hiện trạng: `CLAUDE.md`, `Assets/Editor/LevelBuilder/Level1Builder.cs` (màn đã có), `Assets/Scripts/` (cơ chế đã có), `asset-pipeline/manifest.yaml` + `pipeline status` (asset đã có).
- `docs/design/roadmap.md`, `docs/tasks/backlog.md` nếu đã tồn tại: cập nhật, **không xóa task đã xong**, không đổi ID cũ.

## Bước 2. Hỏi người dùng những gì kế hoạch phụ thuộc vào (nếu chưa rõ)
- Ngân sách Vertex AI cho giai đoạn này (credit còn lại).
- Ưu tiên: chơi được hết cốt truyện thô trước (khuyên dùng) hay làm đẹp từng màn.

## Bước 3. `docs/design/roadmap.md`
Chia theo mốc nhỏ, mỗi mốc kết thúc bằng một bản build chơi được:
- M0 Nền tảng kỹ thuật cần cho nhiều màn (ví dụ: LevelBuilder tổng quát hóa cho nhiều màn, chuyển màn, save checkpoint, hệ thoại).
- M1, M2…: mỗi màn hoặc mỗi hồi.
- Mốc cuối: hoàn thiện (menu, âm thanh, cân bằng, build).
Mỗi mốc ghi: mục tiêu, tiêu chí xong, chi phí API ước tính (~0,13 USD/ảnh Pro, nhạc/giọng theo `config.yaml`).

## Bước 4. `docs/tasks/backlog.md`
Mỗi task đủ nhỏ để làm xong trong một lần `/task` (khoảng 1 file hệ thống hoặc 1 nhóm asset). Định dạng:

```markdown
## T-001 · <tiêu đề ngắn>
- Trạng thái: todo        <!-- todo | doing | done | blocked -->
- Mốc: M1 · Loại: gameplay | level | art | audio | ui | story | tech | test
- Phụ thuộc: T-000, ...
- Chi phí API: 0 | ~x USD
- Công cụ: <skill dự án và skill Unity plugin nên dùng, xem bảng trong /task>
- Việc cần làm: <các bước cụ thể, file sẽ sửa>
- Tiêu chí xong: <kiểm chứng được: test nào qua, ảnh chụp nào, build chạy>
- Ghi chú: <điền khi làm xong: commit, kết quả>
```

Quy tắc:
- Thứ tự: tech nền → concept → anchor/style → asset → level → gameplay → test. Danh sách tư thế/animation lấy từ mục "Chuyển động & đòn đánh" trong concept. Asset luôn qua `/new-asset` (anchor trước asset).
- Mỗi màn mới cần task test (mở rộng `Level1Tests` hoặc test mới có bot đi hết màn).
- Đánh số tiếp nối ID lớn nhất đã có.
- Đầu file có bảng tóm tắt: ID, tiêu đề, mốc, trạng thái, phụ thuộc.

## Bước 5. Kết thúc
Báo: số mốc, số task theo mốc, tổng chi phí API ước tính, task đầu tiên nên làm. Gợi ý `/task` để bắt đầu.
