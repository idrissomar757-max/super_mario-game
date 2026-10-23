using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

// Automated smoke scenario for the presentation copy. Teleports are disclosed.
public class PortfolioRuntimeCapture : MonoBehaviour
{
    private string _root;
    private StringBuilder _results = new StringBuilder();

    private IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        _root = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Communication_LinkedIn"));
        _results.AppendLine("Unity Play Mode smoke scenario; automated repositioning; not a performance benchmark.");
        yield return new WaitForSecondsRealtime(1.5f);
        PlayerController player = FindObjectOfType<PlayerController>();
        GameManager manager = FindObjectOfType<GameManager>();
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        _results.AppendLine("Initial grounded state: " + player.IsGrounded);
        _results.AppendLine("Initial score: " + manager.Score);
        yield return Capture("06_jeu_hud_playmode.png");

        // Keep the original trigger/collision logic; move the test subject only.
        player.SetInputEnabled(false);
        player.enabled = false;
        body.gravityScale = 0f;
        body.velocity = Vector2.zero;
        Coin coin = GameObject.Find("Coin_04").GetComponent<Coin>();
        body.position = coin.transform.position;
        yield return new WaitForSecondsRealtime(0.8f);
        _results.AppendLine("Coin collection score: " + manager.Score + " (expected 1)");
        _results.AppendLine("Coins remaining: " + FindObjectsOfType<Coin>().Length + " (expected 13)");
        yield return Capture("07_collecte_score_playmode.png");

        EnemyController enemy = GameObject.Find("Enemy_01").GetComponent<EnemyController>();
        body.position = enemy.transform.position;
        yield return new WaitForSecondsRealtime(0.6f);
        _results.AppendLine("Enemy contact GameOver: " + manager.IsGameOver + " (expected True)");
        _results.AppendLine("GameOver timeScale: " + Time.timeScale + " (expected 0)");
        yield return Capture("08_game_over_playmode.png");

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MarioDemo");
        yield return new WaitForSecondsRealtime(0.5f);
        player = FindObjectOfType<PlayerController>();
        manager = FindObjectOfType<GameManager>();
        _results.AppendLine("Reload score: " + manager.Score + " (expected 0)");
        _results.AppendLine("Reload GameOver: " + manager.IsGameOver + " (expected False)");
        player.SetInputEnabled(false);
        player.enabled = false;
        body = player.GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.velocity = Vector2.zero;
        body.position = new Vector2(-10f, -8f);
        yield return new WaitForSecondsRealtime(0.3f);
        _results.AppendLine("Falling below killY GameOver: " + manager.IsGameOver + " (expected True)");

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MarioDemo");
        yield return new WaitForSecondsRealtime(0.5f);
        player = FindObjectOfType<PlayerController>();
        player.SetInputEnabled(false);
        player.enabled = false;
        body = player.GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.velocity = Vector2.zero;
        body.position = GameObject.Find("EndTrigger").transform.position;
        yield return new WaitForSecondsRealtime(0.8f);
        _results.AppendLine("Goal transition scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " (expected NextLevel)");
        yield return Capture("09_transition_playmode.png");
        File.WriteAllText(Path.Combine(_root, "preuves/verification_playmode.txt"), _results.ToString());
        File.WriteAllText(Path.Combine(_root, "preuves/playmode_done.flag"), "done");
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(Path.Combine(_root, "visuels/" + name));
        yield return new WaitForSecondsRealtime(0.4f);
    }
}
