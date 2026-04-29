using UnityEngine;
using System.Collections;

public class Piston : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _pushForce = 500f;
    [SerializeField] private float _extendDistance = 1f;
    [SerializeField] private float _extendSpeed = 5f;
    [SerializeField] private float _retractDelay = 0.5f;
    [SerializeField] private bool _autoRetract = true;

    [Header("Визуал")]
    [SerializeField] private Transform _pistonHead;
    [SerializeField] private LayerMask _pushableLayers = -1;

    private Vector3 _initialPosition;
    private Vector3 _extendedPosition;
    private bool _isMoving = false;

    private void Start()
    {
        if (_pistonHead == null)
            _pistonHead = transform;

        _initialPosition = _pistonHead.position;
        _extendedPosition = _initialPosition + _pistonHead.up * _extendDistance;
    }

    public void ActivatePiston()
    {
        if (_isMoving) return;

        StartCoroutine(PistonAction());
    }

    private IEnumerator PistonAction()
    {
        _isMoving = true;

        yield return StartCoroutine(MovePiston(_extendedPosition, _extendSpeed));

        PushObjects();

        if (_autoRetract)
        {
            yield return new WaitForSeconds(_retractDelay);
            yield return StartCoroutine(MovePiston(_initialPosition, _extendSpeed));
        }

        _isMoving = false;
    }

    private IEnumerator MovePiston(Vector3 targetPosition, float speed)
    {
        float journey = 0f;
        Vector3 startPosition = _pistonHead.position;

        while (journey < 1f)
        {
            journey += Time.deltaTime * speed;
            _pistonHead.position = Vector3.Lerp(startPosition, targetPosition, journey);
            yield return null;
        }

        _pistonHead.position = targetPosition;
    }

    private void PushObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_pistonHead.position, 0.5f, _pushableLayers);

        foreach (var hit in hitColliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = transform.up;
                rb.AddForce(pushDirection * _pushForce, ForceMode.Impulse);
                Debug.Log($"Толкнули объект: {hit.name} с силой {_pushForce}");
            }
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(_pistonHead.position, transform.up, out hitInfo, 1f, _pushableLayers))
        {
            Rigidbody rb = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.up * _pushForce, ForceMode.Impulse);
                Debug.Log($"Raycast толкнул объект: {hitInfo.collider.name}");
            }
        }
    }
}