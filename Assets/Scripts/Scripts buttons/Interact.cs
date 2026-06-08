using UnityEngine;
using UnityEngine.Events;

public class Interact : MonoBehaviour
{
    [SerializeField] private bool _isDropped = true;
    [SerializeField] private string _targetTag = "Default";
    [SerializeField] private float _verticalOffset = 2.5f;
    private Rigidbody rb;
    private Collider objectCollider;

    public UnityEvent _onInteract;
    public UnityEvent _offInteract;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();

        if (rb != null)
            rb.isKinematic = false;
        else
            Debug.LogError($"Rigidbody не найден на {gameObject.name}");
    }

    public void DroppeedFalse()
    {
        _isDropped = false;
        Debug.Log($"{gameObject.name}: _isDropped = false");

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }
        if (objectCollider != null)
            objectCollider.isTrigger = false;
    }

    public void DroppeedTrue()
    {
        _isDropped = true;
        Debug.Log($"{gameObject.name}: _isDropped = true");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDropped && collision.gameObject.CompareTag(_targetTag))
        {     
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            transform.SetParent(collision.transform);

            transform.position = collision.transform.position + new Vector3(0, _verticalOffset, 0);

            transform.localRotation = Quaternion.identity;

            if (objectCollider != null)
                objectCollider.isTrigger = true;

            Debug.Log($"{gameObject.name} установлен на {collision.gameObject.name}");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!_isDropped && collision.gameObject.CompareTag(_targetTag))
        {
            rb.isKinematic = false;
        }
    }
}