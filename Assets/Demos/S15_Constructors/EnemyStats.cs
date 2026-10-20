// Session 15 — What `new` Actually Calls
// Concept: a constructor is a method named like its type, with no return type,
//          that runs once, when the object is made.
// Watch:   the HUD log, as each EnemyStats is built.

// A plain C# class, not a MonoBehaviour. Unity builds an Enemy when it spawns;
// nobody builds its EnemyStats but us, with new.
public class EnemyStats
{
    // Field initializers. When do these run?
    private string name = "Unnamed";
    private int health = 1;

    // Session 15: ": this(...)" means "before my body runs, run that other
    // constructor." One constructor builds the object; this one delegates.
    public EnemyStats() : this("Grunt", 100)
    {
        // No parameter called health here, so health can only mean the field.
        DemoHUD.Log("A: " + name + ", health = " + health);
    }

    public EnemyStats(string name, int health)
    {
        // Two different variables: health is the parameter (the nearer name wins),
        // this.health is the field on the object being built.
        DemoHUD.Log("B: " + name + ", this.health = " + this.health);

        // A constructor's job is to make sure the object is never broken.
        // An enemy with no name, or with 0 health that isn't dead, is broken.
        if (name == null || name.Length == 0)
        {
            DemoHUD.Log("   refused an empty name, using Unnamed");
            name = "Unnamed";
        }
        if (health < 1)
        {
            DemoHUD.Log("   refused health " + health + " for " + name + ", using 1");
            health = 1;
        }

        this.name = name;
        this.health = health;
        DemoHUD.Log("C: " + this.name + ", this.health = " + this.health);
    }

    public string Name
    {
        get { return name; }
    }

    public int Health
    {
        get { return health; }
    }

    // Session 15: there is one Hurt method, no matter how many EnemyStats exist.
    // The compiler builds it as Hurt(EnemyStats this, int amount), so
    // stats.Hurt(10) is really Hurt(stats, 10).
    public void Hurt(int amount)
    {
        health -= amount;   // really this.health -= amount
        DemoHUD.Log("Hurt(this = " + name + ", amount = " + amount + "): health now " + health);
    }
}
