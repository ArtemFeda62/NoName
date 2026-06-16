using UnityEngine;

public class LaserDestroyable : MonoBehaviour
{
    [Header("Настройки уничтожения")]
    [SerializeField] private float _destroyDelay = 0.5f;
    [SerializeField] private bool _destroyOnLaserHit = true;
    [SerializeField] private bool _checkForLampInSlot = true;

    [Header("Эффекты")]
    [SerializeField] private GameObject _destroyEffectPrefab;
    [SerializeField] private AudioClip _destroySound;
    [SerializeField] private float _soundVolume = 1f;

    [Header("Защита")]
    [SerializeField] private bool _indestructible = false;
    [SerializeField] private float _invulnerabilityTime = 1f;

    private bool _isDestroyed = false;
    private float _lastHitTime = 0f;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null && _destroySound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
        }
    }

    public void OnLaserHit()
    {
        if (_indestructible) return;
        if (_isDestroyed) return;
        if (Time.time < _lastHitTime + _invulnerabilityTime) return;

        _lastHitTime = Time.time;

        // Проверяем, не лампа ли это в слоте
        if (_checkForLampInSlot)
        {
            Lamp lamp = GetComponent<Lamp>();
            if (lamp != null && lamp.IsInSlot())
            {
                Debug.Log($"Лампа {gameObject.name} в слоте, не уничтожается");
                return;
            }
        }

        if (_destroyOnLaserHit)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        _isDestroyed = true;

        Debug.Log($"Объект {gameObject.name} уничтожен лазером!");

        // Эффект уничтожения
        if (_destroyEffectPrefab != null)
        {
            GameObject effect = Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Звук уничтожения
        if (_audioSource != null && _destroySound != null)
        {
            _audioSource.PlayOneShot(_destroySound, _soundVolume);
        }

        // Уничтожаем объект с задержкой
        Destroy(gameObject, _destroyDelay);
    }

    public void MakeIndestructible(bool indestructible)
    {
        _indestructible = indestructible;
    }

    public void SetDestroyDelay(float delay)
    {
        _destroyDelay = delay;
    }
}