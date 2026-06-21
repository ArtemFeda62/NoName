using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LampSlot : MonoBehaviour
{
    [Header("Настройки слота")]
    [SerializeField] private Transform _lampHolderPoint;
    [SerializeField] private Vector3 _lampOffset = new Vector3(0, 1.5f, 0);

    [Header("Камера слота")]
    [SerializeField] private Camera _slotCamera;
    [SerializeField] private RenderTexture _renderTexture;
    [SerializeField] private GameObject _uiImageObject;
    [SerializeField] private float _cameraDuration = 2f;

    [Header("События")]
    public UnityEvent OnLampTurnedOn;
    public UnityEvent OnLampTurnedOff;

    private Lamp _currentLamp;
    private bool _hasLamp = false;
    private Coroutine _cameraCoroutine;

    private Vector3 _originalScale;
    private Transform _originalParent;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    public bool HasLamp => _hasLamp;
    public Lamp CurrentLamp => _currentLamp;

    private void Start()
    {
        if (_lampHolderPoint == null)
            _lampHolderPoint = transform;

        if (_slotCamera != null)
        {
            _slotCamera.targetTexture = _renderTexture;
            _slotCamera.enabled = false;
        }

        if (_uiImageObject != null)
        {
            _uiImageObject.SetActive(false);
        }
    }

    [System.Obsolete]
    private void OnTriggerStay(Collider other)
    {
        if (_hasLamp) return;

        Lamp lamp = other.GetComponent<Lamp>();
        if (lamp != null && !lamp.IsInSlot())
        {
            PlayerRay playerRay = FindObjectOfType<PlayerRay>();
            if (playerRay != null && playerRay.IsItemPicked)
            {
                return;
            }

            InsertLamp(lamp);
        }
    }

    [System.Obsolete]
    public void InsertLamp(Lamp lamp)
    {
        if (_hasLamp) return;
        if (lamp == null) return;

        PlayerRay playerRay = FindObjectOfType<PlayerRay>();
        if (playerRay != null && playerRay.IsItemPicked)
        {
            return;
        }

        _originalParent = lamp.transform.parent;
        _originalPosition = lamp.transform.position;
        _originalRotation = lamp.transform.rotation;
        _originalScale = lamp.transform.localScale;

        _currentLamp = lamp;
        _hasLamp = true;

        lamp.transform.SetParent(_lampHolderPoint);
        lamp.transform.localPosition = _lampOffset;
        lamp.transform.localRotation = Quaternion.identity;
        lamp.transform.localScale = _originalScale;

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
            Debug.Log($"Слот {gameObject.name}: лампы нет в слоте");
            return;
        }

        if (isOn)
        {
            OnLampTurnedOn?.Invoke();
            Debug.Log($"Слот {gameObject.name}: лампа включена, активирую события");
            ActivateSlotCamera();
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

        removedLamp.transform.SetParent(_originalParent);
        removedLamp.transform.position = _originalPosition;
        removedLamp.transform.rotation = _originalRotation;
        removedLamp.transform.localScale = _originalScale;

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

    public void ActivateSlotCamera()
    {
        if (_cameraCoroutine != null)
            StopCoroutine(_cameraCoroutine);

        _cameraCoroutine = StartCoroutine(ShowCameraCoroutine());
    }

    public void DeactivateSlotCamera()
    {
        if (_cameraCoroutine != null)
        {
            StopCoroutine(_cameraCoroutine);
            _cameraCoroutine = null;
        }

        if (_slotCamera != null)
            _slotCamera.enabled = false;

        if (_uiImageObject != null)
            _uiImageObject.SetActive(false);

        Debug.Log($"Камера слота {gameObject.name} деактивирована");
    }

    private IEnumerator ShowCameraCoroutine()
    {
        if (_slotCamera != null)
            _slotCamera.enabled = true;

        if (_uiImageObject != null)
            _uiImageObject.SetActive(true);

        Debug.Log($"Камера слота {gameObject.name} активирована на {_cameraDuration} секунд");

        yield return new WaitForSeconds(_cameraDuration);

        if (_slotCamera != null)
            _slotCamera.enabled = false;

        if (_uiImageObject != null)
            _uiImageObject.SetActive(false);

        _cameraCoroutine = null;

        Debug.Log($"Камера слота {gameObject.name} деактивирована по таймеру");
    }

    public void ToggleSlotCamera()
    {
        if (_slotCamera != null && _slotCamera.enabled)
            DeactivateSlotCamera();
        else
            ActivateSlotCamera();
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