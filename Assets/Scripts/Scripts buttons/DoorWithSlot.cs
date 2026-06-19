using UnityEngine;

public class DoorWithSlot : MonoBehaviour
{
    [Header("Настройки двери")]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private Vector3 _openPosition = new Vector3(0, 0, 2f);
    [SerializeField] private float _moveSpeed = 3f;

    [Header("Звуки")]
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;
    [SerializeField] private AudioSource _audioSource;

    [Header("Слот для лампы")]
    [SerializeField] private LampSlot _lampSlot;

    private Vector3 _closedPosition;
    private bool _isOpen = false;
    private bool _isMoving = false;
    private bool _hasLampInSlot = false;
    private bool _isClosing = false;

    private void Start()
    {
        if (_doorTransform == null)
            _doorTransform = transform;

        _closedPosition = _doorTransform.localPosition;

        if (_lampSlot != null)
        {
            _lampSlot.OnLampTurnedOn.AddListener(OnLampInSlotTurnedOn);
            _lampSlot.OnLampTurnedOff.AddListener(OnLampInSlotTurnedOff);
            Debug.Log($"ДВЕРЬ {gameObject.name}: Подписана на события слота");
        }
        else
        {
            Debug.LogError($"ДВЕРЬ {gameObject.name}: Нет ссылки на LampSlot!");
        }
    }

    private void Update()
    {
        if (_lampSlot != null)
        {
            _hasLampInSlot = _lampSlot.HasLamp;
        }

        if (_isMoving)
        {
            Vector3 targetPos = _isOpen ? _openPosition : _closedPosition;
            Vector3 newPos = Vector3.MoveTowards(_doorTransform.localPosition, targetPos, _moveSpeed * Time.deltaTime);
            _doorTransform.localPosition = newPos;

            if (Vector3.Distance(_doorTransform.localPosition, targetPos) < 0.01f)
            {
                _isMoving = false;
                _doorTransform.localPosition = targetPos;
                _isClosing = false;
            }
        }
    }

    private void OnLampInSlotTurnedOn()
    {
        if (!_hasLampInSlot)
        {
            Debug.Log($"ДВЕРЬ {gameObject.name}: лампы нет в слоте");
            return;
        }

        Debug.Log($"ДВЕРЬ {gameObject.name}: лампа в слоте, открываю");
        OpenDoor();
    }

    private void OnLampInSlotTurnedOff()
    {
        Debug.Log($"ДВЕРЬ {gameObject.name}: лампа выключилась");

        if (_isOpen || _isMoving)
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        if (_isOpen && !_isMoving)
        {
            Debug.Log($"ДВЕРЬ {gameObject.name}: уже открыта");
            return;
        }

        if (_isMoving && !_isClosing)
        {
            Debug.Log($"ДВЕРЬ {gameObject.name}: уже открывается");
            return;
        }

        _isOpen = true;
        _isMoving = true;
        _isClosing = false;

        if (_audioSource != null && _openSound != null)
            _audioSource.PlayOneShot(_openSound);

        Debug.Log($"ДВЕРЬ {gameObject.name}: ОТКРЫТА");
    }

    private void CloseDoor()
    {
        if (!_isOpen && !_isMoving)
        {
            Debug.Log($"ДВЕРЬ {gameObject.name}: уже закрыта");
            return;
        }

        if (_isMoving && _isClosing)
        {
            Debug.Log($"ДВЕРЬ {gameObject.name}: уже закрывается");
            return;
        }

        _isOpen = false;
        _isMoving = true;
        _isClosing = true;

        if (_audioSource != null && _closeSound != null)
            _audioSource.PlayOneShot(_closeSound);

        Debug.Log($"ДВЕРЬ {gameObject.name}: ЗАКРЫТА");
    }

    private void OnDestroy()
    {
        if (_lampSlot != null)
        {
            _lampSlot.OnLampTurnedOn.RemoveListener(OnLampInSlotTurnedOn);
            _lampSlot.OnLampTurnedOff.RemoveListener(OnLampInSlotTurnedOff);
        }
    }
}