using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private string _deathTag = "Lava";
    [SerializeField] private Transform _spawnPoint;

    private Vector3 _lastCheckpoint;
    private float _saveTimer = 0f;
    private float _saveInterval = 5f;

    private void Start()
    {
        if (_spawnPoint != null)
        {
            _lastCheckpoint = _spawnPoint.position;
            transform.position = _spawnPoint.position;
        }
        else
        {
            _lastCheckpoint = transform.position;
        }
    }

    private void Update()
    {
        _saveTimer += Time.deltaTime;
        if (_saveTimer >= _saveInterval)
        {
            _saveTimer = 0f;
            _lastCheckpoint = transform.position;
            Debug.Log($"Чекпоинт сохранён: {_lastCheckpoint}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_deathTag))
        {
            TeleportToCheckpoint();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_deathTag))
        {
            TeleportToCheckpoint();
        }
    }

    public void TeleportToCheckpoint()
    {
        Debug.Log($"Телепорт к чекпоинту: {_lastCheckpoint}");

        _saveTimer = 0f;

        transform.position = _lastCheckpoint;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            cc.enabled = true;
        }

        Debug.Log("Игрок телепортирован к чекпоинту, таймер сброшен");
    }

    public void TeleportToSpawn()
    {
        if (_spawnPoint == null) return;

        Debug.Log($"Телепорт на спавн: {_spawnPoint.position}");

        _saveTimer = 0f;
        _lastCheckpoint = _spawnPoint.position;

        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            cc.enabled = true;
        }

        Debug.Log("Игрок телепортирован на спавн");
    }
}