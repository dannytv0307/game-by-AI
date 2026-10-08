---
name: build-game
description: Dựng lại scene Level1 (HD) và build DevilBlade.exe cho Windows bằng Unity headless. Dùng khi người dùng muốn build, xuất bản chơi thử hoặc cập nhật file exe.
---

# Build DevilBlade cho Windows

1. Kiểm tra không có Unity Editor nào đang mở project `DevilBlade` (Editor đang mở sẽ khóa project, batchmode báo lỗi). Tiến trình `Unity Hub\resources\unity.exe serve` là CLI của Hub, không sao.
2. Kiểm tra `DevilBlade.exe` không đang chạy (file exe đang chạy sẽ không ghi đè được).
3. Chạy bằng PowerShell, dùng `Start-Process -PassThru` + `WaitForExit()` (không dùng `-Wait`, không truyền `-quit`):

```powershell
$root = "$PWD\DevilBlade"; $log = "$env:TEMP\devilblade_build.log"
$p = Start-Process -PassThru "C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe" `
  -ArgumentList "-batchmode","-projectPath","`"$root`"","-executeMethod","DevilBlade.EditorTools.GameBuild.RebuildAndBuildHeadless","-logFile","`"$log`""
$p.WaitForExit(); "exit $($p.ExitCode)"
Select-String -Path $log -Pattern "\[GameBuild\]|error CS" | Select-Object -First 10 | ForEach-Object Line
```

4. Thành công khi exit 0 và có dòng `[GameBuild] Succeeded`. Nếu lỗi `error CS`, sửa code rồi chạy lại.
5. Kết quả: `DevilBlade/Builds/Windows/DevilBlade.exe` (không nằm trong git). Hiện có một scene: `Level1`.
6. Scene `.unity` và `ProjectSettings/EditorBuildSettings.asset` thay đổi sau khi dựng: commit cùng thay đổi code.
