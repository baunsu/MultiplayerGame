using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector2 bounds;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!(transform.position.x >= bounds.x && transform.position.x <= bounds.y)) {
            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
        }
    }
}
