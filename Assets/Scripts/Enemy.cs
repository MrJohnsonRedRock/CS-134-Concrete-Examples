using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float speed = 2.5f;
    public float detectionRange = 5f;

    public int health = 100; 

    void Update()
    {
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance < detectionRange)
        {
            // STATE: CHASING
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
        // else — STATE: IDLE (do nothing)
    }
}
