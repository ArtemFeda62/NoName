using UnityEngine;

public class OnDestroyDisableKinematic : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject _objectToMonitor; 
    [SerializeField] private Rigidbody _targetRigidbody;
    [SerializeField] private bool _destroySelfOnComplete = true;

    private bool _isTriggered = false;

    private void Start()
    {
        if (_objectToMonitor == null)
            _objectToMonitor = gameObject;
        if (_targetRigidbody == null)
            _targetRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isTriggered) return;

        if (_objectToMonitor == null)
        {
            _isTriggered = true;
            DisableKinematic();
        }
    }

    private void DisableKinematic()
    {
        if (_targetRigidbody != null)
        {
            _targetRigidbody.isKinematic = false;
            Debug.Log($"Отключен Kinematic у {_targetRigidbody.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Целевой Rigidbody не найден!");
        }

        if (_destroySelfOnComplete)
            Destroy(this);
    }
}