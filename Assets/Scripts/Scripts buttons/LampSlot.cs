using UnityEngine;
using UnityEngine.Events;

public class LampSlot : MonoBehaviour
{
    [Header("Настройки слота")]
    [SerializeField] private Transform _lampHolderPoint;
    [SerializeField] private Vector3 _lampOffset = new Vector3(0, 1.5f, 0);

    [Header("События")]
    public UnityEvent OnLampTurnedOn; 
    public UnityEvent OnLampTurnedOff; 

    private Lamp _currentLamp;
    private bool _hasLamp = false;
    

    public bool HasLamp => _hasLamp;
    public Lamp CurrentLamp => _currentLamp;

    private void Start()
    {
        if (_lampHolderPoint == null)
            _lampHolderPoint = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasLamp) return;

        Lamp lamp = other.GetComponent<Lamp>();
        if (lamp != null && !lamp.IsInSlot())
        {
            InsertLamp(lamp);
        }
    }

    private void InsertLamp(Lamp lamp)
    {

        _currentLamp = lamp;
        _hasLamp = true;

        lamp.transform.SetParent(_lampHolderPoint);
        lamp.transform.localPosition = _lampOffset;
        lamp.transform.localRotation = Quaternion.identity;

        Rigidbody rb = lamp.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        Collider col = lamp.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;
        
        lamp.SetSlot(this);

        if (lamp.IsOn())
        {
            OnLampTurnedOn?.Invoke();
        }

        Debug.Log($"Лампа {lamp.name} установлена в слот {gameObject.name}");
    }

    public void OnLampStateChanged(bool isOn)
    {
        if (!_hasLamp || _currentLamp == null)
        {
            Debug.Log($"Слот {gameObject.name}:  лампы нет в слоте");
            return;
        }

        if (isOn)
        {
            OnLampTurnedOn?.Invoke();
            Debug.Log($"Слот {gameObject.name}: лампа включена, активирую события");
        }
        else
        {
            OnLampTurnedOff?.Invoke();
            Debug.Log($"Слот {gameObject.name}: лампа выключена, деактивирую события");
        }
    }

    public void RemoveLamp()
    {
        if (!_hasLamp || _currentLamp == null) return;

        Debug.Log($"Лампа {_currentLamp.name} извлекается из слота {gameObject.name}");
        
        Lamp removedLamp = _currentLamp;
        bool wasLampOn = removedLamp.IsOn();
        
        if (wasLampOn)
        {
            OnLampTurnedOff?.Invoke();
            Debug.Log($"Слот {gameObject.name}: лампа была включена, вызываю OnLampTurnedOff при извлечении");
        }
        
        removedLamp.SetSlot(null);

        Rigidbody rb = removedLamp.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        
        Collider col = removedLamp.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = false;

        _currentLamp = null;
        _hasLamp = false;
        
        Debug.Log($"Лампа {removedLamp.name} извлечена из слота {gameObject.name}");
    }

    private void OnDrawGizmosSelected()
    {
        if (_lampHolderPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 lampPos = _lampHolderPoint.position + _lampHolderPoint.TransformDirection(_lampOffset);
            Gizmos.DrawWireCube(lampPos, Vector3.one * 0.5f);
        }
    }
}