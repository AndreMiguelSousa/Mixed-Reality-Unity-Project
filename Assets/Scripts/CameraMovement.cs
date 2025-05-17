using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 200f;
    public float rotationSpeed = 100f;

    public float minY = 10f;
    public float maxY = 100f;

    public Vector2 xLimits = new Vector2(-50, 50);
    public Vector2 zLimits = new Vector2(-50, 50);


    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
        ClampPosition();
    }

    void HandleMovement()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            dir += transform.forward;
        if (Input.GetKey(KeyCode.S))
            dir -= transform.forward;
        if (Input.GetKey(KeyCode.D))
            dir += transform.right;
        if (Input.GetKey(KeyCode.A))
            dir -= transform.right;
        if (Input.GetKey(KeyCode.Space))
            dir += transform.up;
        if (Input.GetKey(KeyCode.LeftShift))
            dir -= transform.up;
        dir.y = 0f;
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * zoomSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float Xrotate = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float Yrotate = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Yrotate, Xrotate, 0f, Space.World);
        }
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
        pos.z = Mathf.Clamp(pos.z, zLimits.x, zLimits.y);
        transform.position = pos;
    }
}
