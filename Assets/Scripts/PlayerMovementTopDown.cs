using UnityEngine;

public class PlayerMovementTopDown : MonoBehaviour
{
    // VOCAB: "public" is an ACCESS MODIFIER (DGD.5.2.9).
    // Public variables appear in the Inspector so designers can tune them.
    public float moveSpeed = 5f;            // VOCAB: VARIABLE of PRIMITIVE DATA TYPE float
    public float sprintMultiplier = 1.6f;

    private Rigidbody2D rb;                 // private = only this script can touch it

    void Start()   // runs once when the object enters the scene
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()  // VOCAB: the game loop — runs once per frame
    {
        // VOCAB: reading INPUT from HARDWARE (keyboard) — DGD.5.3.4
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        float speed = moveSpeed;

        // STUDENT TASK (Session 5): make sprint work.
        // VOCAB: IF/THEN statement using a BOOLEAN condition (DGD.5.1.1, 5.1.4)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = moveSpeed * sprintMultiplier;   // VOCAB: ORDER OF OPERATIONS matters here
        }

        rb.linearVelocity = new Vector2(x, y).normalized * speed;
        
    }
}