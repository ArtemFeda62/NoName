using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Кнопки")]
    [SerializeField] private GameObject _continueButton;

    private void Start()
    {
        if (_continueButton != null)
        {
            _continueButton.SetActive(SaveSystem.HasSaveData());
        }
    }

    public void NewGame()
    {
        SaveSystem.DeleteProgress();
        SceneManager.LoadScene(1); 
    }

    public void ContinueGame()
    {
        int lastLevel = SaveSystem.LoadProgress();

        if (lastLevel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(lastLevel);
        }
        else
        {
            SaveSystem.DeleteProgress();
            SceneManager.LoadScene(1);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}