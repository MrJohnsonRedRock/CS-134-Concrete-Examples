using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // This "one shared instance everyone can reach" has a name.
    // Remember it — we name it officially on Session 13.
    public static ScoreManager instance;

    public TMP_Text scoreText;  // the HUD text object
    private int score = 0;

    void Awake()
    {
        instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;

        // FORMAT AND DISPLAY the variable to the GUI:
        scoreText.text = "Score: " + score;

        if(score > 100)
        {
           // GameManager.instance.WinGame(); 
        }
    }
}
