// PreToolUse hook (Edit/Write/NotebookEdit): chặn sửa tay những file phải sinh ra từ pipeline hoặc từ code.
// Quy tắc nằm trong CLAUDE.md. Exit 2 = chặn, nội dung stderr được gửi lại cho Claude.
let raw = "";
process.stdin.on("data", (c) => (raw += c));
process.stdin.on("end", () => {
  let input;
  try { input = JSON.parse(raw); } catch { process.exit(0); }
  const file = String(input?.tool_input?.file_path || input?.tool_input?.notebook_path || "")
    .replace(/\\/g, "/");
  if (!file) process.exit(0);

  const rules = [
    [/\/DevilBlade\/Assets\/(ArtHD|Audio)\//i,
      "Asset đồ họa/âm thanh phải đi qua asset-pipeline (manifest → gen → approve), không ghi tay vào Assets/ArtHD, Audio."],
    [/\/DevilBlade\/Assets\/Scenes\/Level1(_HD)?\.unity$/i,
      "Scene được dựng bằng code: sửa LEVEL DATA trong Assets/Editor/LevelBuilder/Level1Builder.cs rồi chạy Level1Builder.BuildHeadless."],
    [/\/DevilBlade\/Packages\/manifest\.json$/i,
      "Không sửa tay Packages/manifest.json: cài package qua Assets/Editor/ProjectBootstrap/PackageInstaller.cs."],
    [/\/asset-pipeline\/logs\/generations\.jsonl$/i,
      "generations.jsonl là nhật ký nguồn gốc asset, chỉ pipeline được ghi thêm. Không sửa hoặc xóa."],
    [/\/asset-pipeline\/refs\/[^/]+\.png$/i,
      "Ảnh anchor trong refs/ chỉ được thay bằng `pipeline approve`. Đổi anchor thì phải tạo lại các asset phụ thuộc."],
  ];

  for (const [re, msg] of rules) {
    if (re.test(file)) {
      process.stderr.write(`Bị chặn bởi .claude/hooks/guard-assets.js: ${msg}\nFile: ${file}\n`);
      process.exit(2);
    }
  }
  process.exit(0);
});
