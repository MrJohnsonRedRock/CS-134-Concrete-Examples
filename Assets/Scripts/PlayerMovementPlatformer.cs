using UnityEngine;

public class PlayerMovementPlatformer : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public Transform groundCheck;     // empty child object at the player's feet
    public LayerMask groundLayer;     // set platforms to a "Ground" layer

    private Rigidbody2D rb;
    private bool isGrounded;          // VOCAB: BOOLEAN — true/false state (DGD.5.1.4)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // COLLISION DETECTION check: is anything on the Ground layer under our feet?
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundLayer);

        // VOCAB: two Boolean conditions joined by && (AND operator) — DGD.5.1.4
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // STUDENT TASK (Session 5): add a SECOND modifier — sprint while Shift is held.
        // HINT: copy the pattern from the top-down script.
    }

    static void Hurt(Enemy e) { e.health = 0; }

}