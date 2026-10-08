---
name: story
description: Cùng người dùng lên ý tưởng cốt truyện, thế giới, nhân vật và cấu trúc màn chơi cho DevilBlade, ghi thành tài liệu thiết kế docs/design/story.md. Dùng khi người dùng muốn bàn cốt truyện, lore, nhân vật mới, hoặc sửa hướng câu chuyện.
argument-hint: "[chủ đề cần bàn, ví dụ: 'phản diện chính' hoặc để trống để làm từ đầu]"
---

# Lên ý tưởng cốt truyện

Mục tiêu: một tài liệu cốt truyện đủ rõ để `/roadmap` biến thành màn chơi, nhân vật, asset và task. Không viết code, không tạo asset ở bước này.

## Bước 1. Đọc bối cảnh hiện có
- `CLAUDE.md` (tiền đề: Kael, dòng máu quỷ, hóa quỷ khi đầy nộ khí).
- `asset-pipeline/bible/entities.yaml` (nhân vật và vùng đất đã có), `bible/style_hd.yaml` (tông u tối).
- `docs/design/story.md` nếu đã tồn tại. Có rồi thì chỉ bàn phần người dùng nêu trong `$ARGUMENTS`, giữ nguyên phần đã chốt.

## Bước 2. Hỏi người dùng (AskUserQuestion, mỗi lượt 1–4 câu, có lựa chọn gợi ý)
Lần lượt chốt, không hỏi dồn tất cả một lúc:
1. **Giọng điệu và quy mô**: u tối bi kịch / sử thi anh hùng / kinh dị; số màn dự kiến (gợi ý 3–5 cho prototype); độ dài chơi.
2. **Kael**: nguồn gốc dòng máu quỷ, động lực, nỗi sợ, cái giá của việc hóa quỷ (gợi ý: mỗi lần hóa quỷ mất dần nhân tính → gắn với cơ chế game).
3. **Phản diện và thế lực quỷ**: kẻ đứng sau, các tầng địa ngục/vùng đất, boss mỗi màn.
4. **Cấu trúc**: các hồi (act), mỗi màn = 1 vùng đất + 1 boss + 1 cơ chế mới; kết thúc (một hay nhiều kết).
5. **Cách kể chuyện trong game**: thoại lồng tiếng ngắn, văn bản giữa màn, môi trường kể chuyện. Giữ ít chữ cho prototype.

Với mỗi câu, đề xuất 2–3 hướng cụ thể kèm hướng mình khuyên dùng, để người dùng chọn nhanh.

## Bước 3. Ghi `docs/design/story.md`
Dùng cấu trúc:

```markdown
# DevilBlade — Cốt truyện
## Tóm tắt một câu
## Giọng điệu & cảm hứng
## Thế giới (lịch sử, luật của dòng máu quỷ, các vùng đất)
## Nhân vật (Kael, đồng minh, phản diện, boss) — vai trò + 1–2 câu ngoại hình; chi tiết nằm ở docs/design/characters/<id>.md (/concept)
## Cấu trúc màn chơi
| Màn | Vùng đất | Sự kiện cốt truyện | Cơ chế mới | Quái mới | Boss |
## Cơ chế gắn với cốt truyện (nộ khí, hóa quỷ, cái giá...)
## Thoại / văn bản then chốt (ngắn)
## Kết thúc
## Câu hỏi còn mở
```

Màn 1 (làng đổ nát → Demon Knight) đã có trong game: giữ làm màn mở đầu trừ khi người dùng muốn đổi.

## Bước 4. Kết thúc
- Tóm tắt những gì đã chốt và các câu hỏi còn mở.
- Liệt kê nhân vật/vùng đất mới hoặc cần thiết kế lại (chỉ đề xuất, chưa sửa `entities.yaml`: đổi mô tả nhân vật cũ sẽ buộc phải tạo lại asset của nhân vật đó).
- Gợi ý bước tiếp: `/concept <id>` cho từng nhân vật/vùng đất (người dùng có thể bỏ ảnh concept vào `docs/concepts/<id>/`), sau đó `/roadmap`.
