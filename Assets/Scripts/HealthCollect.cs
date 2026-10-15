using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HealthCollect : MonoBehaviour
{

    public int heal = 10;
    public UnityEvent healthPickup;
    void OnTriggerEnter2D(Collider2D other)    
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject
                .GetComponent<PlayerHealth>()
                .Heal(heal);
            healthPickup.Invoke();
            Destroy(gameObject);
        }
    }

}
