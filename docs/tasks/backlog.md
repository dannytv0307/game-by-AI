# DevilBlade — Backlog

Làm bằng `/task` (task kế tiếp), `/task T-xxx`, `/task status`. Kế hoạch tổng: [roadmap.md](../design/roadmap.md).
Trạng thái: `todo` · `doing` · `done` · `blocked`.

## Tóm tắt

| ID | Tiêu đề | Mốc | Trạng thái | Phụ thuộc |
|---|---|---|---|---|
| T-001 | Gỡ bản Pixel, chỉ còn HD | M0 | done | — |
| T-002 | LevelDefinition + builder đa màn | M0 | todo | T-001 |
| T-003 | Luồng màn chơi + lưu tiến trình | M0 | todo | T-002 |
| T-004 | Hệ thẻ truyện | M0 | todo | T-003 |
| T-005 | Thoại trong màn + sự kiện kịch bản | M0 | todo | T-002 |
| T-006 | Hệ trang bị + màn hình trang bị | M0 | todo | T-003 |
| T-007 | Lướt (dash) cho Kael | M0 | todo | — |
| T-008 | Ánh sáng 2D + hậu kỳ cho tông tối | M0 | todo | T-001 |
| T-009 | Pipeline: sửa dải khung bị vũ khí tràn | M0 | todo | — |
| T-010 | Test đa màn theo LevelDefinition | M0 | todo | T-002 |
| T-011 | Thử animation bằng Veo (spike) | M0 | todo | T-009 |
| T-012 | Concept vùng Valgrave | M1 | todo | — |
| T-013 | Concept gaki (thay Imp) | M1 | todo | — |
| T-014 | Concept inugami (thay Hellhound) | M1 | todo | — |
| T-015 | Concept Kỵ sĩ Quỷ (ochimusha) | M1 | todo | — |
| T-016 | Concept Aldous | M1 | todo | — |
| T-017 | Concept Morvain | M1 | todo | — |
| T-018 | Kael: bộ tư thế HD đầy đủ | M1 | todo | T-009 |
| T-019 | Kael: animation chính (Veo hoặc thủ tục) | M1 | todo | T-011, T-018 |
| T-020 | Gaki + inugami: anchor + tư thế | M1 | todo | T-013, T-014 |
| T-021 | Kỵ sĩ Quỷ: anchor + tư thế + animation | M1 | todo | T-015, T-011 |
| T-022 | Aldous + Morvain: anchor + tư thế NPC | M1 | todo | T-016, T-017 |
| T-023 | Valgrave: nền parallax, tileset, đạo cụ | M1 | todo | T-012 |
| T-024 | Thẻ truyện mở đầu + sau màn 1 | M1 | todo | T-004, T-023 |
| T-025 | Âm thanh màn 1 | M1 | todo | T-016, T-017 |
| T-026 | Dựng màn 1 Valgrave | M1 | todo | T-002, T-023 |
| T-027 | Kịch bản màn 1 | M1 | todo | T-005, T-022, T-025, T-026 |
| T-028 | Boss Kỵ sĩ Quỷ + bộc phát bắt buộc | M1 | todo | T-021, T-026 |
| T-029 | Hoàn thiện M1: cân chỉnh, test, build | M1 | todo | T-019…T-028 |
| T-030 | Thanh Kiểm soát | M2 | todo | T-006 |
| T-031 | Vật phẩm ẩn: di vật Ysolde, mảnh thép quỷ | M2 | todo | T-003 |
| T-032 | Concept màn 2 (Ashveil, Mẹ Bầy, quỷ non, lính đánh thuê) | M2 | todo | — |
| T-033 | Asset nhân vật màn 2 | M2 | todo | T-032 |
| T-034 | Asset môi trường + thẻ truyện + âm thanh màn 2 | M2 | todo | T-032 |
| T-035 | Dựng màn 2 + nhà sập + kịch bản | M2 | todo | T-033, T-034 |
| T-036 | Boss Mẹ Bầy + Găng Xương Bầy | M2 | todo | T-035 |
| T-037 | Hoàn thiện M2 | M2 | todo | T-030, T-031, T-036 |
| T-038 | Concept màn 3 (Rừng Thối, Kẻ Nhả Độc, quỷ ăn thịt, nhện quỷ, ẩn sĩ) | M3 | todo | — |
| T-039 | Asset nhân vật màn 3 | M3 | todo | T-038 |
| T-040 | Asset môi trường + thẻ truyện + âm thanh màn 3 | M3 | todo | T-038 |
| T-041 | Dựng màn 3 + sương độc + kịch bản ẩn sĩ | M3 | todo | T-039, T-040 |
| T-042 | Boss Kẻ Nhả Độc + Nanh Độc | M3 | todo | T-041 |
| T-043 | Hoàn thiện M3 | M3 | todo | T-042 |
| T-044 | Concept màn 4 (Hắc Diệm, Thống lĩnh, quỷ dung nham, quỷ bay) | M4 | todo | — |
| T-045 | Asset nhân vật màn 4 | M4 | todo | T-044 |
| T-046 | Asset môi trường + thẻ truyện + âm thanh màn 4 | M4 | todo | T-044 |
| T-047 | Dựng màn 4 + dung nham dâng + quỷ bỏ chạy | M4 | todo | T-045, T-046 |
| T-048 | Boss Thống lĩnh + Giáp Hắc Diệm | M4 | todo | T-047 |
| T-049 | Hoàn thiện M4 | M4 | todo | T-048 |
| T-050 | Hệ lựa chọn tha/giết + chọn kết thúc | M5 | todo | T-030 |
| T-051 | Concept màn 5 (Cánh cổng, Morvain dạng boss, lính Hội, quỷ khế ước) | M5 | todo | — |
| T-052 | Asset nhân vật màn 5 | M5 | todo | T-051 |
| T-053 | Asset môi trường + thẻ truyện + âm thanh màn 5 | M5 | todo | T-051 |
| T-054 | Dựng màn 5 + cổng phản ứng theo nộ khí | M5 | todo | T-052, T-053 |
| T-055 | Boss Morvain + lựa chọn + Mặt nạ Kim Ấn | M5 | todo | T-050, T-054 |
| T-056 | Hoàn thiện M5 | M5 | todo | T-055 |
| T-057 | Concept màn 6 (Lò Nguồn, Quỷ vương, hộ vệ ngai) | M6 | todo | — |
| T-058 | Asset nhân vật màn 6 | M6 | todo | T-057 |
| T-059 | Asset môi trường + thẻ truyện + âm thanh màn 6 | M6 | todo | T-057 |
| T-060 | Dựng màn 6 | M6 | todo | T-058, T-059 |
| T-061 | Boss Quỷ vương 2 giai đoạn | M6 | todo | T-060 |
| T-062 | 2 kết thúc × 2 biến thể | M6 | todo | T-050, T-061 |
| T-063 | Hoàn thiện M6 | M6 | todo | T-062 |
| T-064 | Menu chính, tạm dừng, cài đặt | M7 | todo | T-003 |
| T-065 | Mixer + tối ưu âm thanh | M7 | todo | T-063 |
| T-066 | Sprite atlas + hiệu năng | M7 | todo | T-063 |
| T-067 | Cân bằng toàn game | M7 | todo | T-063 |
| T-068 | Build phát hành | M7 | todo | T-064…T-067 |

---

## M0 · Nền tảng

## T-001 · Gỡ bản Pixel, chỉ còn HD
- Trạng thái: done
- Mốc: M0 · Loại: tech
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `build-game`, `run-tests`
- Việc cần làm: theo yêu cầu người dùng, gỡ **toàn bộ** ảnh pixel (không giữ lại trong repo): skin Pixel, phím Tab, fixture test, scene cũ, `Assets/Art`, tile pixel, mục pixel trong manifest/entities, ảnh tham chiếu pixel; cập nhật `CLAUDE.md`, skill, hook.
- Tiêu chí xong: build chỉ có `Level1` (HD); test PlayMode qua; `CLAUDE.md` cập nhật.
- Ghi chú: xong 2026-10-08. Scene HD đổi tên thành `Assets/Scenes/Level1.unity` (bỏ hậu tố _HD). Đã xóa `Assets/Art` (sprite, tileset, nền pixel), `Assets/Tiles/Village`, 6 ảnh tham chiếu pixel trong `refs/`, 21 mục ảnh pixel trong manifest và 5 entity pixel. Giọng thoại chuyển sang entity HD (`hero_hd`, `hero_demon_hd`, `demon_knight_hd`). Pipeline mặc định `style: hd`. Ảnh trắng cho HUD chuyển sang `Assets/ArtHD/UI/white.png`. Test 8/8 qua, build Windows 112 MB, 0 lỗi. Chi phí API: 0.
  Còn lại: code xử lý pixel trong `asset-pipeline/pipeline/pixel.py` và phần `art` của `style.yaml` vẫn còn (HD dùng chung vài hàm đọc ảnh); không ảnh hưởng gì. Ảnh pixel cũ xem lại được trong lịch sử git.

## T-002 · LevelDefinition + builder đa màn
- Trạng thái: todo
- Mốc: M0 · Loại: tech
- Phụ thuộc: T-001
- Chi phí API: 0
- Công cụ: `build-game`; `unity:tilemap-ruletile-createfromsegment` nếu chuyển tileset sang Rule Tile
- Việc cần làm: tách phần LEVEL DATA khỏi `Level1Builder.cs` thành định nghĩa màn (class C# `LevelDefinition`: vùng đất/tileset/nền, đoạn nền, platform, quái, checkpoint, boss, trigger kịch bản, vật phẩm). Một `LevelBuilder` dựng `Assets/Scenes/Levels/Level0N.unity` cho từng định nghĩa. Màn hiện tại chuyển thành `Level00_Test` để giữ test. Cập nhật `CLAUDE.md` (cách thêm màn).
- Tiêu chí xong: `LevelBuilder.BuildAll` dựng được màn thử; test bot đi hết màn qua.
- Ghi chú:

## T-003 · Luồng màn chơi + lưu tiến trình
- Trạng thái: todo
- Mốc: M0 · Loại: tech
- Phụ thuộc: T-002
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: `GameFlow` (singleton không hủy): thứ tự màn, hạ boss → thẻ truyện → màn hình trang bị → màn kế. `SaveData` JSON trong `Application.persistentDataPath`: màn hiện tại, trang bị, Kiểm soát, vật phẩm đã nhặt, lựa chọn. Tiếp tục / chơi mới.
- Tiêu chí xong: test PlayMode: thắng màn → sang màn kế; thoát và tải lại giữ đúng dữ liệu.
- Ghi chú:

## T-004 · Hệ thẻ truyện
- Trạng thái: todo
- Mốc: M0 · Loại: ui
- Phụ thuộc: T-003
- Chi phí API: 0
- Công cụ: `unity:ui-ugui`
- Việc cần làm: scene/overlay `StoryCard`: ảnh minh họa (pan chậm), 2–3 dòng chữ hiện dần, giọng đọc tùy chọn, bỏ qua bằng phím. Dữ liệu thẻ dạng ScriptableObject hoặc JSON. Dùng ảnh tạm cho đến T-024.
- Tiêu chí xong: test hiển thị thẻ và chuyển tiếp; ảnh chụp thẻ.
- Ghi chú:

## T-005 · Thoại trong màn + sự kiện kịch bản
- Trạng thái: todo
- Mốc: M0 · Loại: tech
- Phụ thuộc: T-002
- Chi phí API: 0
- Công cụ: `unity:ui-ugui`
- Việc cần làm: `ScriptedEvent` kích hoạt bằng vùng trigger: khóa điều khiển, di chuyển camera, phụ đề + giọng, NPC nói, chờ, gọi hành động (ví dụ ép hóa quỷ). Khai báo trong `LevelDefinition`.
- Tiêu chí xong: test: đi vào trigger → phụ đề hiện, điều khiển khóa rồi mở lại.
- Ghi chú:

## T-006 · Hệ trang bị + màn hình trang bị
- Trạng thái: todo
- Mốc: M0 · Loại: gameplay
- Phụ thuộc: T-003
- Chi phí API: 0
- Công cụ: `unity:ui-ugui`
- Việc cần làm: dữ liệu trang bị (ô, chỉ số, hiệu ứng) cho 5 món trong [story.md](../design/story.md). `PlayerStats` áp hệ số sát thương, giảm sát thương, độc, vệt lửa, nộ khí. Hiệu ứng trên người Kael: màu lửa, vệt sáng (không đổi sprite). Màn hình trang bị giữa các màn (icon tạm).
- Tiêu chí xong: test đơn vị cho từng hiệu ứng; ảnh chụp màn hình trang bị.
- Ghi chú:

## T-007 · Lướt (dash) cho Kael
- Trạng thái: todo
- Mốc: M0 · Loại: gameplay
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: thêm lướt ngắn (phím L hoặc Shift) vào `PlayerController`: khoảng cách, thời gian hồi, miễn sát thương ngắn, lướt trên không 1 lần. API `SimulateDash` cho test. Tư thế tạm: chạy.
- Tiêu chí xong: test lướt đi đúng khoảng cách và né được đòn.
- Ghi chú:

## T-008 · Ánh sáng 2D + hậu kỳ cho tông tối
- Trạng thái: todo
- Mốc: M0 · Loại: tech
- Phụ thuộc: T-001
- Chi phí API: 0
- Công cụ: `unity:urp-postprocessing`
- Việc cần làm: Global Light 2D tối ánh tím; Light 2D cho lửa, đèn đá, mắt quỷ; Volume: bloom nhẹ cho lửa, vignette, color adjustments ngả chàm. Sprite dùng material Lit.
- Tiêu chí xong: ảnh chụp màn thử khớp tinh thần [art_direction.md](../design/art_direction.md); FPS không giảm đáng kể.
- Ghi chú:

## T-009 · Pipeline: sửa dải khung bị vũ khí tràn
- Trạng thái: todo
- Mốc: M0 · Loại: tech
- Phụ thuộc: —
- Chi phí API: ~0,5 USD (thử lại)
- Công cụ: `new-asset`
- Việc cần làm: dải khung của Kael bị nodachi chồng sang khung bên. Sửa prompt dải khung (khoảng cách rộng, vũ khí nằm trọn trong khung) và/hoặc tách khung theo lưới cứng; thêm "chỉ một thanh nodachi" vào mô tả `hero_hd`.
- Tiêu chí xong: tạo lại `hero_hd_idle`, mỗi khung sạch, không mẩu kiếm thừa.
- Ghi chú:

## T-010 · Test đa màn theo LevelDefinition
- Trạng thái: todo
- Mốc: M0 · Loại: test
- Phụ thuộc: T-002
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: test fixture tự sinh theo danh sách màn; bot đi hết màn tới đấu trường boss; test chụp ảnh từng màn.
- Tiêu chí xong: thêm một màn mới vào danh sách là có test tương ứng, không phải viết thêm.
- Ghi chú:

## T-011 · Thử animation bằng Veo (spike)
- Trạng thái: todo
- Mốc: M0 · Loại: tech
- Phụ thuộc: T-009
- Chi phí API: ~5 USD
- Công cụ: `new-asset`
- Việc cần làm: xác nhận model Veo và giá trên Vertex. Thêm loại asset `video_anim` vào pipeline: ảnh tư thế → video ngắn (nền xanh, máy quay cố định) → lấy 8–12 khung → tách nền → sprite sheet. Thử với Kael chém và Kael chạy. Ghi provenance như ảnh.
- Tiêu chí xong: so sánh trong game (ảnh chụp) Veo với tư thế tĩnh; chốt hướng animation cho M1–M6 và cập nhật roadmap.
- Ghi chú:

---

## M1 · Thành Cuối (Valgrave)

## T-012 · Concept vùng Valgrave
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept valgrave`
- Việc cần làm: `docs/design/zones/valgrave.md`: khu ổ chuột, tường thành, khu quý tộc, quảng trường đấu boss; lâu đài Nhật trên nền nhà thờ gothic; đèn lồng giấy. Thêm zone vào `entities.yaml`.
- Tiêu chí xong: tài liệu + mô tả AI được duyệt.
- Ghi chú:

## T-013 · Concept gaki (thay Imp)
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept imp`
- Việc cần làm: ngạ quỷ gầy, bụng phình, miệng khâu chỉ; cách di chuyển và đòn đánh.
- Tiêu chí xong: `docs/design/characters/imp.md` + mô tả `imp_hd` được duyệt.
- Ghi chú:

## T-014 · Concept inugami (thay Hellhound)
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept hellhound`
- Việc cần làm: chó quỷ không da, đầu treo bùa, chân dài bất thường; lao cắn.
- Tiêu chí xong: tài liệu + mô tả `hellhound_hd` được duyệt.
- Ghi chú:

## T-015 · Concept Kỵ sĩ Quỷ (ochimusha)
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept demon_knight`
- Việc cần làm: samurai sa đọa, giáp ō-yoroi lẫn giáp gothic, menpō, sashimono rách; bộ chiêu boss 2 giai đoạn.
- Tiêu chí xong: tài liệu + mô tả `demon_knight_hd` được duyệt.
- Ghi chú:

## T-016 · Concept Aldous
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept aldous`
- Việc cần làm: trưởng lão dòng Tro Trắng (nón rơm komusō, áo tu sĩ tro trắng); giọng.
- Tiêu chí xong: tài liệu + entity `aldous_hd` mới.
- Ghi chú:

## T-017 · Concept Morvain
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept morvain`
- Việc cần làm: dạng người (màn 1: quý tộc lịch lãm, mặt nạ Noh vàng khi ở Hội) và gợi ý dạng boss (màn 5); giọng.
- Tiêu chí xong: tài liệu + entity `morvain_hd` mới.
- Ghi chú:

## T-018 · Kael: bộ tư thế HD đầy đủ
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-009
- Chi phí API: ~3 USD
- Công cụ: `new-asset`
- Việc cần làm: theo mục "Chuyển động & đòn đánh" trong [hero.md](../design/characters/hero.md): đứng, chạy, nhảy, rơi, combo 3 nhát, lướt, bị đánh, hóa quỷ; dạng quỷ: đứng, chạy, chém, gầm. Cập nhật manifest, tạo lại `hero_hd_parts`.
- Tiêu chí xong: mọi tư thế được duyệt, đúng anchor; approve vào `Assets/ArtHD`.
- Ghi chú:

## T-019 · Kael: animation chính
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-011, T-018
- Chi phí API: ~12 USD nếu Veo; 0 nếu thủ tục
- Công cụ: `new-asset`
- Việc cần làm: theo kết quả T-011: combo 3 nhát, chạy, hóa quỷ, chém dạng quỷ bằng Veo; còn lại tư thế tĩnh + `ProceduralPoseMotion`. Gắn vào `SpriteAnimator`. Thêm Veo cho lướt, bị đánh, nhảy nếu ngân sách cho phép.
- Tiêu chí xong: ảnh chụp các khung chính trong game; cảm giác đánh mượt khi chơi thử.
- Ghi chú:

## T-020 · Gaki + inugami: anchor + tư thế
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-013, T-014
- Chi phí API: ~7 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor mới; tư thế đi/chạy, tấn công, bị đánh, chết cho mỗi loại. Animation Veo cho đi/chạy và tấn công của mỗi loại (nếu T-011 đạt).
- Tiêu chí xong: approve vào `Assets/ArtHD`; skin HD trỏ đúng.
- Ghi chú:

## T-021 · Kỵ sĩ Quỷ: anchor + tư thế + animation
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-015, T-011
- Chi phí API: ~7 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor; tư thế đứng, đi, chém nặng, đâm, triệu hồi, bị đánh, gục; Veo cho đòn chém nặng nếu T-011 đạt.
- Tiêu chí xong: approve; ảnh chụp trận boss.
- Ghi chú:

## T-022 · Aldous + Morvain: anchor + tư thế NPC
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-016, T-017
- Chi phí API: ~2 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor + tư thế đứng, nói/ra hiệu cho mỗi người; ảnh chân dung cho phụ đề.
- Tiêu chí xong: approve; hiện đúng trong cảnh kịch bản.
- Ghi chú:

## T-023 · Valgrave: nền parallax, tileset, đạo cụ
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-012
- Chi phí API: ~5 USD
- Công cụ: `new-asset`; `unity:tilemap-ruletile-createfromsegment`, `unity:sprite-segment-3x3grid`
- Việc cần làm: 3 lớp nền (trời, thành xa, nhà gần); tileset đá + gỗ không lộ đường nối (thử Rule Tile hoặc dải liền); đạo cụ: đèn lồng, cổng, xác, thùng. Nền 4–5 lớp parallax (thêm lớp tiền cảnh và sương).
- Tiêu chí xong: ảnh chụp toàn màn không lộ đường nối ô.
- Ghi chú:

## T-024 · Thẻ truyện mở đầu + sau màn 1
- Trạng thái: todo
- Mốc: M1 · Loại: art
- Phụ thuộc: T-004, T-023
- Chi phí API: ~3 USD
- Công cụ: `new-asset`
- Việc cần làm: thêm loại asset thẻ truyện (16:9, tranh minh họa) vào pipeline; 2–3 thẻ: mở đầu, Kael bị trục xuất. Lời dẫn từ [story.md](../design/story.md). Mỗi thẻ 2 ảnh để pan/chuyển cảnh; 3–4 thẻ.
- Tiêu chí xong: thẻ hiện đúng trong luồng màn.
- Ghi chú:

## T-025 · Âm thanh màn 1
- Trạng thái: todo
- Mốc: M1 · Loại: audio
- Phụ thuộc: T-016, T-017
- Chi phí API: ~2 USD
- Công cụ: `new-asset` (`review` cho nhạc/giọng); `pipeline sfx`
- Việc cần làm: nhạc Valgrave (gothic + nhạc cụ Nhật), nhạc boss; thoại Kael, Aldous, Morvain, dân chúng; SFX mới (lướt, rút kiếm iai, bùa cháy).
- Tiêu chí xong: `review` đạt; nghe thử trong game.
- Ghi chú:

## T-026 · Dựng màn 1 Valgrave
- Trạng thái: todo
- Mốc: M1 · Loại: level
- Phụ thuộc: T-002, T-023
- Chi phí API: 0
- Công cụ: `build-game`, `run-tests`
- Việc cần làm: `LevelDefinition` Valgrave: khu ổ chuột ngoài tường → cổng thành → tường thành bị phá → quảng trường (đấu trường boss). Quái, checkpoint, vật cản. Độ khó tăng dần, dạy lướt.
- Tiêu chí xong: bot đi hết màn tới đấu trường; ảnh chụp toàn màn.
- Ghi chú:

## T-027 · Kịch bản màn 1
- Trạng thái: todo
- Mốc: M1 · Loại: story
- Phụ thuộc: T-005, T-022, T-025, T-026
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: cảnh lính bán chỗ trú cho kẻ giàu; sau boss: dân đòi giết Kael, Morvain "tha", Aldous tiễn và trao Vỏ kiếm Phong Ấn.
- Tiêu chí xong: chơi qua toàn bộ cảnh; test sự kiện kích hoạt đúng thứ tự.
- Ghi chú:

## T-028 · Boss Kỵ sĩ Quỷ + bộc phát bắt buộc
- Trạng thái: todo
- Mốc: M1 · Loại: gameplay
- Phụ thuộc: T-021, T-026
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: cập nhật `DemonKnightBoss` theo bộ chiêu mới; đến giai đoạn 2, Kael bị ép hóa quỷ (mất kiểm soát: điều khiển lệch, đòn mạnh, suýt chém dân) — cảnh bộc phát đầu tiên.
- Tiêu chí xong: test hạ được boss; bộc phát kích hoạt đúng một lần.
- Ghi chú:

## T-029 · Hoàn thiện M1
- Trạng thái: todo
- Mốc: M1 · Loại: test
- Phụ thuộc: T-019, T-020, T-021, T-022, T-023, T-024, T-025, T-026, T-027, T-028
- Chi phí API: ~1 USD (làm lại lặt vặt)
- Công cụ: `run-tests`, `build-game`
- Việc cần làm: chơi thử, cân chỉnh độ khó, sửa lỗi hình; ảnh chụp các khoảnh khắc chính; build.
- Tiêu chí xong: test qua hết; build chạy; người dùng chơi thử và duyệt.
- Ghi chú:

---

## M2 · Tro Ashveil
Task khung — tách nhỏ bằng `/roadmap` khi bắt đầu mốc.

## T-030 · Thanh Kiểm soát
- Trạng thái: todo
- Mốc: M2 · Loại: gameplay
- Phụ thuộc: T-006
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: logic tăng/giảm theo [story.md](../design/story.md); hiển thị gián tiếp (mạch đen lan trên Kael, HUD ngả đỏ); lửa đỏ → tím khi thấp; dưới 20 tự bộc phát; lưu trong `SaveData`.
- Tiêu chí xong: test từng quy tắc tăng/giảm và tự bộc phát.
- Ghi chú:

## T-031 · Vật phẩm ẩn: di vật Ysolde, mảnh thép quỷ
- Trạng thái: todo
- Mốc: M2 · Loại: gameplay
- Phụ thuộc: T-003
- Chi phí API: ~0,5 USD (icon)
- Công cụ: `new-asset`, `run-tests`
- Việc cần làm: nhặt, lưu, hiệu ứng (+Kiểm soát; 3 mảnh thép → +1 bậc kiếm); khai báo vị trí trong `LevelDefinition`.
- Tiêu chí xong: test nhặt và lưu.
- Ghi chú:

## T-032 · Concept màn 2
- Trạng thái: todo
- Mốc: M2 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept`
- Việc cần làm: Ashveil (zone), Mẹ Bầy, quỷ non, lính đánh thuê của Morvain.
- Tiêu chí xong: 4 tài liệu concept được duyệt.
- Ghi chú:

## T-033 · Asset nhân vật màn 2
- Trạng thái: todo
- Mốc: M2 · Loại: art
- Phụ thuộc: T-032
- Chi phí API: ~12 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor + tư thế cho Mẹ Bầy (boss), quỷ non, lính đánh thuê; animation boss theo hướng T-011. Animation Veo cho boss và quái.
- Tiêu chí xong: approve.
- Ghi chú:

## T-034 · Asset môi trường + thẻ truyện + âm thanh màn 2
- Trạng thái: todo
- Mốc: M2 · Loại: art
- Phụ thuộc: T-032
- Chi phí API: ~8 USD
- Công cụ: `new-asset`
- Việc cần làm: nền, tileset, đạo cụ (torii đổ, Jizo mất đầu); thẻ sau màn 2; nhạc màn + boss; thoại; icon Găng Xương Bầy.
- Tiêu chí xong: approve; nghe/xem thử trong game.
- Ghi chú:

## T-035 · Dựng màn 2 + nhà sập + kịch bản
- Trạng thái: todo
- Mốc: M2 · Loại: level
- Phụ thuộc: T-033, T-034
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: layout; nhà sập theo thời gian; dấu ấn nghi thức, thư Aldous, lính bắt nô lệ; vị trí di vật + mảnh thép.
- Tiêu chí xong: bot đi hết màn.
- Ghi chú:

## T-036 · Boss Mẹ Bầy + Găng Xương Bầy
- Trạng thái: todo
- Mốc: M2 · Loại: gameplay
- Phụ thuộc: T-035
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: AI boss (đẻ quỷ non, vồ); trao găng sau boss.
- Tiêu chí xong: test hạ boss + nhận găng.
- Ghi chú:

## T-037 · Hoàn thiện M2
- Trạng thái: todo
- Mốc: M2 · Loại: test
- Phụ thuộc: T-030, T-031, T-036
- Chi phí API: ~0,5 USD
- Công cụ: `run-tests`, `build-game`
- Việc cần làm: cân chỉnh, ảnh chụp, build; chơi nối từ màn 1 sang màn 2.
- Tiêu chí xong: test qua; người dùng duyệt.
- Ghi chú:

---

## M3 · Rừng Thối

## T-038 · Concept màn 3
- Trạng thái: todo
- Mốc: M3 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept`
- Việc cần làm: Rừng Thối (zone), Kẻ Nhả Độc, quỷ ăn thịt nhả độc, nhện quỷ, ẩn sĩ.
- Tiêu chí xong: tài liệu được duyệt.
- Ghi chú:

## T-039 · Asset nhân vật màn 3
- Trạng thái: todo
- Mốc: M3 · Loại: art
- Phụ thuộc: T-038
- Chi phí API: ~12 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor + tư thế boss, 2 loại quái, ẩn sĩ. Animation Veo cho boss và quái.
- Tiêu chí xong: approve.
- Ghi chú:

## T-040 · Asset môi trường + thẻ truyện + âm thanh màn 3
- Trạng thái: todo
- Mốc: M3 · Loại: art
- Phụ thuộc: T-038
- Chi phí API: ~8 USD
- Công cụ: `new-asset`
- Việc cần làm: nền rừng, tileset, shimenawa, tượng không mặt; thẻ; nhạc; thoại; icon Nanh Độc.
- Tiêu chí xong: approve.
- Ghi chú:

## T-041 · Dựng màn 3 + sương độc + kịch bản ẩn sĩ
- Trạng thái: todo
- Mốc: M3 · Loại: level
- Phụ thuộc: T-039, T-040
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: layout; sương độc chặn hồi máu; cây mục sập; cảnh ẩn sĩ bị ám sát.
- Tiêu chí xong: bot đi hết màn.
- Ghi chú:

## T-042 · Boss Kẻ Nhả Độc + Nanh Độc
- Trạng thái: todo
- Mốc: M3 · Loại: gameplay
- Phụ thuộc: T-041
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: AI boss (nhả độc, vùng độc); trao Nanh Độc (độc theo thời gian, kháng sương).
- Tiêu chí xong: test hạ boss + hiệu ứng độc.
- Ghi chú:

## T-043 · Hoàn thiện M3
- Trạng thái: todo
- Mốc: M3 · Loại: test
- Phụ thuộc: T-042
- Chi phí API: ~0,5 USD
- Công cụ: `run-tests`, `build-game`
- Việc cần làm: cân chỉnh, ảnh chụp, build.
- Tiêu chí xong: test qua; người dùng duyệt.
- Ghi chú:

---

## M4 · Hắc Diệm

## T-044 · Concept màn 4
- Trạng thái: todo
- Mốc: M4 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept`
- Việc cần làm: núi lửa Hắc Diệm (zone), Thống lĩnh, quỷ dung nham, quỷ bay.
- Tiêu chí xong: tài liệu được duyệt.
- Ghi chú:

## T-045 · Asset nhân vật màn 4
- Trạng thái: todo
- Mốc: M4 · Loại: art
- Phụ thuộc: T-044
- Chi phí API: ~12 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor + tư thế boss (gồm tư thế quỳ), 2 loại quái. Animation Veo cho boss và quái.
- Tiêu chí xong: approve.
- Ghi chú:

## T-046 · Asset môi trường + thẻ truyện + âm thanh màn 4
- Trạng thái: todo
- Mốc: M4 · Loại: art
- Phụ thuộc: T-044
- Chi phí API: ~8 USD
- Công cụ: `new-asset`
- Việc cần làm: nền núi lửa, tileset, torii cháy đen; thẻ; nhạc; thoại; icon giáp.
- Tiêu chí xong: approve.
- Ghi chú:

## T-047 · Dựng màn 4 + dung nham dâng + quỷ bỏ chạy
- Trạng thái: todo
- Mốc: M4 · Loại: level
- Phụ thuộc: T-045, T-046
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: layout leo dọc; dung nham dâng; đá rơi; quỷ yếu bỏ chạy khi thấy Kael (giết quỷ bỏ chạy trừ Kiểm soát).
- Tiêu chí xong: bot đi hết màn.
- Ghi chú:

## T-048 · Boss Thống lĩnh + Giáp Hắc Diệm
- Trạng thái: todo
- Mốc: M4 · Loại: gameplay
- Phụ thuộc: T-047
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: AI boss; cảnh quỳ gọi "Hoàng tử"; trao giáp (giảm sát thương, miễn bỏng, vệt lửa).
- Tiêu chí xong: test hạ boss + hiệu ứng giáp.
- Ghi chú:

## T-049 · Hoàn thiện M4
- Trạng thái: todo
- Mốc: M4 · Loại: test
- Phụ thuộc: T-048
- Chi phí API: ~0,5 USD
- Công cụ: `run-tests`, `build-game`
- Việc cần làm: cân chỉnh, ảnh chụp, build.
- Tiêu chí xong: test qua; người dùng duyệt.
- Ghi chú:

---

## M5 · Cánh cổng

## T-050 · Hệ lựa chọn tha/giết + chọn kết thúc
- Trạng thái: todo
- Mốc: M5 · Loại: gameplay
- Phụ thuộc: T-030
- Chi phí API: 0
- Công cụ: `unity:ui-ugui`, `run-tests`
- Việc cần làm: UI lựa chọn hai phương án có hẹn giờ; lưu lựa chọn; hàm chọn kết thúc từ lựa chọn cuối + Kiểm soát.
- Tiêu chí xong: test bảng điều kiện kết thúc.
- Ghi chú:

## T-051 · Concept màn 5
- Trạng thái: todo
- Mốc: M5 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept`
- Việc cần làm: Cánh cổng (torii xương), Morvain dạng boss, lính của Hội, quỷ khế ước.
- Tiêu chí xong: tài liệu được duyệt.
- Ghi chú:

## T-052 · Asset nhân vật màn 5
- Trạng thái: todo
- Mốc: M5 · Loại: art
- Phụ thuộc: T-051
- Chi phí API: ~14 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor + tư thế Morvain dạng boss (gồm quỳ van xin, giả chết), lính, quỷ khế ước; Aldous bị trói. Animation Veo cho boss và quái.
- Tiêu chí xong: approve.
- Ghi chú:

## T-053 · Asset môi trường + thẻ truyện + âm thanh màn 5
- Trạng thái: todo
- Mốc: M5 · Loại: art
- Phụ thuộc: T-051
- Chi phí API: ~8 USD
- Công cụ: `new-asset`
- Việc cần làm: nền cánh cổng, tileset cầu xương; thẻ; nhạc; thoại Morvain; icon mặt nạ.
- Tiêu chí xong: approve.
- Ghi chú:

## T-054 · Dựng màn 5 + cổng phản ứng theo nộ khí
- Trạng thái: todo
- Mốc: M5 · Loại: level
- Phụ thuộc: T-052, T-053
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: layout; đường mở theo mức nộ khí; cầu xích; lính người + quỷ.
- Tiêu chí xong: bot đi hết màn.
- Ghi chú:

## T-055 · Boss Morvain + lựa chọn + Mặt nạ Kim Ấn
- Trạng thái: todo
- Mốc: M5 · Loại: gameplay
- Phụ thuộc: T-050, T-054
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: boss nhiều giai đoạn (gọi lính, dùng Aldous làm con tin, giả chết); lựa chọn tha/giết; trao mặt nạ (tùy chọn đeo).
- Tiêu chí xong: test cả hai nhánh lựa chọn.
- Ghi chú:

## T-056 · Hoàn thiện M5
- Trạng thái: todo
- Mốc: M5 · Loại: test
- Phụ thuộc: T-055
- Chi phí API: ~0,5 USD
- Công cụ: `run-tests`, `build-game`
- Việc cần làm: cân chỉnh, ảnh chụp, build.
- Tiêu chí xong: test qua; người dùng duyệt.
- Ghi chú:

---

## M6 · Lò Nguồn

## T-057 · Concept màn 6
- Trạng thái: todo
- Mốc: M6 · Loại: story
- Phụ thuộc: —
- Chi phí API: 0
- Công cụ: `/concept`
- Việc cần làm: Lò Nguồn (zone), Quỷ vương, hộ vệ ngai.
- Tiêu chí xong: tài liệu được duyệt.
- Ghi chú:

## T-058 · Asset nhân vật màn 6
- Trạng thái: todo
- Mốc: M6 · Loại: art
- Phụ thuộc: T-057
- Chi phí API: ~18 USD
- Công cụ: `new-asset`
- Việc cần làm: anchor + tư thế Quỷ vương (2 giai đoạn, rất lớn), hộ vệ ngai; animation boss. Animation Veo cho cả 2 giai đoạn.
- Tiêu chí xong: approve.
- Ghi chú:

## T-059 · Asset môi trường + thẻ truyện + âm thanh màn 6
- Trạng thái: todo
- Mốc: M6 · Loại: art
- Phụ thuộc: T-057
- Chi phí API: ~8 USD
- Công cụ: `new-asset`
- Việc cần làm: nền Lò Nguồn, tileset thịt sống, ngai; nhạc cuối; thoại Quỷ vương.
- Tiêu chí xong: approve.
- Ghi chú:

## T-060 · Dựng màn 6
- Trạng thái: todo
- Mốc: M6 · Loại: level
- Phụ thuộc: T-058, T-059
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: layout; quỷ cúi đầu khi Kael đi qua; đấu trường ngai.
- Tiêu chí xong: bot đi hết màn.
- Ghi chú:

## T-061 · Boss Quỷ vương 2 giai đoạn
- Trạng thái: todo
- Mốc: M6 · Loại: gameplay
- Phụ thuộc: T-060
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: giai đoạn 1 thường; giai đoạn 2 thịnh nộ không giới hạn, bộc phát mạnh nhất.
- Tiêu chí xong: test hạ boss.
- Ghi chú:

## T-062 · 2 kết thúc × 2 biến thể
- Trạng thái: todo
- Mốc: M6 · Loại: story
- Phụ thuộc: T-050, T-061
- Chi phí API: ~5 USD
- Công cụ: `new-asset`, `run-tests`
- Việc cần làm: lựa chọn vương miện; 4 chuỗi thẻ kết thúc + cảnh hậu kết; thoại.
- Tiêu chí xong: test cả 4 biến thể.
- Ghi chú:

## T-063 · Hoàn thiện M6
- Trạng thái: todo
- Mốc: M6 · Loại: test
- Phụ thuộc: T-062
- Chi phí API: ~0,5 USD
- Công cụ: `run-tests`, `build-game`
- Việc cần làm: chơi xuyên 6 màn; cân chỉnh; build.
- Tiêu chí xong: test qua; người dùng duyệt.
- Ghi chú:

---

## M7 · Hoàn thiện

## T-064 · Menu chính, tạm dừng, cài đặt
- Trạng thái: todo
- Mốc: M7 · Loại: ui
- Phụ thuộc: T-003
- Chi phí API: ~1 USD (ảnh nền menu)
- Công cụ: `unity:ui-ugui`, `new-asset`
- Việc cần làm: tiếp tục/chơi mới, âm lượng, độ phân giải, phím.
- Tiêu chí xong: ảnh chụp; test điều hướng.
- Ghi chú:

## T-065 · Mixer + tối ưu âm thanh
- Trạng thái: todo
- Mốc: M7 · Loại: audio
- Phụ thuộc: T-063
- Chi phí API: 0
- Công cụ: `unity:audio-setup-mixers`, `unity:optimize-audio`
- Việc cần làm: nhóm Music/SFX/Voice, ducking khi thoại; import settings.
- Tiêu chí xong: nghe thử; bộ nhớ âm thanh hợp lý.
- Ghi chú:

## T-066 · Sprite atlas + hiệu năng
- Trạng thái: todo
- Mốc: M7 · Loại: tech
- Phụ thuộc: T-063
- Chi phí API: 0
- Công cụ: `unity:manage-sprite-atlas`
- Việc cần làm: atlas theo màn; kiểm tra draw call, bộ nhớ texture, FPS.
- Tiêu chí xong: 60 FPS ổn định ở 1080p trên máy thử.
- Ghi chú:

## T-067 · Cân bằng toàn game
- Trạng thái: todo
- Mốc: M7 · Loại: test
- Phụ thuộc: T-063
- Chi phí API: 0
- Công cụ: `run-tests`
- Việc cần làm: bot chơi xuyên game; chỉnh máu/sát thương/nộ khí/Kiểm soát; chơi thử với người dùng.
- Tiêu chí xong: người dùng duyệt độ khó.
- Ghi chú:

## T-068 · Build phát hành
- Trạng thái: todo
- Mốc: M7 · Loại: tech
- Phụ thuộc: T-064, T-065, T-066, T-067
- Chi phí API: 0
- Công cụ: `build-game`
- Việc cần làm: build release, README chơi thử, tag git.
- Tiêu chí xong: bản build chạy trên máy sạch.
- Ghi chú:
