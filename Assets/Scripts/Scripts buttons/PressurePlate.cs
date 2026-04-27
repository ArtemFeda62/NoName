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
    [SerializeField] private float _pressOffset = 0.1f;
    [SerializeField] private float _pressDuration = 0.3f;

    private float _currentMass = 0f;
    private bool _isPressed = false;
    private bool _alreadyUsed = false;
    private Coroutine _visualCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (_oneTimeOnly && _alreadyUsed) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            _currentMass += rb.mass;
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
            UpdateState();
        }
    }

    private void UpdateState()
    {
        bool shouldBePressed = _currentMass >= _requiredMass;

        if (shouldBePressed && !_isPressed)
        {
            _isPressed = true;
            onPressed.Invoke();
            if (_oneTimeOnly) _alreadyUsed = true;
        }
        else if (!shouldBePressed && _isPressed)
        {
            _isPressed = false;
            onReleased.Invoke();
        }
    }

    private void VisualizePress(bool press)
    {
        if (_visualCoroutine != null)
            StopCoroutine(_visualCoroutine);
        _visualCoroutine = StartCoroutine(AnimatePress(press));
    }

    private IEnumerator AnimatePress(bool press)
    {
        Transform visual = transform.GetChild(1);
        if (visual == null) yield break;

        Vector3 startPos = visual.localPosition;
        Vector3 targetPos = startPos;
        targetPos.y = press ? -_pressOffset : 0f;

        float elapsed = 0f;
        while (elapsed < _pressDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _pressDuration;
            visual.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        visual.localPosition = targetPos;
        _visualCoroutine = null;
    }

    private void OnEnable()
    {
        onPressed.AddListener(() => VisualizePress(true));
        onReleased.AddListener(() => VisualizePress(false));
    }
}