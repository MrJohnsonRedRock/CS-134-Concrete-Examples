// Session 15 — What `new` Actually Calls
// Concept: if you write no constructor, C# writes one for you.
// Watch:   the HUD rows. Then uncomment the constructor below and watch the Console.

// Public fields on purpose: this mirrors the slide, and the demo reads them directly.
public class LootDrop
{
    public string itemName;
    public int coins;
    public bool isRare;

    // Uncomment me. Then read the Console.
    // public LootDrop(int startingCoins)
    // {
    //     coins = startingCoins;
    // }
}
