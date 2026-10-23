// Session 16 — static Means No this
// Concept: an instance field is one per object. A static field is one per type.
// Watch:   the HUD rows, and whether "static Counter() ran" shows up in the log.

public class Counter
{
    public int instanceCount;          // one per Counter
    public static int staticCount;     // ONE. For the whole program.

    // Session 16: the constructor for the type, not for an object.
    // No access modifier, no parameters, and nobody ever calls it.
    static Counter()
    {
        staticCount = 0;
        DemoHUD.Log("static Counter() ran: staticCount = 0");
    }

    public Counter()
    {
        instanceCount++;
        staticCount++;
    }
}
