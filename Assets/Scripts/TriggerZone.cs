using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    public UnityEvent onPlayerEnter;   // wire ANY action here, no code
    public bool fireOnce = true;
    private bool hasFired = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !(fireOnce && hasFired))
        {
            hasFired = true;
            onPlayerEnter.Invoke();    // the SCRIPTED EVENT fires
        }
    }
}