using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IO.Game.Core;
using IO.Game.Gameplay;
using IO.Game.Player;
using IO.Game.UI;

namespace IO.Game.EditorTools
{
    /// <summary>
    /// Creates starter placeholder sprites, prefabs, and a demo scene from inside Unity Editor.
    /// Run via Tools > IO > Create/Reset Demo Scene and Prefabs.
    /// </summary>
    public static class ProjectBootstrapper
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";
        private const string HeadSpritePath = "Assets/Art/Sprites/placeholder_head.png";
        private const string SegmentSpritePath = "Assets/Art/Sprites/placeholder_segment.png";
        private const string FoodSpritePath = "Assets/Art/Sprites/placeholder_food.png";
        private const string SegmentPrefabPath = "Assets/Prefabs/BodySegment.prefab";
        private const string FoodPrefabPath = "Assets/Prefabs/Food.prefab";
        private const string PlayerPrefabPath = "Assets/Prefabs/PlayerWorm.prefab";

        [MenuItem("Tools/IO/Create-Reset Demo Scene and Prefabs")]
        private static void CreateDemoSceneAndPrefabs()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Create/Reset IO Demo",
                "This creates placeholder sprites, rewrites the three starter prefabs, and replaces Assets/Scenes/Main.unity. Continue only if you have no custom work in those generated files.",
                "Create Demo",
                "Cancel");

            if (!confirmed)
                return;

            EnsureFolder("Assets/Editor");
            EnsureFolder("Assets/Scenes");
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Art");
            EnsureFolder("Assets/Art/Sprites");

            Sprite headSprite = CreateCircleSprite(HeadSpritePath, new Color(0.38f, 0.95f, 0.34f));
            Sprite segmentSprite = CreateCircleSprite(SegmentSpritePath, new Color(0.20f, 0.72f, 0.30f));
            Sprite foodSprite = CreateCircleSprite(FoodSpritePath, new Color(1f, 0.72f, 0.18f));

            GameObject segmentPrefab = CreateSegmentPrefab(segmentSprite);
            GameObject foodPrefab = CreateFoodPrefab(foodSprite);
            GameObject playerPrefab = CreatePlayerPrefab(headSprite, segmentPrefab.transform);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject root = new GameObject("GameRoot");
            root.AddComponent<GameManager>();
            ScoreManager scoreManager = root.AddComponent<ScoreManager>();

            GameObject spawnerObject = new GameObject("FoodSpawner");
            FoodSpawner spawner = spawnerObject.AddComponent<FoodSpawner>();
            SetObjectReference(spawner, "foodPrefab", foodPrefab.GetComponent<FoodItem>());
            SetInt(spawner, "targetFoodCount", 90);
            SetInt(spawner, "maxPoolSize", 160);
            SetVector2(spawner, "arenaSize", new Vector2(32f, 20f));
            SetVector2Int(spawner, "scoreRange", new Vector2Int(1, 4));

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7.5f;
            camera.backgroundColor = new Color(0.045f, 0.09f, 0.16f);
            cameraObject.AddComponent<AudioListener>();
            CameraFollow cameraFollow = cameraObject.AddComponent<CameraFollow>();
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.name = "PlayerWorm";
            player.transform.position = Vector3.zero;
            SetObjectReference(player.GetComponent<WormController>(), "scoreManager", scoreManager);
            SetObjectReference(player.GetComponent<PlayerController>(), "worldCamera", camera);
            SetObjectReference(cameraFollow, "target", player.transform);

            CreateBot(playerPrefab, "BotWorm_A", new Vector3(-7f, 4f, 0f), spawner, camera);
            CreateBot(playerPrefab, "BotWorm_B", new Vector3(7f, -4f, 0f), spawner, camera);

            CreateHUD(root.GetComponent<GameManager>(), scoreManager);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            EditorUtility.DisplayDialog(
                "IO Demo Created",
                "The Main scene and starter prefabs were generated. Enter Play mode to try mouse/touch-style steering, keyboard controls, food collection, growth, bot steering, score, pause, and restart. Inspect the Console and test on a phone before treating it as release-ready.",
                "OK");
        }

        private static GameObject CreateSegmentPrefab(Sprite sprite)
        {
            AssetDatabase.DeleteAsset(SegmentPrefabPath);
            GameObject segment = new GameObject("BodySegment");
            SpriteRenderer renderer = segment.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 3;

            CircleCollider2D collider = segment.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.43f;
            segment.AddComponent<WormSegment>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(segment, SegmentPrefabPath);
            Object.DestroyImmediate(segment);
            return prefab;
        }

        private static GameObject CreateFoodPrefab(Sprite sprite)
        {
            AssetDatabase.DeleteAsset(FoodPrefabPath);
            GameObject food = new GameObject("Food");
            SpriteRenderer renderer = food.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 1;

            CircleCollider2D collider = food.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.38f;
            food.AddComponent<FoodItem>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(food, FoodPrefabPath);
            Object.DestroyImmediate(food);
            return prefab;
        }

        private static GameObject CreatePlayerPrefab(Sprite sprite, Transform segmentPrefab)
        {
            AssetDatabase.DeleteAsset(PlayerPrefabPath);
            GameObject player = new GameObject("PlayerWorm");
            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 5;

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.44f;

            player.AddComponent<PlayerController>();
            WormController worm = player.AddComponent<WormController>();
            SetObjectReference(worm, "segmentPrefab", segmentPrefab);
            SetInt(worm, "startingSegments", 5);
            SetFloat(worm, "segmentSpacing", 0.38f);
            SetInt(worm, "maximumSegments", 500);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(player, PlayerPrefabPath);
            Object.DestroyImmediate(player);
            return prefab;
        }

        private static void CreateBot(GameObject playerPrefab, string botName, Vector3 position, FoodSpawner spawner, Camera camera)
        {
            GameObject bot = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            bot.name = botName;
            bot.transform.position = position;

            SetObjectReference(bot.GetComponent<PlayerController>(), "worldCamera", camera);
            WormAIController ai = bot.AddComponent<WormAIController>();
            SetObjectReference(ai, "movement", bot.GetComponent<PlayerController>());
            SetObjectReference(ai, "worm", bot.GetComponent<WormController>());
            SetObjectReference(ai, "foodSpawner", spawner);

            WormController worm = bot.GetComponent<WormController>();
            SetInt(worm, "startingSegments", 3);
        }

        private static void CreateHUD(GameManager gameManager, ScoreManager scoreManager)
        {
            GameObject canvasObject = new GameObject("GameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            Text currentScore = CreateText(canvas.transform, "CurrentScore", "Score: 0",
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(32f, -28f),
                new Vector2(350f, 70f), 34, TextAnchor.UpperLeft);
            Text highScore = CreateText(canvas.transform, "HighScore", "Best: 0",
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(32f, -90f),
                new Vector2(350f, 60f), 27, TextAnchor.UpperLeft);

            ScoreHUD scoreHud = canvasObject.AddComponent<ScoreHUD>();
            SetObjectReference(scoreHud, "scoreManager", scoreManager);
            SetObjectReference(scoreHud, "currentScoreText", currentScore);
            SetObjectReference(scoreHud, "highScoreText", highScore);

            Button pauseButton = CreateButton(canvas.transform, "PauseButton", "PAUSE",
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-105f, -55f),
                new Vector2(170f, 76f), new Color(0.10f, 0.22f, 0.31f));

            GameObject panel = new GameObject("GameStatePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.015f, 0.035f, 0.06f, 0.88f);

            Text statusText = CreateText(panel.transform, "StateTitle", "Paused",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 150f),
                new Vector2(720f, 120f), 64, TextAnchor.MiddleCenter);
            Button resumeButton = CreateButton(panel.transform, "ResumeButton", "RESUME",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f),
                new Vector2(340f, 90f), new Color(0.10f, 0.42f, 0.23f));
            Button restartButton = CreateButton(panel.transform, "RestartButton", "RESTART",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -105f),
                new Vector2(340f, 90f), new Color(0.10f, 0.22f, 0.31f));

            GameFlowUI flow = canvasObject.AddComponent<GameFlowUI>();
            SetObjectReference(flow, "gameManager", gameManager);
            SetObjectReference(flow, "statusPanel", panel);
            SetObjectReference(flow, "statusText", statusText);
            SetObjectReference(flow, "pauseButton", pauseButton);
            SetObjectReference(flow, "resumeButton", resumeButton);
            SetObjectReference(flow, "restartButton", restartButton);
            panel.SetActive(false);

            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            InputSystemUIInputModule uiModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            uiModule.AssignDefaultActions();
        }

        private static Button CreateButton(Transform parent, string objectName, string label,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.color = color;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            Text labelText = CreateText(buttonObject.transform, "Label", label,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28, TextAnchor.MiddleCenter);
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;
            labelText.raycastTarget = false;
            return button;
        }

        private static Text CreateText(Transform parent, string objectName, string value,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x == 0f ? 0f : anchorMax.x == 1f && anchorMin.x == 1f ? 1f : 0.5f,
                                     anchorMin.y == 1f && anchorMax.y == 1f ? 1f : 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private static Font GetBuiltinFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return font;
        }

        private static Sprite CreateCircleSprite(string assetPath, Color color)
        {
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (existing != null)
                return existing;

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = Path.GetFileNameWithoutExtension(assetPath);
            Color clear = new Color(0f, 0f, 0f, 0f);
            float center = (size - 1) * 0.5f;
            float radius = size * 0.46f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    texture.SetPixel(x, y, dx * dx + dy * dy <= radius * radius ? color : clear);
                }
            }

            texture.Apply();
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string absolutePath = Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
            File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = size;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);

            if (!string.IsNullOrEmpty(parent) && AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning("Could not find serialized field " + propertyName + " on " + target.name);
                return;
            }

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(Object target, string propertyName, int value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
                return;

            property.intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(Object target, string propertyName, float value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
                return;

            property.floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector2(Object target, string propertyName, Vector2 value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
                return;

            property.vector2Value = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector2Int(Object target, string propertyName, Vector2Int value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
                return;

            property.vector2IntValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
