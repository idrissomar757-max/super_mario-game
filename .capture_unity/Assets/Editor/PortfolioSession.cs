using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PortfolioSession
{
    [StructLayout(LayoutKind.Sequential)]
    private struct WindowRect { public int left, top, right, bottom; }
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out WindowRect rect);
    private static string Root { get { return Path.GetFullPath(Path.Combine(Application.dataPath, "../../Communication_LinkedIn")); } }
    private static string Flag { get { return Path.Combine(Root, "preuves/editor_session.active"); } }
    private static double _next;
    private static bool _started;

    static PortfolioSession()
    {
        EditorApplication.update += Update;
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "PortfolioSession.Start") >= 0 && !File.Exists(Flag) && !File.Exists(Path.Combine(Root, "preuves/playmode_done.flag")))
            EditorApplication.delayCall += Start;
    }

    public static void Start()
    {
        if (File.Exists(Flag) || EditorApplication.isPlaying) return;
        EditorSceneManager.OpenScene("Assets/Scenes/MarioDemo.unity");
        Selection.activeGameObject = GameObject.Find("Player");
        SceneView scene = SceneView.GetWindow<SceneView>();
        scene.in2DMode = true;
        scene.LookAtDirect(new Vector3(10f, 0f, 0f), Quaternion.identity, 24f);
        scene.Repaint();
        File.WriteAllText(Flag, "capture");
        _next = EditorApplication.timeSinceStartup + 6f;
        _started = true;
    }

    private static void Update()
    {
        if (!File.Exists(Flag)) return;
        if (_started && !EditorApplication.isPlaying && EditorApplication.timeSinceStartup >= _next)
        {
            CaptureEditor("10_environnement_unity.png");
            GameObject recorder = new GameObject("Portfolio Runtime Capture");
            recorder.AddComponent<PortfolioRuntimeCapture>();
            File.WriteAllText(Flag, "play");
            _started = false;
            Type gameViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            EditorWindow.GetWindow(gameViewType).Focus();
            EditorApplication.isPlaying = true;
        }
        if (EditorApplication.isPlaying && File.Exists(Path.Combine(Root, "preuves/playmode_done.flag")))
        {
            CaptureEditor("11_environnement_unity_playmode.png");
            File.Delete(Flag);
            EditorApplication.isPlaying = false;
            Debug.Log("PORTFOLIO_PLAYMODE_SUCCESS");
        }
    }

    private static void CaptureEditor(string name)
    {
        try
        {
            IntPtr window = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            WindowRect rect;
            if (window == IntPtr.Zero || !GetWindowRect(window, out rect)) return;
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            if (width < 300 || height < 300) return;
            Color[] pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(new Vector2(rect.left, rect.top), width, height);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(Path.Combine(Root, "visuels/" + name), texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
        }
        catch (Exception exception) { Debug.LogWarning("Editor screenshot unavailable: " + exception.Message); }
    }
}
