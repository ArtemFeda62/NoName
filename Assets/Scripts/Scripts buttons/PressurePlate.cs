using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Настройки кнопки")]
    [SerializeField] private float _requiredMass = 1f;
    [SerializeField] private bool _oneTimeOnly = false;

    [Header("Связь с объектами")]
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private UnityEvent onReleased;

    [Header("Визуал (опционально)")]
    [SerializeField] private Transform _visualPart;
    [SerializeField] private float _pressOffset = 0.1f;

    private float _currentMass = 0f;
    private bool _isPressed = false;
    private bool _alreadyUsed = false;
    private Vector3 _originalPosition;
    private Vector3 _pressedPosition;

    private void Start()
    {
        if (_visualPart != null)
        {
            _originalPosition = _visualPart.localPosition;
            _pressedPosition = _originalPosition;
            _pressedPosition.y -= _pressOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_oneTimeOnly && _alreadyUsed) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            _currentMass += rb.mass;
            Debug.Log($"{other.name} наступил. Масса: {_currentMass}/{_requiredMass}");
            UpdateState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_oneTimeOnly && _alreadyUsed) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            _currentMass -= rb.mass;
            Debug.Log($"{other.name} ушел. Масса: {_currentMass}/{_requiredMass}");
            UpdateState();

            UpdateVisual();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_oneTimeOnly && _alreadyUsed) return;

        if (other.attachedRigidbody == null)
        {
            _currentMass = _requiredMass;
            UpdateState();
        }
    }

    private void UpdateState()
    {
        bool shouldBePressed = _currentMass >= _requiredMass;

        if (shouldBePressed && !_isPressed)
        {
            _isPressed = true;
            Debug.Log("КНОПКА НАЖАТА!");
            UpdateVisual();
            onPressed.Invoke();

            if (_oneTimeOnly)
                _alreadyUsed = true;
        }
        else if (!shouldBePressed && _isPressed)
        {
            _isPressed = false;
            Debug.Log("КНОПКА ОТЖАТА!");
            UpdateVisual();
            onReleased.Invoke();
        }
    }

    private void UpdateVisual()
    {
        if (_visualPart == null) return;

        if (_isPressed)
        {
            _visualPart.localPosition = _pressedPosition;
            Debug.Log($"Визуал: кнопка нажата (позиция: {_pressedPosition.y})");
        }
        else
        {
            _visualPart.localPosition = _originalPosition;
            Debug.Log($"Визуал: кнопка отжата (позиция: {_originalPosition.y})");
        }
    }

    public void ResetPlate()
    {
        if (_oneTimeOnly)
        {
            _alreadyUsed = false;
            _isPressed = false;
            _currentMass = 0f;
            UpdateVisual();
        }
    }

    public bool IsPressed()
    {
        return _isPressed;
    }

    public void ClearMass()
    {
        _currentMass = 0f;
        UpdateState();
    }
}