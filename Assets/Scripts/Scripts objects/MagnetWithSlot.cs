using UnityEngine;

public class MagnetWithSlot : MonoBehaviour
{
    [Header("Настройки магнита")]
    [SerializeField] private MagneticField _magneticField;
    [SerializeField] private GameObject _magnetVisual;
    [SerializeField] private bool _startActive = false;

    [Header("Слот для лампы")]
    [SerializeField] private LampSlot _lampSlot;
    private bool _hasLampInSlot = false;

    private ParticleSystem _particleSystem;
    private bool _isActive = false;

    private void Start()
    {
        if (_magneticField == null)
            _magneticField = GetComponent<MagneticField>();

        _particleSystem = GetComponent<ParticleSystem>();

        // Устанавливаем начальное состояние
        _isActive = _startActive;
        SetMagnetActive(_isActive);

        if (_lampSlot != null)
        {
            _lampSlot.OnLampTurnedOn.AddListener(OnLampInSlotTurnedOn);
            _lampSlot.OnLampTurnedOff.AddListener(OnLampInSlotTurnedOff);
            Debug.Log($"МАГНИТ {gameObject.name}: Подписан на события слота");
        }
        else
        {
            Debug.LogError($"МАГНИТ {gameObject.name}: Нет ссылки на LampSlot!");
        }
    }

    private void Update()
    {
        if (_lampSlot != null)
        {
            _hasLampInSlot = _lampSlot.HasLamp;
        }
    }

    private void OnLampInSlotTurnedOn()
    {
        if (!_hasLampInSlot)
        {
            Debug.Log($"МАГНИТ {gameObject.name}: лампы нет в слоте");
            return;
        }

        Debug.Log($"МАГНИТ {gameObject.name}: лампа в слоте, включаю магнит");
        SetMagnetActive(true);
    }

    private void OnLampInSlotTurnedOff()
    {
        Debug.Log($"МАГНИТ {gameObject.name}: лампа выключилась, выключаю магнит");
        SetMagnetActive(false);
    }

    private void SetMagnetActive(bool active)
    {
        _isActive = active;

        if (_magneticField != null)
            _magneticField.SetActive(active);

        if (_magnetVisual != null)
            _magnetVisual.SetActive(active);

        if (_particleSystem != null)
        {
            if (active)
                _particleSystem.Play();
            else
                _particleSystem.Stop();
        }

        Debug.Log($"МАГНИТ {gameObject.name}: {(active ? "ВКЛЮЧЕН" : "ВЫКЛЮЧЕН")}");
    }

    public void ToggleMagnet()
    {
        SetMagnetActive(!_isActive);
    }

    private void OnDestroy()
    {
        if (_lampSlot != null)
        {
            _lampSlot.OnLampTurnedOn.RemoveListener(OnLampInSlotTurnedOn);
            _lampSlot.OnLampTurnedOff.RemoveListener(OnLampInSlotTurnedOff);
        }
    }
}