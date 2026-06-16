using UnityEngine;

public class Lamp : MonoBehaviour
{
    [Header("Настройки лампы")]
    [SerializeField] private Material _materialOn;
    [SerializeField] private Material _materialOff;
    [SerializeField] private Renderer _lampRenderer;
    [SerializeField] private Light _lightSource;

    [Header("Звуки")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _turnOnSound;
    [SerializeField] private AudioClip _turnOffSound;

    private bool _isOn = false;
    private LampSlot _currentSlot = null;


    private void Start()
    {
        if (_lampRenderer == null)
            _lampRenderer = GetComponent<Renderer>();

        TurnOff();
    }


    public void TurnOn()
    {
        if (_isOn) return;

        _isOn = true;

        if (_lampRenderer != null && _materialOn != null)
            _lampRenderer.material = _materialOn;

        if (_lightSource != null)
            _lightSource.enabled = true;

        if (_audioSource != null && _turnOnSound != null)
            _audioSource.PlayOneShot(_turnOnSound);

        if (_currentSlot != null)
        {
            _currentSlot.OnLampStateChanged(true);
        }

        Debug.Log($"Лампа {gameObject.name} включена");
    }

    public void TurnOff()
    {
        if (!_isOn) return;

        _isOn = false;

        if (_lampRenderer != null && _materialOff != null)
            _lampRenderer.material = _materialOff;

        if (_lightSource != null)
            _lightSource.enabled = false;

        if (_audioSource != null && _turnOffSound != null)
            _audioSource.PlayOneShot(_turnOffSound);

        if (_currentSlot != null)
        {
            _currentSlot.OnLampStateChanged(false);
        }

        Debug.Log($"Лампа {gameObject.name} выключена");
    }

    public void Toggle()
    {
        if (_isOn)
            TurnOff();
        else
            TurnOn();
    }

    public bool IsOn() => _isOn;

    public void SetSlot(LampSlot slot)
    {
        _currentSlot = slot;
        Debug.Log($"Лампа {gameObject.name} {(slot != null ? $"установлена в слот {slot.name}" : "извлечена из слота")}");
    }

    public bool IsInSlot() => _currentSlot != null;

    public LampSlot GetSlot() => _currentSlot;


}
