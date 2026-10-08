using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectBootstrap
{
    public static class ProjectSaver
    {
        // Invoke with: -executeMethod ProjectBootstrap.ProjectSaver.SaveAll
        public static void SaveAll()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            Debug.Log("[ProjectSaver] Assets imported and saved.");
            EditorApplication.Exit(0);
        }

        // Import + in ra kết quả cắt sprite / thiết lập audio để kiểm tra importer của asset-pipeline.
        // Invoke with: -executeMethod ProjectBootstrap.ProjectSaver.ImportAndReport
        public static void ImportAndReport()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var ti = (TextureImporter)AssetImporter.GetAtPath(path);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
                Debug.Log($"[ImportReport] {path} mode={ti.spriteImportMode} ppu={ti.spritePixelsPerUnit} filter={ti.filterMode} sprites={sprites.Length}");
            }
            foreach (var guid in AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var s = ((AudioImporter)AssetImporter.GetAtPath(path)).defaultSampleSettings;
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                Debug.Log($"[ImportReport] {path} load={s.loadType} fmt={s.compressionFormat} len={clip.length:0.00}s");
            }
            AssetDatabase.SaveAssets();
            EditorApplication.Exit(0);
        }
    }
}
