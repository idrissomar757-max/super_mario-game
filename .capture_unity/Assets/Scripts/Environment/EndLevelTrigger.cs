using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EndLevelTrigger : MonoBehaviour
{
    private bool _triggered;

    private void Awake()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        _triggered = true;

        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LoadNextLevel();
        }
    }
}
