// Session 16 — static Means No this
// Concept: a static list is a root, so an object has to take itself off before it dies.
// Watch:   the same HUD rows as the leak, after the enemies are destroyed.
using System.Collections.Generic;
using UnityEngine;

public class TidyEnemy : MonoBehaviour
{
    // Session 16: ONE list for the whole program, not one per enemy.
    public static List<TidyEnemy> allEnemies = new List<TidyEnemy>();

    [SerializeField] private int health = 100;

    // A megabyte of extra data per enemy, so a leak would be big enough to see.
    private int[] payload = new int[262144];

    public int Health
    {
        get { return health; }
    }

    void Start()
    {
        payload[0] = health;
        allEnemies.Add(this);
    }

    // The one difference from LeakyEnemy. Destroy makes garbage, but only if
    // nothing reachable still holds an arrow to it, and this list is a root.
    void OnDestroy()
    {
        allEnemies.Remove(this);
    }
}
