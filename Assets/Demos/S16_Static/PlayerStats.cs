// Session 16 — static Means No this
// Concept: a static method has no hidden this, so it has no object to read fields from.
// Watch:   the Console, after you uncomment the method below.

public class PlayerStats
{
    private int health = 40;

    public int Health
    {
        get { return health; }
    }

    // An instance method. Session 15: the compiler hands it a hidden this,
    // so health here means this.health.
    public void Heal()
    {
        health = 100;
    }

    // Uncomment me. Then read the Console.
    // public static void FullHeal()
    // {
    //     health = 100;
    // }
}
