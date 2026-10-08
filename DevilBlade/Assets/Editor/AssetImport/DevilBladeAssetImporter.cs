using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace DevilBlade.EditorTools
{
    /// <summary>
    /// Tự cấu hình import cho asset do asset-pipeline/ sinh ra:
    ///  - Assets/Art/**  : sprite pixel art (Point, không nén, PPU cố định). Sprite sheet animation được
    ///    cắt theo file *.sheet.json đi kèm; tileset cắt theo lưới tile.
    ///  - Assets/Audio/**: Music = streaming, SFX = giải nén khi load, Voice = nén trong bộ nhớ.
    /// PPU và kích thước tile phải khớp asset-pipeline/bible/style.yaml (art.pixel).
    /// </summary>
    public class DevilBladeAssetImporter : AssetPostprocessor
    {
        const int PixelsPerUnit = 32;
        const int TileSize = 32;
        const string ArtRoot = "Assets/Art/";
        const string AudioRoot = "Assets/Audio/";

        [Serializable]
        class SheetMeta
        {
            public int frames;
            public int cell_w;
            public int cell_h;
            public float[] pivot;
        }

        // Nhánh thử nghiệm HD (asset-pipeline/bible/style_hd.yaml). Kích thước trong thế giới giữ như bản pixel:
        // Kael HD 360px ≈ 2.1 unit, 1 tile 256px = 1 unit, nền 1920px ≈ 20 unit.
        const string ArtHdRoot = "Assets/ArtHD/";
        const int HdCharacterPPU = 170;
        const int HdTileSize = 256;
        const int HdBackgroundPPU = 96;

        void OnPreprocessTexture()
        {
            var hd = assetPath.StartsWith(ArtHdRoot);
            if (!hd && !assetPath.StartsWith(ArtRoot)) return;
            var root = hd ? ArtHdRoot : ArtRoot;
            var tileSize = hd ? HdTileSize : TileSize;
            var ti = (TextureImporter)assetImporter;
            ti.textureType = TextureImporterType.Sprite;
            ti.spritePixelsPerUnit = !hd ? PixelsPerUnit
                : assetPath.StartsWith(root + "Tiles/") ? HdTileSize
                : assetPath.StartsWith(root + "Backgrounds/") ? HdBackgroundPPU
                : HdCharacterPPU;
            ti.filterMode = hd ? FilterMode.Bilinear : FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            if (hd) ti.maxTextureSize = 4096;

            var sheetPath = Path.ChangeExtension(assetPath, ".sheet.json");
            var meta = File.Exists(sheetPath) ? JsonUtility.FromJson<SheetMeta>(File.ReadAllText(sheetPath)) : null;
            if (meta is { frames: > 0, pivot: { Length: 2 } })
            {
                var pivot = new Vector2(meta.pivot[0], meta.pivot[1]);
                ti.spriteImportMode = SpriteImportMode.Multiple;
                SliceGrid(ti, meta.cell_w, meta.cell_h, meta.frames, 1, pivot);
            }
            else if (assetPath.StartsWith(root + "Tiles/"))
            {
                ti.spriteImportMode = SpriteImportMode.Multiple;
                var (w, h) = TextureSize(ti);
                SliceGrid(ti, tileSize, tileSize, w / tileSize, h / tileSize, new Vector2(0.5f, 0.5f));
            }
            else
            {
                ti.spriteImportMode = SpriteImportMode.Single;
                if (hd && !assetPath.StartsWith(root + "Backgrounds/"))
                {
                    // sprite đơn của nhân vật: pivot ở chân (đáy-giữa) để khớp collider & mặt đất
                    var s = new TextureImporterSettings();
                    ti.ReadTextureSettings(s);
                    s.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    ti.SetTextureSettings(s);
                }
            }
        }

        static (int, int) TextureSize(TextureImporter ti)
        {
            // Đọc header PNG (width/height ở byte 16..23) để không phụ thuộc lần import trước.
            var bytes = File.ReadAllBytes(ti.assetPath);
            int Be(int o) => (bytes[o] << 24) | (bytes[o + 1] << 16) | (bytes[o + 2] << 8) | bytes[o + 3];
            return (Be(16), Be(20));
        }

        /// <summary>Cắt lưới cols x rows, đọc trái→phải, trên→dưới. Tên sprite = tên file + _index.</summary>
        static void SliceGrid(TextureImporter ti, int cellW, int cellH, int cols, int rows, Vector2 pivot)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dp = factory.GetSpriteEditorDataProviderFromObject(ti);
            dp.InitSpriteEditorDataProvider();

            var (_, texH) = TextureSize(ti);
            var baseName = Path.GetFileNameWithoutExtension(ti.assetPath);
            var rects = new List<SpriteRect>();
            var ids = new List<SpriteNameFileIdPair>();
            for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                var name = $"{baseName}_{r * cols + c}";
                var rect = new SpriteRect
                {
                    name = name,
                    rect = new Rect(c * cellW, texH - (r + 1) * cellH, cellW, cellH),
                    alignment = SpriteAlignment.Custom,
                    pivot = pivot,
                    spriteID = StableGuid(ti.assetPath + name),
                };
                rects.Add(rect);
                ids.Add(new SpriteNameFileIdPair(name, rect.spriteID));
            }
            dp.SetSpriteRects(rects.ToArray());
            // GUID ổn định theo tên => Animation clip / prefab không mất tham chiếu khi gen lại sprite sheet
            dp.GetDataProvider<ISpriteNameFileIdDataProvider>()?.SetNameFileIdPairs(ids);
            dp.Apply();
        }

        static GUID StableGuid(string s)
        {
            using var md5 = MD5.Create();
            var h = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var sb = new StringBuilder(32);
            foreach (var b in h) sb.Append(b.ToString("x2"));
            return new GUID(sb.ToString());
        }

        void OnPreprocessAudio()
        {
            if (!assetPath.StartsWith(AudioRoot)) return;
            var ai = (AudioImporter)assetImporter;
            var s = ai.defaultSampleSettings;
            if (assetPath.StartsWith(AudioRoot + "Music/"))
            {
                s.loadType = AudioClipLoadType.Streaming;
                s.compressionFormat = AudioCompressionFormat.Vorbis;
                s.quality = 0.7f;
            }
            else if (assetPath.StartsWith(AudioRoot + "SFX/"))
            {
                s.loadType = AudioClipLoadType.DecompressOnLoad;
                s.compressionFormat = AudioCompressionFormat.ADPCM;
            }
            else
            {
                s.loadType = AudioClipLoadType.CompressedInMemory;
                s.compressionFormat = AudioCompressionFormat.Vorbis;
                s.quality = 0.6f;
            }
            ai.defaultSampleSettings = s;
        }

        // Khi chỉ file .sheet.json thay đổi, import lại texture tương ứng để cắt lại.
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            foreach (var p in imported)
            {
                if (!(p.StartsWith(ArtRoot) || p.StartsWith(ArtHdRoot)) || !p.EndsWith(".sheet.json")) continue;
                var png = p.Substring(0, p.Length - ".sheet.json".Length) + ".png";
                if (File.Exists(png)) AssetDatabase.ImportAsset(png, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
