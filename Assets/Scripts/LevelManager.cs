using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _loadDelay = 3f;
    [SerializeField] private float _fadeDuration = 5f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private GameObject _fadePanel;

    private GameObject _player;
    private PlayerController _playerController;
    private PlayerRay _playerRay;
    private CinemachineCamera _cam;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
        {
            _playerController = _player.GetComponent<PlayerController>();
            _playerRay = _player.GetComponent<PlayerRay>();
            _cam = _player.GetComponent<CinemachineCamera>();
        }

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"Загружен уровень {currentLevel}");

        StartCoroutine(FadeIn());
    }

    public void CompleteLevel()
    {
        if (_playerController != null)
            _playerController.enabled = false;
        if (_playerRay != null)
            _playerRay.enabled = false;
        if( _cam != null )
        {
            _cam.enabled = false;
        }

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int nextLevel = currentLevel + 1;

        SaveSystem.SaveProgress(nextLevel);

        Debug.Log($"Уровень {currentLevel} пройден! Сохранён уровень {nextLevel}");

        StartCoroutine(FadeAndLoad(nextLevel));
    }

    private IEnumerator FadeAndLoad(int nextLevel)
    {
        yield return StartCoroutine(FadeOut());

        yield return new WaitForSeconds(_loadDelay);

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            Debug.Log("Все уровни пройдены!");
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator FadeIn()
    {
        if (_fadeCanvasGroup == null || _fadePanel == null) yield break;

        _fadePanel.SetActive(true);
        _fadeCanvasGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeDuration;
            _fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        _fadeCanvasGroup.alpha = 0f;
        _fadePanel.SetActive(false);

        if (_playerController != null)
            _playerController.enabled = true;
        if (_playerRay != null)
            _playerRay.enabled = true;
    }

    private IEnumerator FadeOut()
    {
        if (_fadeCanvasGroup == null || _fadePanel == null) yield break;

        _fadePanel.SetActive(true);
        _fadeCanvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / _fadeDuration;
            _fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        _fadeCanvasGroup.alpha = 1f;
    }

    public void RestartLevel()
    {
        StartCoroutine(FadeAndRestart());
    }

    private IEnumerator FadeAndRestart()
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        StartCoroutine(FadeAndLoadMainMenu());
    }

    private IEnumerator FadeAndLoadMainMenu()
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CompleteLevel();
        }
    }
}