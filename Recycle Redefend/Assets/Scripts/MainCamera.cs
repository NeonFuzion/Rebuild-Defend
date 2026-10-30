using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    [SerializeField] float speed, zoomSpeed = 10, maxZoom = 2, minZoom = 20;

    float zoomInput;

    Vector3 movementInput;
    Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movementInput * speed * camera.orthographicSize * Time.deltaTime;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + zoomInput * zoomSpeed * Time.deltaTime, maxZoom, minZoom);
    }

    public void HandleMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void HandleZoomInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        zoomInput = -input.y;
    }
}
