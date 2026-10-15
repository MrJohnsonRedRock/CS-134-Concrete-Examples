using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject pausePanel;
    public GameObject thumbsUp;// drag PausePanel here in the Inspector

    void Awake() { instance = this; }

    // VOCAB: public methods can be wired to a Button's OnClick event (DGD.5.3.3)
    public void ShowPause()
    {
        pausePanel.SetActive(true);  // the object switches ON
        Time.timeScale = 0f;         // freeze the game while paused
    }
    public void HidePause()
    {
        pausePanel.SetActive(false); // the object switches OFF
        Time.timeScale = 1f;         // resume normal time
    }

    public void showThumbsUp()
    {
        thumbsUp.SetActive(true);
    }
    public void LoadLevel(string sceneName)
    {
        Time.timeScale = 1f;         // always unfreeze before changing scenes!
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void WinGame()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void LoseGame()
    {
        SceneManager.LoadScene("LoseScreen");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            ShowPause(); 
        }
    }
}