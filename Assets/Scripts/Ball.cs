using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float startSpeed = 7f;
    private Rigidbody2D rb;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Launch();

       

    }

    // VOCAB: a FUNCTION/METHOD we can call from other scripts (DGD.5.1.2)
    public void Launch()
    {
        Vector2 direction = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
        rb.linearVelocity = direction * startSpeed;
    }
    // Requires a Physics Material 2D: friction 0, bounciness 1 (in starter package).
    // STUDENT TASK (Session 20, balancing): ball speed creeps up 5% per paddle hit?
}