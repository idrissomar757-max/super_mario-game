using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScreen : MonoBehaviour
{
    private GUIStyle _labelStyle;
    private GUIStyle _titleStyle;
    private GUIStyle _panelStyle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        GUI.Box(new Rect((Screen.width * 0.5f) - 200f, (Screen.height * 0.5f) - 80f, 400f, 160f), string.Empty, _panelStyle);
        GUI.Label(new Rect((Screen.width * 0.5f) - 145f, (Screen.height * 0.5f) - 48f, 300f, 30f), "Prochaine scene chargee", _titleStyle);
        GUI.Label(new Rect((Screen.width * 0.5f) - 145f, (Screen.height * 0.5f) - 8f, 300f, 24f), "Le joueur a atteint la fin du niveau.", _labelStyle);
        GUI.Label(new Rect((Screen.width * 0.5f) - 145f, (Screen.height * 0.5f) + 24f, 300f, 24f), "Appuie sur Entree ou Espace pour rejouer.", _labelStyle);
    }

    private void EnsureStyles()
    {
        if (_labelStyle == null)
        {
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.fontSize = 18;
            _labelStyle.alignment = TextAnchor.MiddleCenter;
            _labelStyle.normal.textColor = Color.white;
        }

        if (_titleStyle == null)
        {
            _titleStyle = new GUIStyle(_labelStyle);
            _titleStyle.fontSize = 26;
            _titleStyle.fontStyle = FontStyle.Bold;
        }

        if (_panelStyle == null)
        {
            _panelStyle = new GUIStyle(GUI.skin.box);
        }
    }
}
