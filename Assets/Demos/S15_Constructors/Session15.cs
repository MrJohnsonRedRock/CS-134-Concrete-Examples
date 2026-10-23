// Session 15 — What `new` Actually Calls
// Concept: new means "run a constructor." A constructor is a method named like
//          its type, with no return type, that runs once, when the object is made.
// Watch:   the HUD. Set Step in the Inspector to match the slide, then press Play.
using UnityEngine;

public class Session15 : MonoBehaviour
{
    // Each step matches an "In the project: Step N" line in the Session 15 slides.
    [SerializeField] [Range(1, 8)] private int step = 1;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spacing = 2f;

    void Start()
    {
        if (step == 1)
        {
            TheConstructorYouNeverWrote();
        }
        else if (step == 2)
        {
            TheTrap();
        }
        else if (step == 3)
        {
            StructsAreDifferent();
        }
        else if (step == 4)
        {
            HonestyCheck();
        }
        else if (step == 5)
        {
            OurEnemyCantHaveOne();
        }
        else if (step == 6)
        {
            TheHiddenThis();
        }
        else if (step == 7)
        {
            PredictTheHUD();
        }
        else if (step == 8)
        {
            NeverBroken();
        }
    }

    // ---------- Step 1: So where's the one you never wrote? ----------

    private void TheConstructorYouNeverWrote()
    {
        BeginStep("The constructor you never wrote",
                  "LootDrop.cs has no constructor. What's in each field?");

        // LootDrop.cs has no constructor in it. So what does this call?
        LootDrop drop = new LootDrop(10);

        // Session 7: a reference box nobody filled holds null, not "".
        string shownName = "null";
        if (drop.itemName != null)
        {
            shownName = "\"" + drop.itemName + "\"";
        }

        DemoHUD.Show("itemName (string)", shownName);
        DemoHUD.Show("coins (int)", drop.coins);
        DemoHUD.Show("isRare (bool)", drop.isRare);
    }

    // ---------- Step 2: Now the trap ----------

    private void TheTrap()
    {
        BeginStep("The trap",
                  "Give LootDrop a constructor. Does new LootDrop() still work?");

        // Nothing to run: the answer shows up before Play can even start.
        DemoHUD.Show("Try", "Stop Play. Open LootDrop.cs and uncomment the constructor.");
        DemoHUD.Show("Then", "Save, and read the Console. Which file does the error point to?");
    }

    // ---------- Step 3: Structs are different ----------

    private void StructsAreDifferent()
    {
        BeginStep("Structs are different",
                  "DamageHit wrote its own constructor. Is new DamageHit() still legal?");

        DamageHit blank = new DamageHit();
        DamageHit critical = new DamageHit(25, true);

        // We never wrote new DamageHit() for any of these 1,000. What's in the last one?
        DamageHit[] hits = new DamageHit[1000];

        DemoHUD.Show("new DamageHit()", "amount " + blank.amount + ", isCritical " + blank.isCritical);
        DemoHUD.Show("new DamageHit(25, true)", "amount " + critical.amount + ", isCritical " + critical.isCritical);
        DemoHUD.Show("hits[999]", "amount " + hits[999].amount + ", isCritical " + hits[999].isCritical);
    }

    // ---------- Step 4: Honesty check ----------

    private void HonestyCheck()
    {
        BeginStep("Honesty check",
                  "C# 10 allows a parameterless struct constructor. Does Unity?");

        DemoHUD.Show("Look", "Open Assembly-CSharp.csproj and find <LangVersion>.");
        DemoHUD.Show("Try", "Stop Play. Open DamageHit.cs and uncomment the constructor.");
        DemoHUD.Show("Then", "Save, and read the Console.");
    }

    // ---------- Step 5: Our Enemy can't have one ----------

    private void OurEnemyCantHaveOne()
    {
        BeginStep("Our Enemy can't have one",
                  "Who builds each Enemy? Who builds its EnemyStats?");

        string[] names = new string[] { "Grunt A", "Grunt B" };
        int[] startingHealth = new int[] { 100, 60 };
        SpawnEnemies(names, startingHealth);
    }

    // ---------- Step 6: Which means (the hidden this) ----------

    private void TheHiddenThis()
    {
        EnemyStats gruntA = new EnemyStats("Grunt A", 100);
        EnemyStats gruntB = new EnemyStats("Grunt B", 100);
        EnemyStats brute = new EnemyStats("Brute", 250);

        // The constructors logged while building; wipe that so only Hurt shows.
        DemoHUD.Clear();
        BeginStep("The hidden this",
                  "Three enemies, one Hurt method. How does it know whose health?");

        // Each line calls the same method. What changes between them?
        gruntA.Hurt(10);
        gruntB.Hurt(25);
        brute.Hurt(40);

        DemoHUD.Show(gruntA.Name, gruntA.Health);
        DemoHUD.Show(gruntB.Name, gruntB.Health);
        DemoHUD.Show(brute.Name, brute.Health);
    }

    // ---------- Step 7: Do it, predict the HUD ----------

    private void PredictTheHUD()
    {
        BeginStep("Predict the HUD",
                  "Three lines print. In what order, with what values?");

        DemoHUD.Log("new EnemyStats()");

        // One line of ours. Count how many lines of EnemyStats it runs.
        EnemyStats stats = new EnemyStats();

        DemoHUD.Show("Finished", stats.Name + ", health " + stats.Health);
    }

    // ---------- Step 8: Why any of this matters ----------

    private void NeverBroken()
    {
        BeginStep("Never broken",
                  "The Brute asks for 0 health. What does it get?");

        string[] names = new string[] { "Grunt A", "Grunt B", "Brute" };
        int[] startingHealth = new int[] { 100, 60, 0 };
        SpawnEnemies(names, startingHealth);
    }

    // ---------- Helpers ----------

    private void BeginStep(string slideTitle, string prediction)
    {
        DemoHUD.SetTitle("Session 15 · Step " + step + " of 8: " + slideTitle);
        DemoHUD.Show("Predict", prediction);
    }

    private void SpawnEnemies(string[] names, int[] startingHealth)
    {
        // A prefab can't point at an object in a scene, so every spawned Enemy
        // arrives with player == null. Find the player once, here.
        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found != null) { player = found.transform; }
        }

        if (enemyPrefab == null || player == null)
        {
            DemoHUD.Log("Missing the Enemy prefab or the Player. Ask your instructor.");
            return;
        }

        for (int i = 0; i < names.Length; i++)
        {
            Vector2 spawnAt = (Vector2)transform.position + new Vector2(i * spacing, 0f);

            // Unity builds the GameObject and its Enemy. No new, no constructor of ours.
            GameObject spawned = Instantiate(enemyPrefab, spawnAt, Quaternion.identity);
            spawned.name = names[i];

            Enemy enemy = spawned.GetComponent<Enemy>();
            if (enemy == null) { continue; }
            enemy.player = player;

            // We build the stats. new means "run a constructor."
            enemy.stats = new EnemyStats(names[i], startingHealth[i]);

            DemoHUD.Show(names[i], "health " + enemy.stats.Health);
        }
    }
}
