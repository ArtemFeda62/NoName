using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PressurePlate : MonoBehaviour
{
    [Header("Настройки кнопки")]
    [SerializeField] private float _requiredMass = 1f;
    [SerializeField] private bool _oneTimeOnly = false;

    [Header("Связь с объектами")]
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private UnityEvent onReleased;

    [Header("Визуал")]
    [SerializeField] private Transform _visualPart; // Явно указываем визуальную часть
    [SerializeField] private float _pressOffset = 0.1f;
    [SerializeField] private float _pressDuration = 0.3f;

    private float _currentMass = 0f;
    private bool _isPressed = false;
    private bool _alreadyUsed = false;
    private Coroutine _visualCoroutine;
    private Vector3 _originalPosition;

    private void Start()
    {
        // Сохраняем оригинальную позицию
        if (_visualPart != null)
        {
            _originalPosition = _visualPart.localPosition;
            Debug.Log($"Оригинальная позиция сохранена: {_originalPosition}");
        }
        else
        {
            Debug.LogError("Привяжите визуальную часть кнопки в поле Visual Part!");
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
        }
    }

    private void UpdateState()
    {
        bool shouldBePressed = _currentMass >= _requiredMass;

        if (shouldBePressed && !_isPressed)
        {
            _isPressed = true;
            Debug.Log("КНОПКА НАЖАТА!");

            if (_visualCoroutine != null) StopCoroutine(_visualCoroutine);
            _visualCoroutine = StartCoroutine(AnimatePress(true));

            onPressed.Invoke();
            if (_oneTimeOnly) _alreadyUsed = true;
        }
        else if (!shouldBePressed && _isPressed)
        {
            _isPressed = false;
            Debug.Log("КНОПКА ОТЖАТА!");

            if (_visualCoroutine != null) StopCoroutine(_visualCoroutine);
            _visualCoroutine = StartCoroutine(AnimatePress(false));

            onReleased.Invoke();
        }
    }

    private IEnumerator AnimatePress(bool pressed)
    {
        if (_visualPart == null) yield break;

        Vector3 startPos = _visualPart.localPosition;
        Vector3 endPos;

        if (pressed)
        {
            endPos = _originalPosition;
            endPos.y -= _pressOffset;
            Debug.Log($"Анимация: опускаемся до {endPos.y}");
        }
        else
        {
            endPos = _originalPosition;
            Debug.Log($"Анимация: поднимаемся до {endPos.y}");
        }

        float time = 0;
        while (time < _pressDuration)
        {
            time += Time.deltaTime;
            float t = time / _pressDuration;
            _visualPart.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _visualPart.localPosition = endPos;
        Debug.Log($"Анимация завершена. Позиция: {_visualPart.localPosition.y}");
        _visualCoroutine = null;
    }
}