using UnityEngine;

public class PistonWithSlot : MonoBehaviour
{
    [Header("Настройки поршня")]
    [SerializeField] private Transform _pistonHead;
    [SerializeField] private float _extendDistance = 0.5f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _pauseDuration = 0.5f;
    [SerializeField] private float _pushForce = 50f;
    [SerializeField] private float _pushRadius = 1.5f;
    [SerializeField] private LayerMask _pushLayers = -1;

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem _extendParticles;
    [SerializeField] private AudioClip _extendSound;
    [SerializeField] private AudioSource _audioSource;

    [Header("Слот для лампы")]
    [SerializeField] private LampSlot _lampSlot;

    private Vector3 _retractedPosition;
    private Vector3 _extendedPosition;
    private bool _isMoving = false;
    private bool _isExtended = false;
    private float _pauseTimer = 0f;
    private bool _hasActivatedThisCycle = false;
    private bool _hasLampInSlot = false;

    private void Start()
    {
        if (_pistonHead == null)
            _pistonHead = transform;

        _retractedPosition = _pistonHead.localPosition;
        _extendedPosition = _retractedPosition + Vector3.forward * _extendDistance;

        if (_lampSlot != null)
        {
            _lampSlot.OnLampTurnedOn.AddListener(OnLampInSlotTurnedOn);
            _lampSlot.OnLampTurnedOff.AddListener(OnLampInSlotTurnedOff);
        }
    }

    private void Update()
    {
        if (_lampSlot != null)
        {
            _hasLampInSlot = _lampSlot.HasLamp;
        }

        if (_pauseTimer > 0)
        {
            _pauseTimer -= Time.deltaTime;
            if (_pauseTimer <= 0)
            {
                RetractPiston();
                _hasActivatedThisCycle = false;
            }
        }

        if (_isMoving)
        {
            Vector3 targetPos = _isExtended ? _extendedPosition : _retractedPosition;
            Vector3 newPos = Vector3.MoveTowards(_pistonHead.localPosition, targetPos, _moveSpeed * Time.deltaTime);
            _pistonHead.localPosition = newPos;

            if (Vector3.Distance(_pistonHead.localPosition, targetPos) < 0.01f)
            {
                _isMoving = false;
                _pistonHead.localPosition = targetPos;

                if (_isExtended)
                {
                    PushObjects();
                    _pauseTimer = _pauseDuration;
                }
            }
        }
    }

    private void OnLampInSlotTurnedOn()
    {
        if (!_hasLampInSlot)
        {
            Debug.Log($"Поршень {gameObject.name}: получен сигнал включения, но лампы нет в слоте! ИГНОРИРУЮ");
            return;
        }

        if (!_hasActivatedThisCycle)
        {
            ExtendPiston();
            _hasActivatedThisCycle = true;
            Debug.Log($"Лампа в слоте поршня {gameObject.name} включилась -> поршень выдвигается");
        }
    }

    private void OnLampInSlotTurnedOff()
    {
        Debug.Log($"Лампа в слоте поршня {gameObject.name} выключилась");
    }

    private void ExtendPiston()
    {
        if (_isExtended || _isMoving || _pauseTimer > 0) return;

        _isExtended = true;
        _isMoving = true;

        if (_extendParticles != null)
            _extendParticles.Play();

        if (_audioSource != null && _extendSound != null)
            _audioSource.PlayOneShot(_extendSound);

        Debug.Log($"Поршень {gameObject.name} выдвигается");
    }

    private void RetractPiston()
    {
        if (!_isExtended) return;

        _isExtended = false;
        _isMoving = true;

        Debug.Log($"Поршень {gameObject.name} возвращается");
    }

    private void PushObjects()
    {
        Vector3 pushCenter = _pistonHead.position + _pistonHead.forward * (_extendDistance / 2);

        Debug.Log($"Толчок в центре: {pushCenter}, радиус: {_pushRadius}");

        Collider[] hitColliders = Physics.OverlapSphere(pushCenter, _pushRadius, _pushLayers);

        Debug.Log($"Найдено объектов в радиусе: {hitColliders.Length}");

        foreach (Collider hit in hitColliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && hit.gameObject != gameObject)
            {
                Vector3 pushDirection = (hit.transform.position - pushCenter).normalized;
                pushDirection.y = 0;
                pushDirection.Normalize();

                rb.AddForce(pushDirection * _pushForce, ForceMode.Impulse);
                Debug.Log($"Поршень {gameObject.name} толкнул объект {hit.gameObject.name} силой {_pushForce}, направление: {pushDirection}");
            }
        }
    }

    private void OnDestroy()
    {
        if (_lampSlot != null)
        {
            _lampSlot.OnLampTurnedOn.RemoveListener(OnLampInSlotTurnedOn);
            _lampSlot.OnLampTurnedOff.RemoveListener(OnLampInSlotTurnedOff);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_pistonHead != null)
        {
            Vector3 pushCenter = _pistonHead.position + _pistonHead.forward * (_extendDistance / 2);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pushCenter, _pushRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pushCenter, 0.2f);
        }
    }
}