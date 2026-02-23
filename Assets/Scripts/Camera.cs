using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private BoxCollider2D boundsCollider;

    [SerializeField] private float zOffset = -10f;
    private Camera cam;
    private float halfHeight;
    private float halfWidth;
    private Bounds bounds;

    private void Start()
    {
        cam = GetComponent<Camera>();

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        if (boundsCollider != null)
            bounds = boundsCollider.bounds;
    }

    private void LateUpdate()
    {
        if (!target || !boundsCollider) return;

        bounds = boundsCollider.bounds;

        Vector3 desired = new Vector3(
            target.position.x,
            target.position.y,
            zOffset
        );

        float clampedX = Mathf.Clamp(
            desired.x,
            bounds.min.x + halfWidth,
            bounds.max.x - halfWidth
        );

        float clampedY = Mathf.Clamp(
            desired.y,
            bounds.min.y + halfHeight,
            bounds.max.y - halfHeight
        );

        transform.position = new Vector3(clampedX, clampedY, zOffset);
    }
}