// Session 15 — What `new` Actually Calls
// Concept: a struct always has a parameterless constructor, even after you write your own.
// Watch:   the HUD rows. Then uncomment the constructor below and watch the Console.

// A struct: the box holds the thing itself, not an arrow to it.
public struct DamageHit
{
    public int amount;
    public bool isCritical;

    public DamageHit(int amount, bool isCritical)
    {
        this.amount = amount;
        this.isCritical = isCritical;
    }

    // C# 10 lets a struct declare its own parameterless constructor.
    // Which C# does Unity compile? Uncomment me. Then read the Console.
     //public DamageHit()
    //{
    //     amount = 1;
    //     isCritical = false;
    // }
}
