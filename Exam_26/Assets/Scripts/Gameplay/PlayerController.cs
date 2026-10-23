using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 55f;
    public float deceleration = 70f;
    public float airAcceleration = 40f;
    public float airDeceleration = 45f;

    [Header("Jump")]
    public float jumpForce = 12.5f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    public float fallGravityMultiplier = 2.2f;
    public float lowJumpGravityMultiplier = 1.8f;

    [Header("Ground Check")]
    public LayerMask groundMask = -1;
    public float groundCheckHeight = 0.12f;
    public float groundCheckWidthMultiplier = 0.85f;

    public bool IsGrounded { get; private set; }
    public bool IsInputEnabled { get; private set; }
    public int FacingDirection { get; private set; }

    public Vector2 Velocity
    {
        get { return _body != null ? _body.velocity : Vector2.zero; }
    }

    public float HorizontalSpeed
    {
        get { return _body != null ? Mathf.Abs(_body.velocity.x) : 0f; }
    }

    private Rigidbody2D _body;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    private float _moveInput;
    private float _coyoteCounter;
    private float _jumpBufferCounter;
    private float _baseGravityScale;

    private void Reset()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.gravityScale = 3f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        collider2D.size = new Vector2(0.68f, 1.35f);
        collider2D.offset = new Vector2(0f, -0.02f);
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _baseGravityScale = Mathf.Max(1f, _body.gravityScale);
        FacingDirection = 1;
        IsInputEnabled = true;
    }

    private void Update()
    {
        _moveInput = IsInputEnabled ? Input.GetAxisRaw("Horizontal") : 0f;

        if (IsInputEnabled && Input.GetButtonDown("Jump"))
        {
            _jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (IsGrounded)
        {
            _coyoteCounter = coyoteTime;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        TryJump();
        UpdateFacingDirection();
        ApplyBetterJump();
    }

    private void FixedUpdate()
    {
        UpdateGroundedState();
        UpdateHorizontalMovement();
    }

    public void SetInputEnabled(bool enabled)
    {
        IsInputEnabled = enabled;

        if (!enabled)
        {
            _moveInput = 0f;
            _jumpBufferCounter = 0f;
            _body.velocity = Vector2.zero;
        }
    }

    private void TryJump()
    {
        if (_jumpBufferCounter <= 0f || _coyoteCounter <= 0f)
        {
            return;
        }

        Vector2 velocity = _body.velocity;
        velocity.y = 0f;
        _body.velocity = velocity;
        _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        IsGrounded = false;
        _jumpBufferCounter = 0f;
        _coyoteCounter = 0f;
    }

    private void UpdateHorizontalMovement()
    {
        float targetSpeed = _moveInput * moveSpeed;
        float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f
            ? (IsGrounded ? acceleration : airAcceleration)
            : (IsGrounded ? deceleration : airDeceleration);

        float newSpeed = Mathf.MoveTowards(_body.velocity.x, targetSpeed, accelerationRate * Time.fixedDeltaTime);
        _body.velocity = new Vector2(newSpeed, _body.velocity.y);
    }

    private void ApplyBetterJump()
    {
        if (_body.velocity.y < -0.01f)
        {
            _body.gravityScale = _baseGravityScale * fallGravityMultiplier;
            return;
        }

        if (_body.velocity.y > 0.01f && !Input.GetButton("Jump"))
        {
            _body.gravityScale = _baseGravityScale * lowJumpGravityMultiplier;
            return;
        }

        _body.gravityScale = _baseGravityScale;
    }

    private void UpdateFacingDirection()
    {
        if (_moveInput > 0.01f)
        {
            FacingDirection = 1;
            _spriteRenderer.flipX = false;
        }
        else if (_moveInput < -0.01f)
        {
            FacingDirection = -1;
            _spriteRenderer.flipX = true;
        }
    }

    private void UpdateGroundedState()
    {
        Bounds bounds = _boxCollider.bounds;
        Vector2 checkSize = new Vector2(bounds.size.x * groundCheckWidthMultiplier, groundCheckHeight);
        Vector2 checkCenter = new Vector2(bounds.center.x, bounds.min.y - (groundCheckHeight * 0.5f));
        int mask = groundMask.value == 0 ? Physics2D.DefaultRaycastLayers : groundMask.value;

        IsGrounded = Physics2D.OverlapBox(checkCenter, checkSize, 0f, mask) != null;
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D collider2D = _boxCollider != null ? _boxCollider : GetComponent<BoxCollider2D>();
        if (collider2D == null)
        {
            return;
        }

        Bounds bounds = collider2D.bounds;
        Vector2 checkSize = new Vector2(bounds.size.x * groundCheckWidthMultiplier, groundCheckHeight);
        Vector2 checkCenter = new Vector2(bounds.center.x, bounds.min.y - (groundCheckHeight * 0.5f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(checkCenter, checkSize);
    }
}
