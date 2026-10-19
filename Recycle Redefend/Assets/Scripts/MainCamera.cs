using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    [SerializeField] float speed;

    Vector3 movementInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movementInput * speed * Time.deltaTime;
    }

    public void HandleMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
}
