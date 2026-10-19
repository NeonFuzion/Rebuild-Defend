using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10;

    Vector2 movementInput;
    Rigidbody2D rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rigidbody.linearVelocity = movementInput * movementSpeed;
    }

    public void HandleMovementInput(Vector2 input)
    {
        movementInput = input;
    }
}
