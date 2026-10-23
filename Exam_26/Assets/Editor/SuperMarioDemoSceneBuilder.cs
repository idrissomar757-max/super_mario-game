using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SuperMarioDemoSceneBuilder
{
    private const string MainScenePath = "Assets/Scenes/MarioDemo.unity";
    private const string NextScenePath = "Assets/Scenes/NextLevel.unity";
    private const string TileFolder = "Assets/Art/KenneyPixelPlatformer/Tiles/";
    private const string BackgroundFolder = "Assets/Art/KenneyPixelPlatformer/Tiles/Backgrounds/";
    private const string CharacterFolder = "Assets/Art/KenneyPixelPlatformer/Tiles/Characters/";
    private const string PlayerFolder = "Assets/Art/OpenGameArt/PlayerFrames/";

    private static readonly string[] TileSprites =
    {
        TileFolder + "tile_0000.png",
        TileFolder + "tile_0001.png",
        TileFolder + "tile_0002.png",
        TileFolder + "tile_0009.png",
        TileFolder + "tile_0010.png",
        TileFolder + "tile_0030.png",
        TileFolder + "tile_0060.png",
        TileFolder + "tile_0061.png",
        TileFolder + "tile_0062.png",
        TileFolder + "tile_0084.png",
        TileFolder + "tile_0085.png",
        TileFolder + "tile_0097.png",
        TileFolder + "tile_0127.png",
        TileFolder + "tile_0130.png",
        TileFolder + "tile_0131.png",
        TileFolder + "tile_0152.png"
    };

    private static readonly string[] BackgroundSprites =
    {
        BackgroundFolder + "tile_0000.png",
        BackgroundFolder + "tile_0008.png",
        BackgroundFolder + "tile_0012.png",
        BackgroundFolder + "tile_0014.png"
    };

    private static readonly string[] CharacterSprites =
    {
        CharacterFolder + "tile_0018.png",
        CharacterFolder + "tile_0019.png"
    };

    private static readonly string[] PlayerSprites =
    {
        PlayerFolder + "player_idle.png",
        PlayerFolder + "player_run_1.png",
        PlayerFolder + "player_run_2.png",
        PlayerFolder + "player_run_3.png",
        PlayerFolder + "player_run_4.png",
        PlayerFolder + "player_jump.png"
    };

    [MenuItem("Tools/Super Mario 2D/Create Demo Scene")]
    public static void CreateDemoScene()
    {
        EnsureFolders();
        ConfigureSpriteImports();
        CreateMainLevelScene();
        CreateNextLevelScene();
        ConfigureBuildSettings();
        EditorSceneManager.OpenScene(MainScenePath);
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScenePath);
        Debug.Log("Scenes creees: " + MainScenePath + " et " + NextScenePath);
    }

    private static void EnsureFolders()
    {
        string scenesDirectory = Path.Combine(Application.dataPath, "Scenes");
        if (!Directory.Exists(scenesDirectory))
        {
            Directory.CreateDirectory(scenesDirectory);
        }
    }

    private static void ConfigureSpriteImports()
    {
        ConfigureSpriteGroup(TileSprites, 18f);
        ConfigureSpriteGroup(BackgroundSprites, 18f);
        ConfigureSpriteGroup(CharacterSprites, 18f);
        ConfigureSpriteGroup(PlayerSprites, 16f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ConfigureSpriteGroup(string[] paths, float pixelsPerUnit)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            TextureImporter importer = AssetImporter.GetAtPath(paths[i]) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }

    private static void CreateMainLevelScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject worldRoot = new GameObject("World");

        GameObject systemsRoot = new GameObject("Systems");
        systemsRoot.transform.SetParent(worldRoot.transform, false);
        CreateGameManager(systemsRoot.transform);

        GameObject backgroundRoot = new GameObject("Background");
        backgroundRoot.transform.SetParent(worldRoot.transform, false);
        CreateBackground(backgroundRoot.transform);

        GameObject geometryRoot = new GameObject("Geometry");
        geometryRoot.transform.SetParent(worldRoot.transform, false);
        CreateGround(geometryRoot.transform);
        CreatePlatforms(geometryRoot.transform);
        CreateBlocks(geometryRoot.transform);
        CreateDecorations(geometryRoot.transform);

        GameObject coinsRoot = new GameObject("Coins");
        coinsRoot.transform.SetParent(worldRoot.transform, false);
        CreateCoins(coinsRoot.transform);

        GameObject enemiesRoot = new GameObject("Enemies");
        enemiesRoot.transform.SetParent(worldRoot.transform, false);
        CreateEnemies(enemiesRoot.transform);

        GameObject goalRoot = new GameObject("Goal");
        goalRoot.transform.SetParent(worldRoot.transform, false);
        CreateGoal(goalRoot.transform);

        GameObject player = CreatePlayer(worldRoot.transform);
        CreateCamera(player.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, MainScenePath);
    }

    private static void CreateNextLevelScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4.5f;
        camera.backgroundColor = new Color(0.35f, 0.72f, 0.95f, 1f);
        camera.clearFlags = CameraClearFlags.SolidColor;

        cameraObject.AddComponent<AudioListener>();

        GameObject backgroundRoot = new GameObject("Background");
        CreateBackgroundSprite(backgroundRoot.transform, "Sky", BackgroundFolder + "tile_0000.png", new Vector3(0f, 0f, 0f), new Vector3(24f, 14f, 1f), -30, false, 0f, 0f);
        CreateBackgroundSprite(backgroundRoot.transform, "Mountains", BackgroundFolder + "tile_0012.png", new Vector3(0f, -0.1f, 0f), new Vector3(6f, 6f, 1f), -20, false, 0f, 0f);
        CreateBackgroundSprite(backgroundRoot.transform, "Hills", BackgroundFolder + "tile_0014.png", new Vector3(0f, -0.8f, 0f), new Vector3(6f, 6f, 1f), -10, false, 0f, 0f);

        GameObject screen = new GameObject("NextLevelScreen");
        screen.AddComponent<NextLevelScreen>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, NextScenePath);
    }

    private static void ConfigureBuildSettings()
    {
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(MainScenePath, true),
            new EditorBuildSettingsScene(NextScenePath, true)
        };
    }

    private static void CreateGameManager(Transform parent)
    {
        GameObject manager = new GameObject("GameManager");
        manager.transform.SetParent(parent, false);

        GameManager gameManager = manager.AddComponent<GameManager>();
        gameManager.levelTitle = "Niveau Mario 2D";
        gameManager.killY = -7f;
    }

    private static void CreateBackground(Transform parent)
    {
        CreateBackgroundSprite(parent, "Sky", BackgroundFolder + "tile_0000.png", new Vector3(10f, 2f, 0f), new Vector3(52f, 16f, 1f), -50, false, 0f, 0f);

        CreateBackgroundSprite(parent, "Cloud_A", BackgroundFolder + "tile_0008.png", new Vector3(-7f, 4.7f, 0f), new Vector3(4f, 4f, 1f), -40, true, 0.08f, 0.04f);
        CreateBackgroundSprite(parent, "Cloud_B", BackgroundFolder + "tile_0008.png", new Vector3(6f, 4.1f, 0f), new Vector3(3.5f, 3.5f, 1f), -40, true, 0.08f, 0.04f);
        CreateBackgroundSprite(parent, "Cloud_C", BackgroundFolder + "tile_0008.png", new Vector3(19f, 4.5f, 0f), new Vector3(4.2f, 4.2f, 1f), -40, true, 0.08f, 0.04f);
        CreateBackgroundSprite(parent, "Cloud_D", BackgroundFolder + "tile_0008.png", new Vector3(31f, 4.2f, 0f), new Vector3(3.3f, 3.3f, 1f), -40, true, 0.08f, 0.04f);

        CreateBackgroundSprite(parent, "Mountains_A", BackgroundFolder + "tile_0012.png", new Vector3(-8f, 0.8f, 0f), new Vector3(6.2f, 6.2f, 1f), -30, true, 0.18f, 0.08f);
        CreateBackgroundSprite(parent, "Mountains_B", BackgroundFolder + "tile_0012.png", new Vector3(4f, 0.8f, 0f), new Vector3(6.2f, 6.2f, 1f), -30, true, 0.18f, 0.08f);
        CreateBackgroundSprite(parent, "Mountains_C", BackgroundFolder + "tile_0012.png", new Vector3(16f, 0.8f, 0f), new Vector3(6.2f, 6.2f, 1f), -30, true, 0.18f, 0.08f);
        CreateBackgroundSprite(parent, "Mountains_D", BackgroundFolder + "tile_0012.png", new Vector3(28f, 0.8f, 0f), new Vector3(6.2f, 6.2f, 1f), -30, true, 0.18f, 0.08f);

        CreateBackgroundSprite(parent, "Hills_A", BackgroundFolder + "tile_0014.png", new Vector3(-8f, -0.1f, 0f), new Vector3(6.2f, 6.2f, 1f), -20, true, 0.28f, 0.12f);
        CreateBackgroundSprite(parent, "Hills_B", BackgroundFolder + "tile_0014.png", new Vector3(4f, -0.1f, 0f), new Vector3(6.2f, 6.2f, 1f), -20, true, 0.28f, 0.12f);
        CreateBackgroundSprite(parent, "Hills_C", BackgroundFolder + "tile_0014.png", new Vector3(16f, -0.1f, 0f), new Vector3(6.2f, 6.2f, 1f), -20, true, 0.28f, 0.12f);
        CreateBackgroundSprite(parent, "Hills_D", BackgroundFolder + "tile_0014.png", new Vector3(28f, -0.1f, 0f), new Vector3(6.2f, 6.2f, 1f), -20, true, 0.28f, 0.12f);
    }

    private static void CreateGround(Transform parent)
    {
        CreateGroundStrip(parent, "Ground_Start", -12, -2, 11, 3);
        CreateGroundStrip(parent, "Ground_MiddleA", 3, -2, 8, 3);
        CreateGroundStrip(parent, "Ground_MiddleB", 14, -2, 7, 3);
        CreateGroundStrip(parent, "Ground_End", 23, -2, 10, 3);
    }

    private static void CreatePlatforms(Transform parent)
    {
        CreatePlatform(parent, "Platform_GapA", -1, 0, 3);
        CreatePlatform(parent, "Platform_GapA_High", 1, 2, 2);
        CreatePlatform(parent, "Platform_MiddleA", 5, 1, 3);
        CreatePlatform(parent, "Platform_MiddleB", 9, 3, 3);
        CreatePlatform(parent, "Platform_GapB_Left", 11, 1, 3);
        CreatePlatform(parent, "Platform_GapB_Top", 15, 3, 3);
        CreatePlatform(parent, "Platform_GapB_Right", 18, 1, 3);
        CreatePlatform(parent, "Platform_End", 26, 1, 4);
    }

    private static void CreateBlocks(Transform parent)
    {
        CreateSolidBlock(parent, "QuestionBlock_01", TileFolder + "tile_0010.png", new Vector3(6f, 3f, 0f), 12);
        CreateSolidBlock(parent, "BrickBlock_01", TileFolder + "tile_0009.png", new Vector3(7f, 3f, 0f), 12);
        CreateSolidBlock(parent, "BrickBlock_02", TileFolder + "tile_0009.png", new Vector3(17f, 4f, 0f), 12);
        CreateSolidBlock(parent, "QuestionBlock_02", TileFolder + "tile_0010.png", new Vector3(18f, 4f, 0f), 12);
    }

    private static void CreateDecorations(Transform parent)
    {
        CreateSpriteOnly(parent, "Sign_Start", TileFolder + "tile_0084.png", new Vector3(-10f, -1f, 0f), Vector3.one, 2);
        CreateSpriteOnly(parent, "Sign_Gap", TileFolder + "tile_0085.png", new Vector3(2.8f, -1f, 0f), Vector3.one, 2);
        CreateSpriteOnly(parent, "Bush_A", TileFolder + "tile_0097.png", new Vector3(8f, -1.1f, 0f), Vector3.one, 2);
        CreateSpriteOnly(parent, "Cactus_A", TileFolder + "tile_0127.png", new Vector3(20f, -1f, 0f), Vector3.one, 2);
        CreateSpriteOnly(parent, "Bush_B", TileFolder + "tile_0097.png", new Vector3(28f, -1.1f, 0f), Vector3.one, 2);
    }

    private static void CreateCoins(Transform parent)
    {
        CreateCoin(parent, "Coin_01", new Vector3(-1f, 1.2f, 0f));
        CreateCoin(parent, "Coin_02", new Vector3(0f, 1.8f, 0f));
        CreateCoin(parent, "Coin_03", new Vector3(1f, 2.4f, 0f));
        CreateCoin(parent, "Coin_04", new Vector3(5.5f, 2.2f, 0f));
        CreateCoin(parent, "Coin_05", new Vector3(7f, 2.2f, 0f));
        CreateCoin(parent, "Coin_06", new Vector3(9.5f, 4.2f, 0f));
        CreateCoin(parent, "Coin_07", new Vector3(11f, 4.2f, 0f));
        CreateCoin(parent, "Coin_08", new Vector3(12f, 2.2f, 0f));
        CreateCoin(parent, "Coin_09", new Vector3(15.5f, 4.2f, 0f));
        CreateCoin(parent, "Coin_10", new Vector3(17f, 4.2f, 0f));
        CreateCoin(parent, "Coin_11", new Vector3(19f, 2.2f, 0f));
        CreateCoin(parent, "Coin_12", new Vector3(24.5f, 0.8f, 0f));
        CreateCoin(parent, "Coin_13", new Vector3(27f, 2.2f, 0f));
        CreateCoin(parent, "Coin_14", new Vector3(29f, 2.2f, 0f));
    }

    private static void CreateEnemies(Transform parent)
    {
        CreateEnemy(parent, "Enemy_01", CharacterFolder + "tile_0018.png", new Vector3(6f, -1.05f, 0f), 4.2f, 8.3f);
        CreateEnemy(parent, "Enemy_02", CharacterFolder + "tile_0019.png", new Vector3(16.5f, -1.05f, 0f), 14.5f, 19.2f);
        CreateEnemy(parent, "Enemy_03", CharacterFolder + "tile_0018.png", new Vector3(27f, -1.05f, 0f), 24.5f, 30f);
    }

    private static void CreateGoal(Transform parent)
    {
        CreateSpriteOnly(parent, "FlagPole", TileFolder + "tile_0131.png", new Vector3(31.2f, -0.3f, 0f), new Vector3(1f, 5f, 1f), 8);

        GameObject trigger = new GameObject("EndTrigger");
        trigger.transform.SetParent(parent, false);
        trigger.transform.localPosition = new Vector3(31.9f, -0.2f, 0f);
        trigger.layer = 2;

        BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1.2f, 2.6f);
        collider.isTrigger = true;

        trigger.AddComponent<EndLevelTrigger>();
    }

    private static GameObject CreatePlayer(Transform parent)
    {
        GameObject player = new GameObject("Player");
        player.transform.SetParent(parent, false);
        player.transform.position = new Vector3(-10.5f, 0f, 0f);

        SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(PlayerFolder + "player_idle.png");
        renderer.sortingOrder = 25;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 3f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D boxCollider = player.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(0.68f, 1.35f);
        boxCollider.offset = new Vector2(0f, -0.02f);

        PlayerController controller = player.AddComponent<PlayerController>();
        controller.groundMask = LayerMask.GetMask("Default");

        PlayerAnimation2D animation = player.AddComponent<PlayerAnimation2D>();
        animation.idleFrame = LoadSprite(PlayerFolder + "player_idle.png");
        animation.jumpFrame = LoadSprite(PlayerFolder + "player_jump.png");
        animation.runFrames = new Sprite[]
        {
            LoadSprite(PlayerFolder + "player_run_1.png"),
            LoadSprite(PlayerFolder + "player_run_2.png"),
            LoadSprite(PlayerFolder + "player_run_3.png"),
            LoadSprite(PlayerFolder + "player_run_4.png")
        };

        return player;
    }

    private static void CreateCamera(Transform target)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(-8f, 1f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4.5f;
        camera.backgroundColor = new Color(0.72f, 0.9f, 1f, 1f);
        camera.clearFlags = CameraClearFlags.SolidColor;

        cameraObject.AddComponent<AudioListener>();

        CameraFollow2D follow = cameraObject.AddComponent<CameraFollow2D>();
        follow.target = target;
        follow.offset = new Vector3(1.7f, 1f, -10f);
        follow.smoothTime = 0.14f;
        follow.horizontalLookAhead = 1.75f;
        follow.verticalLookAhead = 0.35f;
        follow.clampX = true;
        follow.minX = -8f;
        follow.maxX = 29.5f;
        follow.clampY = true;
        follow.minY = -0.5f;
        follow.maxY = 5f;
    }

    private static void CreateGroundStrip(Transform parent, string objectName, int startX, int topY, int length, int depth)
    {
        GameObject root = new GameObject(objectName);
        root.transform.SetParent(parent, false);

        for (int row = 0; row < depth; row++)
        {
            for (int column = 0; column < length; column++)
            {
                string spritePath = TileFolder + "tile_0030.png";

                if (row == 0)
                {
                    if (column == 0)
                    {
                        spritePath = TileFolder + "tile_0000.png";
                    }
                    else if (column == length - 1)
                    {
                        spritePath = TileFolder + "tile_0002.png";
                    }
                    else
                    {
                        spritePath = TileFolder + "tile_0001.png";
                    }
                }

                CreateSpriteOnly(root.transform, string.Format("Tile_{0}_{1}", row, column), spritePath, new Vector3(startX + column, topY - row, 0f), Vector3.one, 0);
            }
        }

        BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(length, depth);
        collider.offset = new Vector2(startX + ((length - 1) * 0.5f), topY - ((depth - 1) * 0.5f));
    }

    private static void CreatePlatform(Transform parent, string objectName, int startX, int tileY, int length)
    {
        GameObject root = new GameObject(objectName);
        root.transform.SetParent(parent, false);

        for (int column = 0; column < length; column++)
        {
            string spritePath = TileFolder + "tile_0061.png";

            if (column == 0)
            {
                spritePath = TileFolder + "tile_0060.png";
            }
            else if (column == length - 1)
            {
                spritePath = TileFolder + "tile_0062.png";
            }

            CreateSpriteOnly(root.transform, string.Format("PlatformTile_{0}", column), spritePath, new Vector3(startX + column, tileY, 0f), Vector3.one, 6);
        }

        BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(length, 0.3f);
        collider.offset = new Vector2(startX + ((length - 1) * 0.5f), tileY + 0.35f);
    }

    private static void CreateSolidBlock(Transform parent, string objectName, string spritePath, Vector3 position, int sortingOrder)
    {
        GameObject block = CreateSpriteOnly(parent, objectName, spritePath, position, Vector3.one, sortingOrder);
        BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.9f, 0.9f);
    }

    private static void CreateCoin(Transform parent, string objectName, Vector3 position)
    {
        GameObject coin = CreateSpriteOnly(parent, objectName, TileFolder + "tile_0152.png", position, Vector3.one, 18);
        coin.layer = 2;

        CircleCollider2D trigger = coin.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.35f;

        Coin collectible = coin.AddComponent<Coin>();
        collectible.scoreValue = 1;
        collectible.bobHeight = 0.12f;
        collectible.bobSpeed = 3f;
    }

    private static void CreateEnemy(Transform parent, string objectName, string spritePath, Vector3 position, float leftBoundary, float rightBoundary)
    {
        GameObject enemy = CreateSpriteOnly(parent, objectName, spritePath, position, Vector3.one, 14);
        enemy.layer = 2;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.freezeRotation = true;

        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 0.75f);
        collider.offset = Vector2.zero;

        EnemyController enemyController = enemy.AddComponent<EnemyController>();
        enemyController.leftBoundary = leftBoundary;
        enemyController.rightBoundary = rightBoundary;
        enemyController.moveSpeed = 1.4f;
    }

    private static GameObject CreateBackgroundSprite(Transform parent, string objectName, string spritePath, Vector3 position, Vector3 scale, int sortingOrder, bool parallaxEnabled, float parallaxX, float parallaxY)
    {
        GameObject spriteObject = CreateSpriteOnly(parent, objectName, spritePath, position, scale, sortingOrder);

        BoxCollider2D collider = spriteObject.AddComponent<BoxCollider2D>();
        collider.enabled = false;
        collider.size = Vector2.one;

        if (parallaxEnabled)
        {
            ParallaxLayer2D parallax = spriteObject.AddComponent<ParallaxLayer2D>();
            parallax.parallaxX = parallaxX;
            parallax.parallaxY = parallaxY;
        }

        return spriteObject;
    }

    private static GameObject CreateSpriteOnly(Transform parent, string objectName, string spritePath, Vector3 position, Vector3 scale, int sortingOrder)
    {
        GameObject spriteObject = new GameObject(objectName);
        spriteObject.transform.SetParent(parent, false);
        spriteObject.transform.localPosition = position;
        spriteObject.transform.localScale = scale;

        SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(spritePath);
        renderer.sortingOrder = sortingOrder;

        return spriteObject;
    }

    private static Sprite LoadSprite(string assetPath)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

        if (sprite == null)
        {
            Debug.LogError("Sprite introuvable: " + assetPath);
        }

        return sprite;
    }
}
