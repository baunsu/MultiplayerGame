using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    public float jumpHeight = 1.0f;
    public Vector2 bounds;

    private Rigidbody2D rb;
    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool grounded;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        jumped = context.action.triggered;
    }

    private bool isGrounded() {
        return rb.velocity.y == 0;
    }

    void Update() {    
        if (jumped && isGrounded()) {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(playerSpeed * movementInput.x, rb.velocity.y);

        // Prevents player from going off camera
        if (transform.position.x < bounds.x) {
            transform.position = new Vector3(bounds.x, transform.position.y, 0);
        } else if (transform.position.x > bounds.y) {
            transform.position = new Vector3(bounds.y, transform.position.y, 0);
        }
    }
}