using UnityEngine;

public class ParallaxLayer2D : MonoBehaviour
{
    public Transform cameraTransform;
    [Range(0f, 1f)] public float parallaxX = 0.2f;
    [Range(0f, 1f)] public float parallaxY = 0.05f;

    private Vector3 _startPosition;
    private Vector3 _cameraStartPosition;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        _startPosition = transform.position;

        if (cameraTransform != null)
        {
            _cameraStartPosition = cameraTransform.position;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 delta = cameraTransform.position - _cameraStartPosition;
        transform.position = new Vector3(
            _startPosition.x + (delta.x * parallaxX),
            _startPosition.y + (delta.y * parallaxY),
            _startPosition.z);
    }
}
