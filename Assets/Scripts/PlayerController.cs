using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;

    private Rigidbody2D rb;
    private Vector2 movementInput = Vector2.zero;

    private void Start() 
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context) 
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void Update() 
    {    

    }

    void FixedUpdate()
    {
        // Handles player movement
        Vector2 move = new Vector2(movementInput.x, movementInput.y).normalized;
        rb.linearVelocity = playerSpeed * move;
    }
}