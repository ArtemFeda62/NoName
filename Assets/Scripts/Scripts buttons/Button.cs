using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Button : MonoBehaviour
{
    [Header("Настройки кнопки")]
    [SerializeField] private float _requiredWeight = 1f;
    [SerializeField] private float _pressDepth = 0.1f;
    [SerializeField] private float _returnSpeed = 5f;

    [Header("Ссылки")]
    [SerializeField] private Transform _pressPoint;
    [SerializeField] private List<Lamp> _connectedLamps;

    [Header("События")]
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;

    private Vector3 _initialPosition;
    private bool _isPressed = false;
    private GameObject _currentObjectOnButton;
    private bool _isReturning = false;

    private void Start()
    {
        if (_pressPoint != null)
            _initialPosition = _pressPoint.localPosition;
        else
            _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_isReturning && !_isPressed)
        {
            Vector3 targetPos = _initialPosition;
            Vector3 currentPos = _pressPoint != null ? _pressPoint.localPosition : transform.localPosition;

            Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, _returnSpeed * Time.deltaTime);

            if (_pressPoint != null)
                _pressPoint.localPosition = newPos;
            else
                transform.localPosition = newPos;

            if (Vector3.Distance(currentPos, targetPos) < 0.001f)
            {
                _isReturning = false;
                if (_pressPoint != null)
                    _pressPoint.localPosition = _initialPosition;
                else
                    transform.localPosition = _initialPosition;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPressed) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.mass >= _requiredWeight)
        {
            PressButton(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isPressed && _currentObjectOnButton == other.gameObject)
        {
            ReleaseButton();
        }
    }

    private void PressButton(GameObject obj)
    {
        _isPressed = true;
        _currentObjectOnButton = obj;
        _isReturning = false;

        Vector3 pressedPos = _initialPosition - new Vector3(0, _pressDepth, 0);
        if (_pressPoint != null)
            _pressPoint.localPosition = pressedPos;
        else
            transform.localPosition = pressedPos;

        foreach (Lamp lamp in _connectedLamps)
        {
            if (lamp != null)
                lamp.TurnOn();
        }

        OnButtonPressed?.Invoke();

        Debug.Log($"Кнопка {gameObject.name} нажата, включено ламп: {_connectedLamps.Count}");
    }

    private void ReleaseButton()
    {
        _isPressed = false;
        _currentObjectOnButton = null;
        _isReturning = true;

        foreach (Lamp lamp in _connectedLamps)
        {
            if (lamp != null)
                lamp.TurnOff();  
        }

        OnButtonReleased?.Invoke();

        Debug.Log($"Кнопка {gameObject.name} отпущена, лампы выключены");
    }
}