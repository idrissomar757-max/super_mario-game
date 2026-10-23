using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : MonoBehaviour
{
    public float leftBoundary = -1f;
    public float rightBoundary = 1f;
    public float moveSpeed = 1.8f;

    private Rigidbody2D _body;
    private SpriteRenderer _renderer;
    private int _direction = 1;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();

        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.gravityScale = 0f;
        _body.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = _body.position;
        float nextX = currentPosition.x + (_direction * moveSpeed * Time.fixedDeltaTime);

        if (nextX <= leftBoundary)
        {
            nextX = leftBoundary;
            _direction = 1;
        }
        else if (nextX >= rightBoundary)
        {
            nextX = rightBoundary;
            _direction = -1;
        }

        _body.MovePosition(new Vector2(nextX, currentPosition.y));
        _renderer.flipX = _direction < 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    private void HandleCollision(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
}
