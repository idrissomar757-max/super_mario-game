using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string levelTitle = "Niveau Mario 2D";
    public float killY = -7f;

    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsLevelComplete { get; private set; }

    private GUIStyle _labelStyle;
    private GUIStyle _titleStyle;
    private GUIStyle _panelStyle;
    private PlayerController _player;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<PlayerController>();
        }

        if (!IsGameOver && !IsLevelComplete && _player != null && _player.transform.position.y < killY)
        {
            GameOver();
        }

        if (IsGameOver && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)))
        {
            RestartLevel();
        }
    }

    public void AddScore(int amount)
    {
        if (IsGameOver || IsLevelComplete)
        {
            return;
        }

        Score += Mathf.Max(1, amount);
    }

    public void GameOver()
    {
        if (IsGameOver || IsLevelComplete)
        {
            return;
        }

        IsGameOver = true;

        if (_player != null)
        {
            _player.SetInputEnabled(false);
        }

        Time.timeScale = 0f;
    }

    public void LoadNextLevel()
    {
        if (IsGameOver || IsLevelComplete)
        {
            return;
        }

        IsLevelComplete = true;

        if (_player != null)
        {
            _player.SetInputEnabled(false);
        }

        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGUI()
    {
        EnsureGuiStyles();

        GUI.Box(new Rect(12f, 12f, 220f, 76f), string.Empty, _panelStyle);
        GUI.Label(new Rect(24f, 22f, 180f, 24f), levelTitle, _titleStyle);
        GUI.Label(new Rect(24f, 50f, 180f, 20f), "Score : " + Score, _labelStyle);

        if (IsGameOver)
        {
            GUI.Box(new Rect((Screen.width * 0.5f) - 170f, 28f, 340f, 100f), string.Empty, _panelStyle);
            GUI.Label(new Rect((Screen.width * 0.5f) - 140f, 42f, 280f, 28f), "Game Over", _titleStyle);
            GUI.Label(new Rect((Screen.width * 0.5f) - 140f, 76f, 280f, 22f), "Appuie sur R ou Espace pour recommencer.", _labelStyle);
        }
    }

    private void EnsureGuiStyles()
    {
        if (_labelStyle == null)
        {
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.fontSize = 16;
            _labelStyle.normal.textColor = Color.white;
        }

        if (_titleStyle == null)
        {
            _titleStyle = new GUIStyle(_labelStyle);
            _titleStyle.fontSize = 20;
            _titleStyle.fontStyle = FontStyle.Bold;
        }

        if (_panelStyle == null)
        {
            _panelStyle = new GUIStyle(GUI.skin.box);
            _panelStyle.normal.textColor = Color.white;
        }
    }
}
