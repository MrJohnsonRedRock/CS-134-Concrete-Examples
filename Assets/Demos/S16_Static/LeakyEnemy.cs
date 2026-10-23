// Session 16 — static Means No this
// Concept: a static field is a GC root. Whatever it points at stays reachable.
// Watch:   the HUD rows after the enemies are destroyed, then after the next Play.
//
// Wrong on purpose: an enemy adds itself to a static list and never takes itself
// off. Left this way so the leak can be measured. TidyEnemy is the fix.
using System.Collections.Generic;
using UnityEngine;

public class LeakyEnemy : MonoBehaviour
{
    // Session 16: ONE list for the whole program, not one per enemy.
    public static List<LeakyEnemy> allEnemies = new List<LeakyEnemy>();

    [SerializeField] private int health = 100;

    // A megabyte of extra data per enemy, so a leak is big enough to see on the HUD.
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
}
