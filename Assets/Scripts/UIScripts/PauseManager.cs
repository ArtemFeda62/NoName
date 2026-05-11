using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;
    public void OnEscape()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;      
        pausePanel.SetActive(true);  
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;   
        pausePanel.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;      
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }
}