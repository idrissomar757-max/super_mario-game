using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimation2D : MonoBehaviour
{
    public Sprite idleFrame;
    public Sprite jumpFrame;
    public Sprite[] runFrames;
    public float runFramesPerSecond = 10f;
    public float runThreshold = 0.15f;

    private PlayerController _controller;
    private SpriteRenderer _spriteRenderer;
    private float _animationTimer;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (idleFrame != null)
        {
            _spriteRenderer.sprite = idleFrame;
        }
    }

    private void Update()
    {
        if (!_controller.IsGrounded && jumpFrame != null)
        {
            _spriteRenderer.sprite = jumpFrame;
            return;
        }

        if (runFrames != null && runFrames.Length > 0 && _controller.HorizontalSpeed > runThreshold)
        {
            _animationTimer += Time.deltaTime * runFramesPerSecond;
            int frameIndex = (int)_animationTimer % runFrames.Length;
            _spriteRenderer.sprite = runFrames[frameIndex];
            return;
        }

        _animationTimer = 0f;

        if (idleFrame != null)
        {
            _spriteRenderer.sprite = idleFrame;
        }
    }
}
