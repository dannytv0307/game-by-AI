# DevilBlade — Định hướng mỹ thuật

> Trạng thái: **bản nháp 2** (bảng màu tối hơn, thêm tím; Kael vạm vỡ hơn), chờ duyệt. Chưa áp dụng vào `asset-pipeline/bible/style_hd.yaml`.
> Từ khóa người dùng chốt: **Dark Fantasy + Gothic + Psychological Horror + Japanese Fantasy**.

## Một câu
**Một thế giới nơi nhà thờ gothic mục nát và đền thần Nhật Bản đổ nát mọc lẫn vào nhau, quỷ mang hình hài yêu quái, và nỗi sợ đến từ cảm giác "có gì đó sai" hơn là máu me.**

Tham chiếu tinh thần: Sekiro và Nioh (yêu quái, samurai sa đọa), Bloodborne và Blasphemous (gothic, tôn giáo mục ruỗng), Berserk (dark fantasy, thịnh nộ), Junji Ito và Silent Hill (kinh dị tâm lý), tranh mực tàu sumi-e và tranh ukiyo-e ma quái của Tsukioka Yoshitoshi.

## Bốn trụ cột và vai trò của từng cái

| Trụ cột | Đóng góp gì cho hình ảnh | Dùng ở đâu |
|---|---|---|
| **Dark Fantasy** | Thế giới tàn lụi, vật liệu cũ nát, vũ khí nặng nề, cảm giác cái chết ở khắp nơi | Toàn bộ game; là nền chung |
| **Gothic** | Kiến trúc thẳng đứng, nhọn, vòm cung, kính màu vỡ, tượng thánh che mặt; tương phản sáng tối mạnh (chiaroscuro) | Thành trì của loài người, tu viện, Hội Kim Ấn |
| **Psychological Horror** | Sự bất thường thay vì máu me: quá nhiều mắt, khuôn mặt ẩn trong vân gỗ, nụ cười kéo dài, người đứng im ở hậu cảnh, chi tiết lặp lại (bàn tay, con mắt), tỉ lệ cơ thể hơi sai | Quỷ, các vùng đất càng gần Lò Nguồn càng nặng; dạng quỷ của Kael |
| **Japanese Fantasy** | Yêu quái (oni, gaki, inugami), đền thần, cổng torii, đèn đá, dây shimenawa, mặt nạ Noh, áo haori, kiếm nodachi; nét mực tàu | Hình dáng quỷ, trang phục, kiến trúc trộn lẫn, đạo cụ |

**Nguyên tắc trộn:** loài người mang **gothic phương Tây pha Nhật** (nhà thờ đá có mái cong, tu sĩ đội nón rơm); loài quỷ mang **yêu quái Nhật** (oni, ngạ quỷ). Càng đi sâu vào lãnh địa quỷ, chất gothic càng mất dần và chất yêu quái kinh dị càng đậm.

## Cách vẽ
- **Hiện thực kiểu tranh sơn** (như concept art của FromSoftware) — giữ hướng HD hiện tại.
- Thêm **chất mực tàu**: viền bóng đổ loang như mực, vết cọ khô ở rìa áo choàng, khói và tro vẽ như nét mực. Không dùng cel shading anime.
- Ánh sáng: một nguồn sáng trăng lạnh từ trên bên trái, bóng tối sâu. Lửa quỷ là nguồn sáng thứ hai duy nhất.
- Hình khối phải đọc được ở kích thước nhỏ: mỗi nhân vật có một chi tiết nhận diện từ xa (nón, sừng, mặt nạ, vũ khí).

## Bảng màu
Tổng thể **tối hơn**: phần lớn khung hình nằm trong vùng tối; chỉ viền sáng trăng, mắt, lửa và kim loại mới bắt sáng. Trắng xương chỉ là điểm nhấn nhỏ.

| Nhóm | Màu |
|---|---|
| Nền chung | đen mực, đen than ánh chàm, xám tro sẫm, xanh đá rất tối |
| Bóng tối, trời đêm, sương | **tím than / chàm sẫm** — bóng không đen chết mà ngả tím |
| Loài người, kiến trúc | xám thép tối, gỗ mục nâu đen, giấy washi ngả vàng đục (rất ít) |
| Đền thần Nhật | đỏ son **đã phai thành nâu gỉ**, không dùng đỏ son tươi |
| Linh hồn, yêu thuật, quỷ cấp cao | **tím u linh** (ma trơi onibi, ký tự trên Cánh cổng, phép của Quỷ vương, Lò Nguồn) |
| Máu, thịnh nộ, lửa của Kael | đỏ máu khô, đỏ than hồng |
| Hội Kim Ấn | vàng lá kintsugi, xỉn, chỉ ở vết nứt |

**Ý nghĩa của hai màu lửa:**
- **Đỏ** là thịnh nộ và máu: lửa của Kael, thứ lửa sinh ra từ cảm xúc con người.
- **Tím** là quỷ thuần chủng và yêu thuật: lửa của Quỷ vương và Lò Nguồn.
- Khi Kiểm soát của Kael thấp, lửa đỏ của hắn **ngả dần sang tím**. Người chơi nhìn màu là biết Kael đang mất mình.

Quy tắc dự án được mở rộng: đỏ, cam, vàng lửa và **tím sáng** chỉ dùng cho quỷ, máu, nộ khí, linh hồn. Tím **sẫm** được dùng cho bóng tối và môi trường.

## Áp dụng cho nhân vật và quái (gợi ý, chốt chi tiết bằng `/concept <id>`)

| Đối tượng | Hiện tại | Hướng mới |
|---|---|---|
| **Kael** (`hero`) | Kiếm sĩ gầy gò, hốc hác | **Chiến binh** vai rộng, cơ bắp săn chắc, đầy sẹo; thợ săn quỷ lai ronin với haori đen rách ngoài giáp da gothic, nodachi. Chi tiết: [characters/hero.md](characters/hero.md). |
| **Kael dạng quỷ** (`hero_demon`) | Quỷ cháy đen, gầy dài, sừng | **Oni** to lớn, vạm vỡ hơn dạng người (không còn gầy dài): hai sừng mọc lệch, nửa mặt nứt thành **mặt nạ hannya**, miệng toác đến mang tai, quá nhiều khớp ngón tay. Kinh dị vì vẫn nhận ra nét mặt Kael bên dưới. |
| **Imp** (`imp`) | Quỷ nhỏ cánh dơi | **Gaki (ngạ quỷ)**: thân gầy trơ xương, bụng phình, miệng khâu chỉ — khớp với ý "quỷ là cơn đói". |
| **Hellhound** (`hellhound`) | Chó quỷ đầu sọ | **Inugami**: chó quỷ không da, đầu treo bùa giấy, đi bằng những chân dài bất thường. |
| **Kỵ sĩ Quỷ** (`demon_knight`) | Hiệp sĩ giáp sắt mục | **Ochimusha** (samurai sa đọa): giáp ō-yoroi mục rữa lẫn giáp tấm gothic, mặt nạ menpō, lá cờ sashimono rách. |
| **Tu sĩ Tro Trắng** | — | Nón rơm rũ che kín mặt kiểu **komusō**, áo tu sĩ gothic màu tro trắng. Không thấy mặt ai — gây bất an. |
| **Hội Kim Ấn, Morvain** | — | Quý tộc gothic với **mặt nạ Noh** mạ vàng, da nứt trám vàng kintsugi. |
| **Quỷ vương** | — | Oni khổng lồ như núi lửa nguội, vương miện sừng, hàng trăm con mắt nhắm trên thân thể, chỉ mở khi hắn nói. |

## Áp dụng cho vùng đất

| Vùng | Hướng mới |
|---|---|
| Thành trì Valgrave | Lâu đài Nhật (mái cong nhiều tầng) xây trên nền nhà thờ gothic; khu ổ chuột dưới chân tường thành, đèn lồng giấy. |
| Tro Ashveil | Thị trấn cháy rụi: cổng torii đổ, đèn đá, tượng Jizo mất đầu, tháp chuông gothic gãy. |
| Rừng Thối | Rừng tre và tuyết tùng mục, dây shimenawa quấn cây, bùa giấy trên thân cây, tượng không mặt dọc lối đi (cảm hứng rừng Aokigahara). |
| Núi lửa Hắc Diệm | Đền thờ núi lửa bị quỷ chiếm, bậc đá dẫn lên miệng núi, hàng nghìn torii nhỏ cháy đen. |
| Cánh cổng Địa ngục | Cánh cổng là **một cổng torii khổng lồ làm bằng xương**, quấn xích, khắc ký tự tím phát sáng. |
| Lò Nguồn | Hang thịt sống như tử cung, vách có khuôn mặt người, mắt mở nhắm; điện thờ ngược treo từ trần. |

## Ảnh hưởng nếu áp dụng
- Đổi phong cách HD nghĩa là phải tạo lại **style tile HD** rồi **anchor** của mọi nhân vật HD, sau đó tới các tư thế, nền và tileset.
  - Hiện có khoảng 25 ảnh HD đã duyệt. Tạo lại ước tính **4–6 USD**, gồm cả vài lần làm lại.
- Màn 1 đằng nào cũng làm lại theo cốt truyện mới, nên đây là lúc đổi phong cách ít tốn kém nhất.
- Bản pixel không bị ảnh hưởng (dùng `style.yaml` riêng).

## Đề xuất `style_prompt` mới cho `style_hd.yaml` (chưa áp dụng)

```
Dark fantasy gothic horror 2D game art fused with Japanese folklore, painterly concept-art realism in the vein of
Sekiro, Nioh, Bloodborne and Blasphemous key art, with sumi-e ink wash textures: ink-bleed shadow edges, dry-brush
strokes on cloth, smoke and ash drawn like ink. Grim, eerie, psychologically unsettling atmosphere where something
always feels wrong. Very dark low-key image: most of the frame sits in shadow, lit only by a cold violet-tinted
moonlight rim light from the upper left; shadows fall into deep indigo and bruised purple, never flat black.
Palette of ink black, charcoal with indigo undertone, dark ash grey, deep violet and very dark slate blue, bone
white only as tiny accents; shrine lacquer only as faded rust-brown. Saturated colors are reserved: dried-blood
crimson and smouldering ember red for blood, rage and Kael's fire; glowing spectral violet for pure demons,
spirits and curses. Weathered, tattered materials with fine texture
detail, sharp clean silhouette readable at small size.
```

Thêm vào `negative_prompt`: `no bright vermilion, no clean anime style, no kawaii, no gore splatter, no bright high-key lighting, no pastel colors`.

## Câu hỏi còn mở
1. Tên riêng trong cốt truyện đang theo kiểu phương Tây (Valgrave, Aldous, Morvain). Giữ nguyên, Nhật hóa toàn bộ, hay trộn (loài người tên phương Tây, quỷ tên Nhật)?
2. Mức kinh dị: "bất an, ám ảnh" (khuyên dùng, không gore) hay chấp nhận cả body horror nặng?
3. Có ảnh tham khảo nào bạn thích (game, tranh, phim)? Bỏ vào `docs/concepts/_style/` để pipeline gửi kèm khi tạo style tile.
