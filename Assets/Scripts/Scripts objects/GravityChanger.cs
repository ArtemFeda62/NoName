using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _gravityForce = 9.81f;

    private Rigidbody _rigidbody;
    private bool _isGravityDown = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void FixedUpdate()
    {
        Vector3 gravity = _isGravityDown
            ? Vector3.down * _gravityForce
            : Vector3.up * _gravityForce;

        _rigidbody.AddForce(gravity, ForceMode.Impulse);
    }

    public void ToggleGravity()
    {
        _isGravityDown = !_isGravityDown;
    }
}