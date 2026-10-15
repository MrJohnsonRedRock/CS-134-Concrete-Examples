using UnityEngine;

public class ScoreCollect : MonoBehaviour
{
   
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScoreManager.instance.AddScore(10);
        Destroy(gameObject);
    }
}
