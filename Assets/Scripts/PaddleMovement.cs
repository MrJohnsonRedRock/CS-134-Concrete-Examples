using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float boostMultiplier = 1.8f;
    public float minX = -7.5f;   // play-space BOUNDARIES (DGD.6.2.3)
    public float maxX = 7.5f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float speed = moveSpeed;

        // STUDENT TASK (Session 5): boost modifier while Shift is held (if/then).

        Vector3 pos = transform.position + Vector3.right * x * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);   // clamp = enforce boundaries in code
        transform.position = pos;
    }
}