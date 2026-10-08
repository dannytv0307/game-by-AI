# DevilBlade

Game hành động 2D pixel art (platformer + chiến đấu). Toàn bộ hình ảnh và âm thanh được tạo bằng AI trên Google Vertex AI, thông qua pipeline trong `asset-pipeline/`.

- `DevilBlade/`: dự án Unity 6000.3.25f1 (URP 2D). Màn 1 chơi được từ đầu đến khi hạ boss.
- `asset-pipeline/`: pipeline Python tạo đồ họa và âm thanh, có cơ chế giữ phong cách đồng nhất.
- `CLAUDE.md`: quy tắc đồng nhất asset và các lệnh làm việc chi tiết.

## Clone

Ảnh và âm thanh được lưu bằng Git LFS, nên cần cài LFS trước khi clone:

```bash
git lfs install
git clone https://github.com/dannytv0307/game-by-AI.git
```

## Chơi thử

1. Mở thư mục `DevilBlade/` bằng Unity Hub, chọn đúng Editor **6000.3.25f1**.
2. Mở scene `Assets/Scenes/Level1.unity` rồi bấm Play.
3. Muốn build bản Windows: menu **DevilBlade → Build Windows Player**. File chạy được tạo ở `DevilBlade/Builds/Windows/DevilBlade.exe`.

Phím điều khiển:

| Phím | Hành động |
|---|---|
| A / D | Di chuyển |
| Space | Nhảy (S + Space để rơi xuyên giàn gỗ) |
| J | Chém |
| K | Hóa quỷ khi nộ khí đầy |
| Esc | Tạm dừng |

## Chạy asset pipeline

Yêu cầu:
- Python 3.12 trở lên.
- Một project Google Cloud đã bật billing và bật Vertex AI API.
- Tài khoản Google có quyền dùng Vertex AI trên project đó.

```bash
cd asset-pipeline
python -m venv .venv
.venv\Scripts\pip install -r requirements.txt     # macOS/Linux: .venv/bin/pip
gcloud auth application-default login             # đăng nhập ADC, không lưu key trong repo
gcloud services enable aiplatform.googleapis.com --project <PROJECT_ID>
```

Sau đó sửa `gcp.project` trong `asset-pipeline/config.yaml` thành project của bạn, rồi chạy thử:

```bash
.venv\Scripts\python -m pipeline status
.venv\Scripts\python -m pipeline gen hero_idle -n 2
```

Repo không chứa credential nào. Xác thực dùng Application Default Credentials của từng máy. Các lệnh khác và quy tắc giữ phong cách đồng nhất có trong `CLAUDE.md`.
