using UnityEngine;

public class Piston : MonoBehaviour
{
    [Header("Piston Settings")]
    [SerializeField] private float _pushForce = 10f;
    [SerializeField] private float _pushDuration = 0.5f;
    [SerializeField] private Vector3 _pushDirection = Vector3.up;
    [SerializeField] private float _detectionRadius = 1f;

    private ButtonTriggerPiston _buttonTrigger;
    private bool _isPushing;
    private float _pushTimer;

    private void Start()
    {
        _buttonTrigger = GetComponent<ButtonTriggerPiston>();

        if (_buttonTrigger != null)
        {
            _buttonTrigger.OnPiston += ActivatePiston;
        }
        else
        {
        }
    }

    private void Update()
    {
        if (_isPushing)
        {
            _pushTimer -= Time.deltaTime;

            if (_pushTimer <= 0)
            {
                _isPushing = false;
            }
        }
    }

    private void ActivatePiston()
    {
        if (_isPushing) return;

        _isPushing = true;
        _pushTimer = _pushDuration;

        PushObjectsInRange();
    }

    private void PushObjectsInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 forceDirection = transform.TransformDirection(_pushDirection).normalized;
                rb.AddForce(forceDirection * _pushForce, ForceMode.Impulse);
            }
        }
    }
}