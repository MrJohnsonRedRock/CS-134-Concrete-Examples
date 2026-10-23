// Session 16 — static Means No this
// Concept: static means no hidden this parameter. No this, no object, so a static
//          member belongs to the type itself, and lives as long as the program does.
// Watch:   the HUD. Set Step in the Inspector to match the slide, then press Play.
using UnityEngine;

public class Session16 : MonoBehaviour
{
    // Each step matches an "In the project: Step N" line in the Session 16 slides.
    [SerializeField] [Range(1, 7)] private int step = 1;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spacing = 1.5f;

    // Session 16: static, so it survives Stop and Play in this project (see the
    // "Stopping isn't ending" slide). It counts Plays until the scripts recompile.
    private static int playNumber = 0;

    private GameObject[] spawned = new GameObject[10];
    private float destroyAt = -1f;
    private float measureAt = -1f;
    private float memoryAtStart = 0f;

    // Step 7
    private string[] testMembers = new string[]
    {
        "Mathf.Abs",
        "Enemy.health",
        "Counter.staticCount",
        "PlayerStats.Heal",
        "Enemy.speed",
        "GameManager.instance",
    };
    private string[] testAnswers = new string[]
    {
        "static. There's no 'which Mathf': the absolute value of -5 is 5, always.",
        "instance. Fifty enemies, fifty healths.",
        "static. One count for the whole program, shared by every Counter.",
        "instance. Heal WHICH player? It needs a this.",
        "instance. Two enemies could move at different speeds.",
        "static. One GameManager for the whole game, but it's a root. Keep it on purpose.",
    };
    private int revealed = 0;

    void Start()
    {
        playNumber++;

        if (step == 1)
        {
            ThereIsntOne();
        }
        else if (step == 2)
        {
            DecodeTheError();
        }
        else if (step == 3)
        {
            OnePerObjectOnePerType();
        }
        else if (step == 4)
        {
            TheConstructorForTheType();
        }
        else if (step == 5)
        {
            SoThisIsALeak();
        }
        else if (step == 6)
        {
            LetGo();
        }
        else if (step == 7)
        {
            TheTest();
        }
    }

    void Update()
    {
        if (step == 1)
        {
            DemoHUD.Show("Time.deltaTime", Time.deltaTime);
        }

        if (destroyAt > 0f && Time.time >= destroyAt)
        {
            DestroyAllSpawned();
        }

        if (measureAt > 0f && Time.time >= measureAt)
        {
            measureAt = -1f;
            if (step == 5)
            {
                MeasureLeaky();
            }
            else if (step == 6)
            {
                MeasureTidy();
            }
        }

        if (step == 7 && Input.GetKeyDown(KeyCode.Return))
        {
            RevealNext();
        }
    }

    // ---------- Step 1: There isn't one ----------

    private void ThereIsntOne()
    {
        BeginStep("There isn't one",
                  "You never made a Mathf. So which Mathf did Mathf.Abs(-5) run on?");

        // No new Mathf() anywhere in this project. The call works anyway.
        DemoHUD.Show("Mathf.Abs(-5)", Mathf.Abs(-5));

        // An instance method needs an object to be called on. That object becomes this.
        PlayerStats stats = new PlayerStats();
        stats.Heal();
        DemoHUD.Show("stats.Heal(), then stats.Health", stats.Health);
    }

    // ---------- Step 2: And it explains this error ----------

    private void DecodeTheError()
    {
        BeginStep("Decode the error",
                  "A static method sets health. Whose health?");

        DemoHUD.Show("Try", "Stop Play. Open PlayerStats.cs and uncomment FullHeal.");
        DemoHUD.Show("Then", "Save. Read the Console. What 'object reference' is it asking for?");
        DemoHUD.Show("Last", "Comment it back out. Nothing runs until the project compiles.");
    }

    // ---------- Step 3: Do it ----------

    private void OnePerObjectOnePerType()
    {
        BeginStep("One per object, one per type",
                  "Three new Counters. What's c.instanceCount? What's Counter.staticCount?");

        Counter c = BuildThreeCounters();

        DemoHUD.Show("c.instanceCount", c.instanceCount);
        DemoHUD.Show("Counter.staticCount", Counter.staticCount);
        DemoHUD.Show("Then", "Uncomment the line in BuildThreeCounters (Session16.cs). Read the Console.");
        DemoHUD.Show("Last", "Comment it back out before the next step.");
    }

    // ---------- Step 4: Static constructor ----------

    private void TheConstructorForTheType()
    {
        BeginStep("The constructor for the type",
                  "Stop, then press Play again. Does static Counter() run again? What's staticCount?");

        Counter c = BuildThreeCounters();

        DemoHUD.Show("Play number (static)", playNumber);
        DemoHUD.Show("c.instanceCount", c.instanceCount);
        DemoHUD.Show("Counter.staticCount", Counter.staticCount);
    }

    private Counter BuildThreeCounters()
    {
        Counter a = new Counter();
        Counter b = new Counter();
        Counter c = new Counter();

        // Uncomment me. Then read the Console.
        // DemoHUD.Show("c.staticCount", c.staticCount);

        return c;
    }

    // ---------- Step 5: So this is a leak ----------

    private void SoThisIsALeak()
    {
        BeginStep("So this is a leak",
                  "The enemies get destroyed. Does LeakyEnemy.allEnemies let go of them?");

        memoryAtStart = MemoryAfterFullGC();
        for (int i = 0; i < spawned.Length; i++)
        {
            spawned[i] = SpawnBody("Leaky " + i, i);
            spawned[i].AddComponent<LeakyEnemy>();
        }
        DemoHUD.Log("Spawned 10 LeakyEnemies. Destroying them in 2 seconds...");
        destroyAt = Time.time + 2f;
    }

    private void MeasureLeaky()
    {
        DemoHUD.Show("Play number (static)", playNumber);
        DemoHUD.Show("Still in LeakyEnemy.allEnemies", LeakyEnemy.allEnemies.Count);

        if (LeakyEnemy.allEnemies.Count > 0)
        {
            LeakyEnemy first = LeakyEnemy.allEnemies[0];
            DemoHUD.Show("allEnemies[0] == null  (Unity's answer)", first == null);
            DemoHUD.Show("allEnemies[0].Health  (C# reads it anyway)", first.Health);
        }

        DemoHUD.Show("Memory change this Play, after a full GC (MB)", MemoryAfterFullGC() - memoryAtStart);
    }

    // ---------- Step 6: The fix ----------

    private void LetGo()
    {
        BeginStep("Let go",
                  "Same enemies, same Destroy. TidyEnemy has OnDestroy. What's left in its list?");

        memoryAtStart = MemoryAfterFullGC();
        for (int i = 0; i < spawned.Length; i++)
        {
            spawned[i] = SpawnBody("Tidy " + i, i);
            spawned[i].AddComponent<TidyEnemy>();
        }
        DemoHUD.Log("Spawned 10 TidyEnemies. Destroying them in 2 seconds...");
        destroyAt = Time.time + 2f;
    }

    private void MeasureTidy()
    {
        DemoHUD.Show("Play number (static)", playNumber);
        DemoHUD.Show("Still in TidyEnemy.allEnemies", TidyEnemy.allEnemies.Count);
        DemoHUD.Show("Memory change this Play, after a full GC (MB)", MemoryAfterFullGC() - memoryAtStart);
    }

    // ---------- Step 7: The test ----------

    private void TheTest()
    {
        BeginStep("The test",
                  "Would new on this type ever make sense? Decide each one, THEN press Enter.");

        for (int i = 0; i < testMembers.Length; i++)
        {
            DemoHUD.Show(testMembers[i], "static or instance?");
        }
    }

    private void RevealNext()
    {
        if (revealed >= testMembers.Length) { return; }

        DemoHUD.Show(testMembers[revealed], testAnswers[revealed]);
        revealed++;
    }

    // ---------- Helpers ----------

    private void BeginStep(string slideTitle, string prediction)
    {
        DemoHUD.SetTitle("Session 16 · Step " + step + " of 7: " + slideTitle);
        DemoHUD.Show("Predict", prediction);
    }

    private GameObject SpawnBody(string bodyName, int index)
    {
        Vector2 spawnAt = (Vector2)transform.position + new Vector2(index * spacing, 2f);
        GameObject body;

        if (enemyPrefab != null)
        {
            body = Instantiate(enemyPrefab, spawnAt, Quaternion.identity);

            // A prefab can't point at the Player in the scene, so wire it here.
            Enemy enemy = body.GetComponent<Enemy>();
            GameObject player = GameObject.FindWithTag("Player");
            if (enemy != null && player != null)
            {
                enemy.player = player.transform;
            }
            else if (enemy != null)
            {
                enemy.enabled = false;   // no player to chase; don't throw every frame
            }
        }
        else
        {
            // No prefab assigned: an invisible body is enough for the numbers.
            body = new GameObject();
            body.transform.position = spawnAt;
        }

        body.name = bodyName;
        return body;
    }

    private void DestroyAllSpawned()
    {
        destroyAt = -1f;
        for (int i = 0; i < spawned.Length; i++)
        {
            if (spawned[i] != null)
            {
                Destroy(spawned[i]);
            }
            // Let go of our own arrows too, so the only thing left holding an
            // enemy is the static list.
            spawned[i] = null;
        }
        DemoHUD.Log("Destroyed all 10.");

        // Destroy finishes at the end of the frame; measure a moment later.
        measureAt = Time.time + 0.5f;
    }

    private float MemoryAfterFullGC()
    {
        // true = run a full collection first, so what's left is only what's still reachable.
        long bytes = System.GC.GetTotalMemory(true);
        return bytes / (1024f * 1024f);
    }
}
