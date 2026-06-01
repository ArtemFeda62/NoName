using UnityEngine;

public class HandRotation : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Трансформ руки")]
    [SerializeField] private Transform _handTransform;

    [Tooltip("Ось вращения=")]
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    [Tooltip("Скорость вращения в градусах в секунду")]
    [SerializeField] private float _rotationSpeed = 90f;

    [Tooltip("Вращать непрерывно=")]
    [SerializeField] private bool _continuous = true;

    [Tooltip("Максимальный угол поворота")]
    [SerializeField] private float _maxAngle = 45f;

    private Quaternion _initialRotation;
    private bool _isRotating = false;
    private float _currentAngle = 0f;

    private void Awake()
    {
        if (_handTransform == null)
            _handTransform = transform;

        _initialRotation = _handTransform.localRotation;
    }

    private void Update()
    {
        if (!_isRotating) return;

        if (_continuous)
        {
            float delta = _rotationSpeed * Time.deltaTime;
            _handTransform.Rotate(_rotationAxis, delta, Space.Self);
        }
        else
        {
            float delta = _rotationSpeed * Time.deltaTime;
            float newAngle = _currentAngle + delta;

            if (newAngle >= _maxAngle)
            {
                _handTransform.localRotation = _initialRotation * Quaternion.AngleAxis(_maxAngle, _rotationAxis);
                _currentAngle = _maxAngle;
                enabled = false; 
            }
            else
            {
                _handTransform.Rotate(_rotationAxis, delta, Space.Self);
                _currentAngle = newAngle;
            }
        }
    }

    public void StartRotation()
    {
        _isRotating = true;
        if (!_continuous)
        {
          
            _currentAngle = 0f;
            _handTransform.localRotation = _initialRotation;
            enabled = true; 
        }
    }


    public void StopRotation()
    {
        _isRotating = false;
        if (!_continuous)
        {
            _handTransform.localRotation = _initialRotation;
            _currentAngle = 0f;
            enabled = false;
        }
    }

    public void PauseRotation()
    {
        _isRotating = false;
    }
}