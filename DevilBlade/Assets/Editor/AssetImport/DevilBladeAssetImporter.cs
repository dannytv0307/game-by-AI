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

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot)) return;
            var ti = (TextureImporter)assetImporter;
            ti.textureType = TextureImporterType.Sprite;
            ti.spritePixelsPerUnit = PixelsPerUnit;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;

            var sheetPath = Path.ChangeExtension(assetPath, ".sheet.json");
            if (File.Exists(sheetPath))
            {
                var meta = JsonUtility.FromJson<SheetMeta>(File.ReadAllText(sheetPath));
                var pivot = new Vector2(meta.pivot[0], meta.pivot[1]);
                ti.spriteImportMode = SpriteImportMode.Multiple;
                SliceGrid(ti, meta.cell_w, meta.cell_h, meta.frames, 1, pivot);
            }
            else if (assetPath.StartsWith(ArtRoot + "Tiles/"))
            {
                ti.spriteImportMode = SpriteImportMode.Multiple;
                var (w, h) = TextureSize(ti);
                SliceGrid(ti, TileSize, TileSize, w / TileSize, h / TileSize, new Vector2(0.5f, 0.5f));
            }
            else
            {
                ti.spriteImportMode = SpriteImportMode.Single;
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
                if (!p.StartsWith(ArtRoot) || !p.EndsWith(".sheet.json")) continue;
                var png = p.Substring(0, p.Length - ".sheet.json".Length) + ".png";
                if (File.Exists(png)) AssetDatabase.ImportAsset(png, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
