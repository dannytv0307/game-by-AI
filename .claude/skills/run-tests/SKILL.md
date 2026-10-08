---
name: run-tests
description: Chạy test PlayMode của DevilBlade (cả Level1 và Level1_HD, gồm bot chạy hết màn) bằng Unity headless, tùy chọn chụp ảnh lúc chơi. Dùng sau khi sửa gameplay, level hoặc asset, hoặc khi người dùng muốn kiểm tra game.
---

# Chạy test PlayMode

Test nằm ở `DevilBlade/Assets/Tests/PlayMode/`, mỗi fixture chạy trên cả `Level1` và `Level1_HD`.
Nếu vừa sửa `Level1Builder.cs` thì dựng lại scene trước (`DevilBlade.EditorTools.Level1Builder.BuildHeadless`).

```powershell
$root = "$PWD\DevilBlade"; $xml = "$env:TEMP\devilblade_tests.xml"; $log = "$env:TEMP\devilblade_tests.log"
$argv = "-batchmode","-projectPath","`"$root`"","-runTests","-testPlatform","PlayMode","-testResults","`"$xml`"","-logFile","`"$log`""
# Chỉ chụp ảnh: thêm "-testCategory","Capture"
$p = Start-Process -PassThru "C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe" -ArgumentList $argv
$p.WaitForExit(); "exit $($p.ExitCode)"
Select-String -Path $xml -Pattern '<test-run .*?total="(\d+)" passed="(\d+)" failed="(\d+)"' | ForEach-Object Line
Select-String -Path $xml -Pattern 'result="Failed"' -Context 0,6 | Select-Object -First 5
```

- Exit 0 = tất cả qua; exit 2 = có test hỏng (đọc `<failure><message>` trong file XML).
- Không truyền `-quit` cùng `-runTests`.
- Ảnh chụp từ `Level1CaptureTests` nằm ở `DevilBlade/Logs/Screens/{scene}_{tên}.png`. Xem ảnh bằng Read để duyệt hình ảnh trước khi báo cáo.
- Báo kết quả trung thực: số test qua/hỏng theo từng scene, kèm thông báo lỗi nếu có.
