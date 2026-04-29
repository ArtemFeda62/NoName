using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _gravityForce = 9.81f;
    [SerializeField] private bool _startWithGravityDown = true;

    private Rigidbody _rigidbody;
    private bool _isGravityDown;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"GravityChanger: Rigidbody не найден на {gameObject.name}");
            enabled = false;
            return;
        }

        _rigidbody.useGravity = false;
        _isGravityDown = _startWithGravityDown;
    }

    private void FixedUpdate()
    {
        if (_rigidbody == null) return;

        Vector3 gravityDirection = _isGravityDown ? Vector3.down : Vector3.up;
        Vector3 gravityForce = gravityDirection * _gravityForce;
        _rigidbody.AddForce(gravityForce, ForceMode.Impulse);
    }

    public void ToggleGravity()
    {
        _isGravityDown = !_isGravityDown;
        Debug.Log($"Гравитация изменена на: {(_isGravityDown ? "вниз" : "вверх")}");
    }

    public void SetGravityDown(bool isDown)
    {
        _isGravityDown = isDown;
    }

    public bool IsGravityDown => _isGravityDown;
}