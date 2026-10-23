using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Capture utility for the isolated presentation copy only.
public static class PortfolioCapture
{
    private static string Output { get { return Path.GetFullPath(Path.Combine(Application.dataPath, "../../Communication_LinkedIn")); } }

    public static void Generate()
    {
        try
        {
            Directory.CreateDirectory(Path.Combine(Output, "visuels"));
            SuperMarioDemoSceneBuilder.CreateDemoScene();
            Camera camera = Camera.main;
            Capture(camera, "01_depart_camera_unity.png", new Vector3(-8f, 1f, -10f), 4.5f, 1920, 1080);
            Capture(camera, "02_plateformes_camera_unity.png", new Vector3(8f, 1f, -10f), 4.5f, 1920, 1080);
            Capture(camera, "03_arrivee_camera_unity.png", new Vector3(26.5f, 1f, -10f), 4.5f, 1920, 1080);
            Capture(camera, "04_vue_ensemble_camera_unity.png", new Vector3(10f, 0.5f, -10f), 7.2f, 3200, 1000);
            WriteInventory();
            EditorSceneManager.OpenScene("Assets/Scenes/NextLevel.unity");
            Capture(Camera.main, "05_fond_transition_camera_unity.png", new Vector3(0f, 0f, -10f), 4.5f, 1920, 1080);
            EditorSceneManager.OpenScene("Assets/Scenes/MarioDemo.unity");
            Debug.Log("PORTFOLIO_CAPTURE_SUCCESS: " + Output);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void Capture(Camera camera, string file, Vector3 position, float size, int width, int height)
    {
        camera.transform.position = position;
        camera.orthographicSize = size;
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        target.antiAliasing = 1;
        camera.targetTexture = target;
        RenderTexture.active = target;
        camera.Render();
        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();
        File.WriteAllBytes(Path.Combine(Output, "visuels/" + file), image.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.DestroyImmediate(image);
        target.Release();
        UnityEngine.Object.DestroyImmediate(target);
    }

    private static void WriteInventory()
    {
        StringBuilder text = new StringBuilder();
        text.AppendLine("Unity: " + Application.unityVersion);
        text.AppendLine("Graphics device: " + SystemInfo.graphicsDeviceName);
        text.AppendLine("Mode: edit-time camera renders, not runtime gameplay screenshots.");
        text.AppendLine("HUD/OnGUI is not included in Camera.Render output.");
        text.AppendLine("Scene generated with the project's original scene builder in an isolated copy.");
        text.AppendLine("Coin components: " + UnityEngine.Object.FindObjectsOfType<Coin>().Length);
        text.AppendLine("Enemy components: " + UnityEngine.Object.FindObjectsOfType<EnemyController>().Length);
        text.AppendLine("Parallax components: " + UnityEngine.Object.FindObjectsOfType<ParallaxLayer2D>().Length);
        text.AppendLine("Sprite renderers: " + UnityEngine.Object.FindObjectsOfType<SpriteRenderer>().Length);
        text.AppendLine("BoxCollider2D components (includes disabled): " + UnityEngine.Object.FindObjectsOfType<BoxCollider2D>().Length);
        text.AppendLine("Scene hierarchy:");
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            WriteTransform(text, root.transform, 0);
        File.WriteAllText(Path.Combine(Output, "preuves/inventaire_scene_unity.txt"), text.ToString());
    }

    private static void WriteTransform(StringBuilder text, Transform transform, int depth)
    {
        text.AppendLine(new string(' ', depth * 2) + transform.name);
        foreach (Transform child in transform) WriteTransform(text, child, depth + 1);
    }
}
