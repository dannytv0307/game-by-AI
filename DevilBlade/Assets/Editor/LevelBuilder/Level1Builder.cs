using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevilBlade;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace DevilBlade.EditorTools
{
    /// <summary>
    /// Dựng toàn bộ màn 1 (Làng đổ nát → đấu trường boss) từ code, để màn chơi tái tạo được và luôn khớp asset mới nhất.
    /// Chạy: menu DevilBlade/Build Level 1, hoặc headless -executeMethod DevilBlade.EditorTools.Level1Builder.BuildHeadless
    /// Bố cục màn chơi nằm trong phần "LEVEL DATA" bên dưới (đơn vị: ô 1x1 = 32px).
    /// </summary>
    public static class Level1Builder
    {
        public const string ScenePath = "Assets/Scenes/Level1.unity";
        public const string ScenePathHD = "Assets/Scenes/Level1_HD.unity";
        const string UnlitMat = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat";

        // ---------------- SKIN: cùng bố cục màn, khác bộ đồ họa ----------------
        class Skin
        {
            public string Name, ScenePath, ArtRoot, TilesDir, TilesSheet;
            public int[] GroundTop, GroundFill, Platform, Decor, Gate;
            public int CheckpointTile;
            public Vector3 CheckpointOffset, CheckpointScale = Vector3.one;
            public bool PixelPerfect, ProceduralMotion;
            public Color GroundTint = Color.white;
            public (string sprite, float fx, float fy, float baseY, int order, Color tint, float scale)[] Backgrounds;
            public System.Func<List<SpriteAnimator.Clip>> Player;
            public System.Func<Dictionary<string, (Sprite[] f, float fps, bool loop)>> Imp, Hound, Boss;
        }

        static Skin S; // skin đang dựng

        static Sprite[] Repeat(Sprite[] s, int n) => Enumerable.Repeat(s[0], n).ToArray();

        static readonly Skin Pixel = new()
        {
            Name = "Pixel", ScenePath = ScenePath, ArtRoot = "Assets/Art/",
            TilesDir = "Assets/Tiles/Village/", TilesSheet = "Tiles/Village/village_tiles.png",
            GroundTop = new[] { 0, 1, 2 }, GroundFill = new[] { 6, 7 }, Platform = new[] { 18, 19 },
            Decor = new[] { 9, 10, 11 }, Gate = new[] { 4, 5 }, CheckpointTile = 23, CheckpointOffset = new Vector3(0, 0.5f, 0),
            PixelPerfect = true,
            Backgrounds = new[]
            {
                ("Backgrounds/Village/village_bg_far.png", 0.9f, 0.95f, 6f, -100, new Color(0.45f, 0.45f, 0.58f), 1.35f), // phóng to để luôn phủ kín khung hình
                ("Backgrounds/Village/village_bg_mid.png", 0.6f, 0.75f, 5f, -90, new Color(0.62f, 0.6f, 0.72f), 1f),
            },
            Player = () =>
            {
                var jump = Sheet("Characters/Hero/hero_jump.png");
                return new List<SpriteAnimator.Clip>
                {
                    Clip("idle", Sheet("Characters/Hero/hero_idle.png"), 6),
                    Clip("run", Sheet("Characters/Hero/hero_run.png"), 12),
                    Clip("jump", new[] { jump[Mathf.Min(1, jump.Length - 1)] }, 1),
                    Clip("fall", new[] { jump[jump.Length - 1] }, 1),
                    Clip("attack", Sheet("Characters/Hero/hero_attack1.png"), 14, false),
                    Clip("hurt", Sheet("Characters/Hero/hero_hurt.png"), 8, false),
                    Clip("transform", Sheet("Characters/Hero/hero_transform.png"), 8, false),
                    Clip("d_idle", Sheet("Characters/HeroDemon/hero_demon_idle.png"), 7),
                    Clip("d_attack", Sheet("Characters/HeroDemon/hero_demon_attack.png"), 15, false),
                };
            },
            Imp = () => new()
            {
                ["run"] = (Sheet("Enemies/Imp/imp_run.png"), 12, true),
                ["attack"] = (Sheet("Enemies/Imp/imp_attack.png"), 10, false),
            },
            Hound = () =>
            {
                var run = Sheet("Enemies/Hellhound/hellhound_run.png");
                return new() { ["idle"] = (new[] { run[0] }, 1, true), ["run"] = (run, 14, true) };
            },
            Boss = () => new() { ["idle"] = (Sheet("Enemies/DemonKnight/demon_knight_idle.png"), 5, true) },
        };

        // HD: phần lớn là tư thế tĩnh (AI vẽ từng frame chân thật không đồng nhất) + chuyển động thủ tục (ProceduralPoseMotion).
        // Số frame lặp lại để giữ đúng thời điểm ra đòn (attackHitFrame) như bản pixel.
        static readonly Skin HD = new()
        {
            Name = "HD", ScenePath = ScenePathHD, ArtRoot = "Assets/ArtHD/",
            TilesDir = "Assets/Tiles/VillageHD/", TilesSheet = "Tiles/VillageHD/village_hd_tiles.png",
            GroundTop = new[] { 0 }, GroundFill = new[] { 4 }, Platform = new[] { 2, 3 }, GroundTint = new Color(0.62f, 0.6f, 0.66f), // 1 biến thể + tối bớt => đỡ lộ đường nối ô
            Decor = new int[0], Gate = new[] { 2, 3 }, CheckpointTile = 6,
            CheckpointOffset = new Vector3(0, 0.5f, 0), CheckpointScale = new Vector3(1.4f, 1.4f, 1),
            ProceduralMotion = true,
            Backgrounds = new[]
            {
                ("Backgrounds/VillageHD/village_hd_bg.png", 0.85f, 0.9f, 6.5f, -100, new Color(0.62f, 0.62f, 0.7f), 1.35f),
            },
            Player = () =>
            {
                var idle = Sheet("Characters/HeroHD/hero_hd_idle.png");
                var tr = Sheet("Characters/HeroHD/hero_hd_transform_pose.png");
                var dIdle = Sheet("Characters/HeroDemonHD/hero_demon_hd_idle_pose.png");
                return new List<SpriteAnimator.Clip>
                {
                    Clip("idle", idle, 3),
                    Clip("run", Sheet("Characters/HeroHD/hero_hd_run.png"), 8),
                    Clip("jump", Sheet("Characters/HeroHD/hero_hd_jump_pose.png"), 1),
                    Clip("fall", Sheet("Characters/HeroHD/hero_hd_jump_pose.png"), 1),
                    Clip("attack", Repeat(Sheet("Characters/HeroHD/hero_hd_attack_pose.png"), 5), 14, false),
                    Clip("hurt", Repeat(Sheet("Characters/HeroHD/hero_hd_hurt_pose.png"), 2), 8, false),
                    Clip("transform", new[] { idle[0] }.Concat(Repeat(tr, 4)).Concat(dIdle).ToArray(), 8, false),
                    Clip("d_idle", dIdle, 1),
                    Clip("d_attack", Repeat(Sheet("Characters/HeroDemonHD/hero_demon_hd_attack_pose.png"), 5), 15, false),
                };
            },
            Imp = () => new()
            {
                ["run"] = (Sheet("Enemies/ImpHD/imp_hd_run_pose.png"), 1, true),
                ["attack"] = (Repeat(Sheet("Enemies/ImpHD/imp_hd_attack_pose.png"), 4), 10, false),
            },
            Hound = () =>
            {
                var run = Sheet("Enemies/HellhoundHD/hellhound_hd_run_pose.png");
                return new() { ["idle"] = (run, 1, true), ["run"] = (run, 1, true) };
            },
            Boss = () => new() { ["idle"] = (Sheet("Enemies/DemonKnightHD/demon_knight_hd_idle_pose.png"), 1, true) },
        };

        // ---------------- LEVEL DATA ----------------
        // Đoạn nền đất: (x bắt đầu, x kết thúc — bao gồm, độ cao mặt đất)
        static readonly (int x0, int x1, int h)[] GroundSegments =
        {
            (-16, -1, 16),  // tường trái (dày để camera không bao giờ thấy khoảng trống)
            (0, 30, 2),     // A: khởi đầu
            (34, 55, 3),    // B: sau vực 1
            (59, 80, 2),    // C: sau vực 2
            (81, 83, 3),    // bậc lên
            (84, 96, 4),    // D: gờ cao trước đấu trường
            (97, 131, 2),   // đấu trường boss
            (132, 148, 18), // tường phải
        };
        // Platform một chiều: (x bắt đầu, x kết thúc, hàng ô — mặt trên ở y = hàng + 1)
        static readonly (int x0, int x1, int y)[] Platforms =
        {
            (40, 44, 5), (62, 65, 4), (68, 71, 6), (86, 89, 6), (106, 109, 5), (119, 122, 5),
        };
        // Gạch vụn trang trí (không va chạm): x trên mặt đất
        static readonly int[] Rubble = { 6, 19, 27, 38, 49, 61, 74, 85, 100, 113, 127 };
        static readonly Vector2 PlayerSpawn = new(3f, 2f);
        static readonly Vector2[] Imps = { new(14, 2), new(24, 2), new(46, 3), new(49, 3), new(72, 2), new(88, 4), new(91, 4) };
        static readonly Vector2[] Hounds = { new(50, 3), new(77, 2) };
        static readonly Vector2[] Checkpoints = { new(35.5f, 3), new(94.5f, 4) };
        static readonly Vector2 BossSpawn = new(125, 2);
        static readonly Vector2[] SummonPoints = { new(110, 4), new(117, 4) };
        const float GateX = 96.5f;
        const float ArenaTriggerX = 101f;
        static readonly Rect CameraBounds = new(-4, -3, 140, 24);

        /// <summary>Dựng cả hai bản: Level1_HD (cảnh 0, mở đầu tiên) và Level1 (pixel, cảnh 1). Tab trong game để đổi.</summary>
        [MenuItem("DevilBlade/Build Level 1 (Pixel + HD)")]
        public static void Build()
        {
            Build(Pixel);
            Build(HD);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePathHD, true), // HD mở trước
                new EditorBuildSettingsScene(ScenePath, true),
            };
            AssetDatabase.SaveAssets();
        }

        static void Build(Skin skin)
        {
            S = skin;
            SetupLayers();
            SetupPlayerSettings();
            var white = EnsureWhiteSprite();
            var tiles = EnsureTiles();
            var unlit = AssetDatabase.LoadAssetAtPath<Material>(UnlitMat);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            BuildAudio();
            BuildCamera();
            BuildBackgrounds(unlit);
            BuildTilemaps(tiles, unlit);
            var player = BuildPlayer(unlit);
            BuildEnemies(unlit, out var boss);
            BuildCheckpoints(tiles, unlit);
            BuildArena(boss, tiles, unlit);
            BuildKillZone();
            BuildCinemachine(player.transform);
            var hud = BuildHUD(player, white);

            var gm = new GameObject("GameManager").AddComponent<GameManager>();
            gm.player = player;
            gm.hud = hud;

            EditorSceneManager.SaveScene(scene, skin.ScenePath);
            Debug.Log($"[Level1Builder] Đã dựng {skin.ScenePath} ({skin.Name})");
        }

        public static void BuildHeadless()
        {
            try
            {
                Build();
                EditorApplication.Exit(0);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }

        // ================= thiết lập dự án =================
        static void SetupLayers()
        {
            var tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tm.FindProperty("layers");
            layers.GetArrayElementAtIndex(Layers.Ground).stringValue = "Ground";
            layers.GetArrayElementAtIndex(Layers.Player).stringValue = "Player";
            layers.GetArrayElementAtIndex(Layers.Enemy).stringValue = "Enemy";
            tm.ApplyModifiedProperties();
        }

        static void SetupPlayerSettings()
        {
            PlayerSettings.productName = "DevilBlade";
            PlayerSettings.companyName = "DevilBlade";
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = false;
        }

        static Sprite EnsureWhiteSprite()
        {
            const string path = "Assets/Art/UI/white.png"; // UI dùng chung cho mọi skin
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var tex = new Texture2D(4, 4);
                tex.SetPixels(Enumerable.Repeat(Color.white, 16).ToArray());
                File.WriteAllBytes(path, tex.EncodeToPNG());
                AssetDatabase.ImportAsset(path);
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        static Sprite[] Sheet(string path)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(S.ArtRoot + path).OfType<Sprite>()
                .OrderBy(s => int.Parse(s.name.Substring(s.name.LastIndexOf('_') + 1))).ToArray();
            if (sprites.Length == 0) throw new System.Exception($"Không có sprite trong {path} — đã chạy asset-pipeline approve chưa?");
            return sprites;
        }

        static Tile[] EnsureTiles()
        {
            Directory.CreateDirectory(S.TilesDir);
            var sprites = Sheet(S.TilesSheet);
            var tiles = new Tile[sprites.Length];
            for (var i = 0; i < sprites.Length; i++)
            {
                var path = $"{S.TilesDir}village_{i:00}.asset";
                var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
                if (tile == null)
                {
                    tile = ScriptableObject.CreateInstance<Tile>();
                    AssetDatabase.CreateAsset(tile, path);
                }
                tile.sprite = sprites[i];
                tile.colliderType = Tile.ColliderType.Grid;
                EditorUtility.SetDirty(tile);
                tiles[i] = tile;
            }
            return tiles;
        }

        // ================= âm thanh =================
        static AudioManager BuildAudio()
        {
            AudioClip A(string p) => AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + p);
            var am = new GameObject("AudioManager").AddComponent<AudioManager>();
            am.musicLevel = A("Music/music_village.wav");
            am.musicBoss = A("Music/music_boss.wav");
            am.slash = A("SFX/sfx_slash.wav");
            am.hit = A("SFX/sfx_hit.wav");
            am.jump = A("SFX/sfx_jump.wav");
            am.land = A("SFX/sfx_land.wav");
            am.hurt = A("SFX/sfx_hurt.wav");
            am.rageFull = A("SFX/sfx_rage_full.wav");
            am.transformBurst = A("SFX/sfx_transform.wav");
            am.enemyDie = A("SFX/sfx_enemy_die.wav");
            am.voiceIntro = A("Voice/vo_hero_intro.wav");
            am.voiceTransform = A("Voice/vo_hero_transform.wav");
            am.voiceBossTaunt = A("Voice/vo_boss_taunt.wav");
            return am;
        }

        // ================= camera =================
        static Camera BuildCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            go.transform.position = new Vector3(PlayerSpawn.x, PlayerSpawn.y + 3, -10);
            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.625f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color32(0x1f, 0x1a, 0x24, 255);
            go.AddComponent<AudioListener>();
            go.AddComponent<UniversalAdditionalCameraData>();
            if (S.PixelPerfect)
            {
                var ppc = go.AddComponent<PixelPerfectCamera>();
                ppc.assetsPPU = 32;
                ppc.refResolutionX = 640;
                ppc.refResolutionY = 360;
            }
            go.AddComponent<CinemachineBrain>();
            return cam;
        }

        static void BuildCinemachine(Transform target)
        {
            var go = new GameObject("CM FollowCam");
            var vcam = go.AddComponent<CinemachineCamera>();
            vcam.Follow = target;
            var lens = vcam.Lens;
            lens.OrthographicSize = 5.625f;
            lens.NearClipPlane = 0.1f;
            lens.FarClipPlane = 100f;
            vcam.Lens = lens;
            var composer = go.AddComponent<CinemachinePositionComposer>();
            composer.TargetOffset = new Vector3(0, 1.6f, 0);
            composer.Damping = new Vector3(0.35f, 0.6f, 0);

            var bounds = new GameObject("CameraBounds");
            var poly = bounds.AddComponent<PolygonCollider2D>();
            poly.isTrigger = true;
            var r = CameraBounds;
            poly.points = new[] { new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax) };
            bounds.layer = 2; // Ignore Raycast — không ảnh hưởng gameplay
            var confiner = go.AddComponent<CinemachineConfiner2D>();
            confiner.BoundingShape2D = poly;
            if (S.PixelPerfect) go.AddComponent<CinemachinePixelPerfect>();
        }

        // ================= nền =================
        static void BuildBackgrounds(Material mat)
        {
            void Layer(string name, string sprite, float fx, float fy, float baseY, int order, Color tint, float scale)
            {
                var root = new GameObject(name);
                var p = root.AddComponent<Parallax>();
                var s = AssetDatabase.LoadAssetAtPath<Sprite>(S.ArtRoot + sprite);
                p.width = s.bounds.size.x * scale;
                p.factorX = fx;
                p.factorY = fy;
                p.baseY = baseY;
                root.transform.position = new Vector3(0, baseY, 0);
                for (var i = -1; i <= 1; i++)
                {
                    var c = new GameObject($"{name}_{i + 1}");
                    c.transform.SetParent(root.transform, false);
                    c.transform.localPosition = new Vector3(i * p.width, 0, 0);
                    c.transform.localScale = new Vector3(scale, scale, 1);
                    var sr = c.AddComponent<SpriteRenderer>();
                    sr.sprite = s;
                    sr.sortingOrder = order;
                    sr.sharedMaterial = mat;
                    sr.color = tint; // tối & lạnh hơn để nhân vật nổi bật trên nền
                }
            }
            for (var i = 0; i < S.Backgrounds.Length; i++)
            {
                var b = S.Backgrounds[i];
                Layer($"BG_{i}", b.sprite, b.fx, b.fy, b.baseY, b.order, b.tint, b.scale);
            }
        }

        // ================= tilemap =================
        static GameObject BuildTilemaps(Tile[] t, Material mat)
        {
            var grid = new GameObject("Grid");
            grid.AddComponent<Grid>();

            Tilemap Map(string name, int order, bool collide, bool oneWay)
            {
                var go = new GameObject(name);
                go.transform.SetParent(grid.transform, false);
                var map = go.AddComponent<Tilemap>();
                var tr = go.AddComponent<TilemapRenderer>();
                tr.sortingOrder = order;
                tr.sharedMaterial = mat;
                if (!collide) return map;
                go.layer = Layers.Ground;
                var rb = go.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
                var tc = go.AddComponent<TilemapCollider2D>();
                tc.compositeOperation = Collider2D.CompositeOperation.Merge;
                var comp = go.AddComponent<CompositeCollider2D>();
                comp.geometryType = CompositeCollider2D.GeometryType.Polygons;
                if (oneWay)
                {
                    comp.usedByEffector = true;
                    var eff = go.AddComponent<PlatformEffector2D>();
                    eff.useOneWay = true;
                    eff.surfaceArc = 160f;
                }
                return map;
            }

            var ground = Map("Ground", 0, true, false);
            ground.color = S.GroundTint;
            var platforms = Map("Platforms", 1, true, true);
            var decor = Map("Decor", 2, false, false);
            var rng = new System.Random(7);

            foreach (var (x0, x1, h) in GroundSegments)
                for (var x = x0; x <= x1; x++)
                    for (var y = -8; y < h; y++)
                    {
                        var top = y == h - 1;
                        var tile = top ? t[Pick(S.GroundTop, rng)] : t[Pick(S.GroundFill, rng)];
                        ground.SetTile(new Vector3Int(x, y, 0), tile);
                    }
            foreach (var (x0, x1, y) in Platforms)
                for (var x = x0; x <= x1; x++)
                    platforms.SetTile(new Vector3Int(x, y, 0), t[Pick(S.Platform, rng)]);
            foreach (var x in S.Decor.Length > 0 ? Rubble : new int[0])
            {
                var h = GroundSegments.First(s => x >= s.x0 && x <= s.x1).h;
                decor.SetTile(new Vector3Int(x, h, 0), t[Pick(S.Decor, rng)]);
            }
            // Bắt buộc sinh hình va chạm ngay: nếu lưu scene khi composite còn rỗng, nhân vật sẽ rơi xuyên đất.
            foreach (var tc in grid.GetComponentsInChildren<TilemapCollider2D>()) tc.ProcessTilemapChanges();
            foreach (var comp in grid.GetComponentsInChildren<CompositeCollider2D>())
            {
                comp.GenerateGeometry();
                if (comp.pathCount == 0) throw new System.Exception($"{comp.name}: composite collider rỗng");
            }
            return grid;
        }

        static int Pick(int[] options, System.Random rng) => options[rng.Next(0, options.Length)];

        // ================= nhân vật =================
        static SpriteAnimator.Clip Clip(string name, Sprite[] frames, float fps, bool loop = true) =>
            new() { name = name, frames = frames, fps = fps, loop = loop };

        static GameObject Body(string name, Vector2 pos, int layer, Material mat, int order, Vector2 colSize, out SpriteAnimator anim)
        {
            var go = new GameObject(name) { layer = layer };
            go.transform.position = pos;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3.5f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var col = go.AddComponent<CapsuleCollider2D>();
            col.sharedMaterial = NoFriction(); // không dính vào vách tường khi giữ phím di chuyển
            col.size = colSize;
            col.offset = new Vector2(0, colSize.y / 2f + 0.02f);
            var sprite = new GameObject("Sprite") { layer = layer };
            sprite.transform.SetParent(go.transform, false);
            var sr = sprite.AddComponent<SpriteRenderer>();
            sr.sharedMaterial = mat;
            sr.sortingOrder = order;
            anim = sprite.AddComponent<SpriteAnimator>();
            if (S.ProceduralMotion) sprite.AddComponent<ProceduralPoseMotion>();
            go.AddComponent<HitFlash>();
            return go;
        }

        static PhysicsMaterial2D NoFriction()
        {
            const string path = "Assets/Settings/NoFriction.physicsMaterial2D";
            var m = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
            if (m != null) return m;
            m = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
            AssetDatabase.CreateAsset(m, path);
            return m;
        }

        static PlayerController BuildPlayer(Material mat)
        {
            var go = Body("Player_Kael", PlayerSpawn, Layers.Player, mat, 10, new Vector2(0.7f, 1.75f), out var anim);
            anim.clips = S.Player();
            anim.GetComponent<SpriteRenderer>().sprite = anim.clips[0].frames[0];
            var health = go.AddComponent<Health>();
            health.max = 100;
            health.invulnerableAfterHit = 1.0f;
            return go.AddComponent<PlayerController>();
        }

        static T Enemy<T>(string name, Vector2 pos, Material mat, Vector2 col, int hp, Dictionary<string, (Sprite[] f, float fps, bool loop)> clips)
            where T : EnemyBase
        {
            var go = Body(name, pos, Layers.Enemy, mat, 5, col, out var anim);
            anim.clips = clips.Select(kv => Clip(kv.Key, kv.Value.f, kv.Value.fps, kv.Value.loop)).ToList();
            anim.GetComponent<SpriteRenderer>().sprite = anim.clips[0].frames[0];
            var health = go.AddComponent<Health>();
            health.max = hp;
            var e = go.AddComponent<T>();
            e.contactSize = new Vector2(col.x + 0.1f, col.y);
            e.contactOffset = new Vector2(0, col.y / 2f);
            return e;
        }

        static ImpEnemy CreateImp(Vector2 pos, Material mat, Transform parent)
        {
            var imp = Enemy<ImpEnemy>("Imp", pos, mat, new Vector2(0.8f, 0.95f), 30, S.Imp());
            imp.rageReward = 4;
            imp.transform.SetParent(parent, true);
            return imp;
        }

        static void BuildEnemies(Material mat, out DemonKnightBoss boss)
        {
            var root = new GameObject("Enemies").transform;
            foreach (var p in Imps) CreateImp(p, mat, root);
            foreach (var p in Hounds)
            {
                var h = Enemy<HellhoundEnemy>("Hellhound", p, mat, new Vector2(1.6f, 1.0f), 50, S.Hound());
                h.rageReward = 8;
                h.transform.SetParent(root, true);
                h.GetComponentInChildren<SpriteRenderer>().flipX = true; // nhìn về phía người chơi đi tới
            }

            boss = Enemy<DemonKnightBoss>("Boss_DemonKnight", BossSpawn, mat, new Vector2(1.5f, 3.0f), 420, S.Boss());
            boss.contactDamage = 12;
            boss.GetComponent<Rigidbody2D>().mass = 20f;
            boss.GetComponentInChildren<SpriteRenderer>().flipX = true;
            boss.transform.SetParent(root, true);

            var template = CreateImp(new Vector2(-50, -50), mat, root);
            template.name = "Imp_SummonTemplate";
            template.gameObject.SetActive(false);
            boss.impTemplate = template;
            boss.summonPoints = SummonPoints.Select((p, i) =>
            {
                var t = new GameObject($"SummonPoint_{i}").transform;
                t.position = p;
                t.SetParent(root, true);
                return t;
            }).ToArray();
        }

        // ================= checkpoint / đấu trường / vực =================
        static void BuildCheckpoints(Tile[] t, Material mat)
        {
            foreach (var p in Checkpoints)
            {
                var go = new GameObject("Checkpoint_Shrine");
                go.transform.position = p;
                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2(1.5f, 2.5f);
                box.offset = new Vector2(0, 1.25f);
                go.AddComponent<Checkpoint>();
                var s = new GameObject("Sprite");
                s.transform.SetParent(go.transform, false);
                s.transform.localPosition = S.CheckpointOffset;
                s.transform.localScale = S.CheckpointScale;
                var sr = s.AddComponent<SpriteRenderer>();
                sr.sprite = t[S.CheckpointTile].sprite;
                sr.sharedMaterial = mat;
                sr.sortingOrder = 3;
            }
        }

        static void BuildArena(DemonKnightBoss boss, Tile[] t, Material mat)
        {
            var gate = new GameObject("ArenaGate") { layer = Layers.Ground };
            gate.transform.position = new Vector3(GateX, 4f, 0);
            var col = gate.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 9f);
            col.offset = new Vector2(0, 4.5f);
            for (var i = 0; i < 9; i++)
            {
                var piece = new GameObject($"Plank_{i}");
                piece.transform.SetParent(gate.transform, false);
                piece.transform.localPosition = new Vector3(0, i + 0.5f, 0);
                var sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite = t[S.Gate[i % S.Gate.Length]].sprite;
                sr.sharedMaterial = mat;
                sr.sortingOrder = 4;
            }

            var trigger = new GameObject("BossArenaTrigger");
            trigger.transform.position = new Vector3(ArenaTriggerX, 2f, 0);
            var box = trigger.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(3f, 12f);
            box.offset = new Vector2(0, 6f);
            var arena = trigger.AddComponent<BossArena>();
            arena.boss = boss;
            arena.gate = gate;
        }

        static void BuildKillZone()
        {
            var go = new GameObject("KillZone");
            go.transform.position = new Vector3(65, -5f, 0);
            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(220, 2);
            go.AddComponent<KillZone>();
        }

        // ================= HUD =================
        static HUD BuildHUD(PlayerController player, Sprite white)
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvasGo = new GameObject("HUD", typeof(RectTransform));
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(640, 360);
            scaler.matchWidthOrHeight = 0.5f;
            var hud = canvasGo.AddComponent<HUD>();
            hud.player = player;

            RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size)
            {
                var go = new GameObject(name, typeof(RectTransform));
                var rt = (RectTransform)go.transform;
                rt.SetParent(parent, false);
                rt.anchorMin = rt.anchorMax = anchor;
                rt.pivot = anchor;
                rt.anchoredPosition = pos;
                rt.sizeDelta = size;
                return rt;
            }
            Image Img(RectTransform rt, Color c, bool filled = false)
            {
                var img = rt.gameObject.AddComponent<Image>();
                img.sprite = white;
                img.color = c;
                if (filled)
                {
                    img.type = Image.Type.Filled;
                    img.fillMethod = Image.FillMethod.Horizontal;
                }
                return img;
            }
            Text Txt(RectTransform rt, string s, int size, TextAnchor align, Color c)
            {
                var t = rt.gameObject.AddComponent<Text>();
                t.font = font;
                t.text = s;
                t.fontSize = size;
                t.alignment = align;
                t.color = c;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                rt.gameObject.AddComponent<Outline>().effectColor = new Color(0, 0, 0, 0.8f);
                return t;
            }
            Image Bar(string name, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size, Color fill)
            {
                var bg = Rect(name, parent, anchor, pos, size);
                Img(bg, new Color32(0x12, 0x0d, 0x16, 230));
                var f = Rect("Fill", bg, new Vector2(0, 0.5f), new Vector2(1, 0), size - new Vector2(2, 2));
                return Img(f, fill, true);
            }

            var tl = new Vector2(0, 1);
            Txt(Rect("HPLabel", canvasGo.transform, tl, new Vector2(10, -8), new Vector2(60, 12)), "KAEL", 10, TextAnchor.UpperLeft, Color.white);
            hud.healthFill = Bar("HealthBar", canvasGo.transform, tl, new Vector2(10, -22), new Vector2(150, 9), new Color32(0xd4, 0x28, 0x3a, 255));
            hud.rageFill = Bar("RageBar", canvasGo.transform, tl, new Vector2(10, -34), new Vector2(150, 6), new Color32(0xe0, 0x66, 0x1f, 255));
            hud.rageLabel = Txt(Rect("RageLabel", canvasGo.transform, tl, new Vector2(164, -31), new Vector2(80, 12)), "NỘ KHÍ", 8, TextAnchor.UpperLeft, new Color32(0xff, 0x9b, 0x2f, 255));
            hud.message = Txt(Rect("Message", canvasGo.transform, new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(600, 20)), "", 13, TextAnchor.MiddleCenter, Color.white);

            var bossPanel = Rect("BossPanel", canvasGo.transform, new Vector2(0.5f, 0), new Vector2(0, 14), new Vector2(320, 26));
            Txt(Rect("BossName", bossPanel, new Vector2(0.5f, 1), Vector2.zero, new Vector2(320, 12)), "FALLEN DEMON KNIGHT", 10, TextAnchor.UpperCenter, new Color32(0xb3, 0xbc, 0xcb, 255));
            hud.bossFill = Bar("BossBar", bossPanel, new Vector2(0.5f, 0), Vector2.zero, new Vector2(320, 8), new Color32(0x7c, 0x3a, 0x91, 255));
            hud.bossPanel = bossPanel.gameObject;

            var pause = Rect("PausePanel", canvasGo.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(2000, 2000));
            Img(pause, new Color(0, 0, 0, 0.6f));
            Txt(Rect("PauseText", pause, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400, 60)), "TẠM DỪNG\n\nEsc: tiếp tục    R: chơi lại", 14, TextAnchor.MiddleCenter, Color.white);
            hud.pausePanel = pause.gameObject;

            var victory = Rect("VictoryPanel", canvasGo.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(2000, 2000));
            Img(victory, new Color(0.05f, 0.02f, 0.05f, 0.7f));
            Txt(Rect("VictoryTitle", victory, new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(500, 40)), "MÀN 1 HOÀN THÀNH", 26, TextAnchor.MiddleCenter, new Color32(0xff, 0xd4, 0x6b, 255));
            hud.victoryStats = Txt(Rect("VictoryStats", victory, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(500, 50)), "", 11, TextAnchor.MiddleCenter, Color.white);
            hud.victoryPanel = victory.gameObject;

            var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
            es.transform.SetParent(null);
            return hud;
        }
    }
}
