using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Coin : MonoBehaviour
{
    public int scoreValue = 1;
    public float bobHeight = 0.12f;
    public float bobSpeed = 3f;
    public float pulseAmount = 0.06f;

    private Vector3 _startPosition;
    private Vector3 _startScale;
    private bool _collected;

    private void Awake()
    {
        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;

        _startPosition = transform.position;
        _startScale = transform.localScale;
    }

    private void Update()
    {
        float time = Time.time * bobSpeed;
        transform.position = _startPosition + (Vector3.up * Mathf.Sin(time) * bobHeight);
        transform.localScale = _startScale * (1f + (Mathf.Sin(time * 1.35f) * pulseAmount));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        _collected = true;

        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }
}
