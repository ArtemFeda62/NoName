using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _restartDelay = 2f;

    private bool _isDead = false;
    private float _deathTimer = 0f;

    private void Update()
    {
        if (_isDead)
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= _restartDelay)
            {
                RestartLevel();
            }
        }
    }

    public void CheckPlayerDeath()
    {
        if (_player == null)
        {
            _isDead = true;
            Debug.Log("Игрок умер! Рестарт через " + _restartDelay + " секунд");
        }
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}