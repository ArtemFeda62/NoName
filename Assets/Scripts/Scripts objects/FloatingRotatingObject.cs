using UnityEngine;

public class FloatingRotatingObject : MonoBehaviour
{
    [Header("Настройки вращения")]
    [SerializeField] private Vector3 _rotationSpeed = new Vector3(0f, 45f, 0f); // Вращается только вокруг Y оси (как на стенде)
    [SerializeField] private bool _rotateAroundYOnly = true; // Вращать только вокруг вертикальной оси

    [Header("Настройки движения вверх-вниз")]
    [SerializeField] private float _floatAmplitude = 0.1f;
    [SerializeField] private float _floatSpeed = 0.5f;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    void Update()
    {
        // Вращение (как на музейном стенде)
        if (_rotateAroundYOnly)
        {
            // Вращаем только вокруг вертикальной оси
            transform.Rotate(Vector3.up, _rotationSpeed.y * Time.deltaTime, Space.World);
        }
        else
        {
            // Полное вращение по всем осям
            transform.Rotate(_rotationSpeed * Time.deltaTime);
        }

        // Плавное поднятие-опускание
        float newY = _startPosition.y + Mathf.Sin(Time.time * _floatSpeed) * _floatAmplitude;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
}