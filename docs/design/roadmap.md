# DevilBlade — Roadmap

> Lập ngày 2026-10-08 từ [story.md](story.md), [art_direction.md](art_direction.md), [characters/hero.md](characters/hero.md).
> Task chi tiết: [docs/tasks/backlog.md](../tasks/backlog.md). Làm bằng `/task`.

## Quyết định nền
| Chủ đề | Quyết định |
|---|---|
| Đồ họa | **Chỉ HD** (dark fantasy × gothic × kinh dị tâm lý × yêu quái Nhật). Bản Pixel gỡ khỏi game; asset pixel giữ trong repo để tham khảo. |
| Ngân sách Vertex AI | **~200 USD** cho 6 màn. Phần tăng thêm dùng cho: animation Veo cho Kael, **mọi boss và quái thường**; thẻ truyện nhiều hơn (mỗi màn 3–4 thẻ, cảnh kết thúc đầy đủ); nền parallax nhiều lớp hơn; thoại lồng tiếng đầy đủ; dư ~30 USD để làm lại. |
| Thứ tự | **Làm đẹp từng màn**: hoàn thiện màn 1 rồi mới sang màn 2. Phần kỹ thuật dùng chung (M0) làm trước để không phải sửa lại. |
| Trang bị | **Chỉ hiệu ứng** trên người Kael (màu lửa, vệt sáng, ánh giáp); hình trang bị hiện ở màn hình trang bị. Không vẽ lại tư thế theo trang bị. |
| Animation | Thử **Veo (ảnh → video → khung hình)** ở M0. Nếu đạt: dùng cho Kael, boss và quái thường. Nếu không đạt: tư thế tĩnh + chuyển động thủ tục, và chuyển phần ngân sách Veo sang thêm tư thế + thẻ truyện. |

## Các mốc
Mỗi mốc kết thúc bằng một bản build Windows chơi được và test PlayMode qua hết.

| Mốc | Mục tiêu | Tiêu chí xong | Chi phí API ước tính |
|---|---|---|---|
| **M0 · Nền tảng** | Khung kỹ thuật cho 6 màn: bỏ Pixel, builder đa màn, luồng màn + lưu tiến trình, thẻ truyện, thoại kịch bản, trang bị, lướt, ánh sáng tối, thử Veo | Màn thử nghiệm hiện tại chạy trên khung mới; chuyển màn, lưu/tải, thẻ truyện, trang bị hoạt động; quyết định xong hướng animation | ~6 USD |
| **M1 · Thành Cuối (Valgrave)** | Màn 1 hoàn chỉnh, đẹp: Kael bản mới đầy đủ, gaki, inugami, Kỵ sĩ Quỷ; Aldous, Morvain; bộc phát theo kịch bản; trục xuất | Chơi trọn màn 1 + thẻ mở đầu + thẻ sau màn + nhận Vỏ kiếm Phong Ấn; ảnh chụp duyệt | ~40 USD |
| **M2 · Tro Ashveil** | Thanh Kiểm soát, vật phẩm ẩn, màn 2, boss Mẹ Bầy, Găng Xương Bầy | Như M1 cho màn 2; Kiểm soát + di vật + mảnh thép hoạt động | ~22 USD |
| **M3 · Rừng Thối** | Màn 3, sương độc, boss Kẻ Nhả Độc, Nanh Độc | Như trên | ~22 USD |
| **M4 · Hắc Diệm** | Màn 4, dung nham dâng, quỷ bỏ chạy, boss Thống lĩnh, Giáp Hắc Diệm | Như trên | ~22 USD |
| **M5 · Cánh cổng** | Màn 5, hệ lựa chọn tha/giết, boss Morvain (con tin, giả chết), Mặt nạ Kim Ấn | Như trên; lựa chọn được lưu và ảnh hưởng Kiểm soát | ~24 USD |
| **M6 · Lò Nguồn** | Màn 6, boss Quỷ vương 2 giai đoạn, 2 kết thúc × 2 biến thể | Chơi hết game từ màn 1 đến cả 4 biến thể kết thúc | ~30 USD |
| **M7 · Hoàn thiện** | Menu, cài đặt, mixer âm thanh, sprite atlas, cân bằng, build phát hành | Bot chơi xuyên 6 màn không lỗi; build phát hành | ~3 USD |
| | | **Tổng** | **~170 USD** (dư ~30 USD làm lại) |

Ghi chú chi phí: ảnh Gemini 3 Pro ~0,13 USD/ảnh (2K); nhạc Lyria và giọng TTS rất rẻ; **giá Veo cần xác nhận ở T-011** — nếu đắt hơn dự kiến, quái thường quay về tư thế tĩnh trước, boss và Kael giữ Veo.

## Cách làm từng màn (M1–M6)
1. **Concept** (`/concept`): vùng đất, quái, boss, NPC mới. Miễn phí.
2. **Asset nhân vật**: anchor → tư thế → (Veo nếu dùng). Luôn xem ảnh trước khi approve.
3. **Asset môi trường**: nền parallax, tileset, đạo cụ, thẻ truyện.
4. **Âm thanh**: nhạc màn, nhạc boss, thoại, SFX.
5. **Dựng màn** (LevelDefinition), **kịch bản** (thoại, sự kiện), **boss + trang bị**.
6. **Test + ảnh chụp + build**.

M2–M6 trong backlog mới chia ở mức khung; khi bắt đầu mỗi mốc, chạy lại `/roadmap` để tách nhỏ theo concept đã chốt.
