using UnityEngine;

public class LaserDestroyable : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private bool _checkForLampInSlot = true;
    [SerializeField] private string _lavaTag = "Lava";

    [Header("Эффекты")]
    [SerializeField] private GameObject _teleportEffectPrefab;
    [SerializeField] private AudioClip _teleportSound;
    [SerializeField] private float _soundVolume = 1f;

    [Header("Защита")]
    [SerializeField] private bool _indestructible = false;

    private AudioSource _audioSource;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null && _teleportSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
        }

        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_indestructible) return;

        if (other.CompareTag(_lavaTag))
        {
            TeleportObject();
            return;
        }

        if (other.GetComponent<Laser>() != null)
        {
            OnLaserHit();
        }
    }

    public void OnLaserHit()
    {
        if (_indestructible) return;

        if (_checkForLampInSlot)
        {
            Lamp lamp = GetComponent<Lamp>();
            if (lamp != null && lamp.IsInSlot())
            {
                Debug.Log($"Лампа {gameObject.name} в слоте, не телепортируется");
                return;
            }
        }

        TeleportObject();
    }

    private void TeleportObject()
    {
        Debug.Log($"Объект {gameObject.name} телепортирован на стартовую позицию!");

        transform.position = _startPosition;
        transform.rotation = _startRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (_teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(_teleportEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (_audioSource != null && _teleportSound != null)
        {
            _audioSource.PlayOneShot(_teleportSound, _soundVolume);
        }
    }

    public void MakeIndestructible(bool indestructible)
    {
        _indestructible = indestructible;
    }
}