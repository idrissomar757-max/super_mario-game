using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(1.5f, 1f, -10f);
    public float smoothTime = 0.18f;
    public float horizontalLookAhead = 1.5f;
    public float verticalLookAhead = 0.35f;
    public bool clampX = true;
    public float minX = -10f;
    public float maxX = 10f;
    public bool clampY = true;
    public float minY = -2f;
    public float maxY = 5f;

    private Vector3 _velocity;
    private PlayerController _playerController;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (_playerController == null)
        {
            _playerController = target.GetComponent<PlayerController>();
        }

        Vector3 desiredPosition = target.position + offset;

        if (_playerController != null)
        {
            float lookStrength = Mathf.InverseLerp(0f, _playerController.moveSpeed, _playerController.HorizontalSpeed);
            desiredPosition.x += _playerController.FacingDirection * horizontalLookAhead * lookStrength;

            if (!_playerController.IsGrounded)
            {
                desiredPosition.y += verticalLookAhead;
            }
        }

        desiredPosition.z = offset.z;

        if (clampX)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        if (clampY)
        {
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothTime);
    }
}
