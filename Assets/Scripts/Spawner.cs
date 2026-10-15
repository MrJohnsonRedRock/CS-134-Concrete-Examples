using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;  // ARRAY of prefabs
    public Transform[] spawnPoints;     // ARRAY of positions

    void Start()
    {
        // FOR LOOP — runs once per spawn point
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomIndex = Random.Range(0, prefabsToSpawn.Length);
            Instantiate(
                prefabsToSpawn[randomIndex],
                spawnPoints[i].position,
                Quaternion.identity
            ); // INSTANTIATE = create at runtime
        }
    }
}
