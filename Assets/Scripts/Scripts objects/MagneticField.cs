using UnityEngine;
using System.Collections.Generic;

public class MagneticField : MonoBehaviour
{
    [Header("Настройки магнита")]
    [SerializeField] private float _magneticForce = 20f;
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _stopDistance = 0.5f;
    [SerializeField] private LayerMask _magneticLayers = -1;

    [Header("Точки притяжения")]
    [SerializeField] private Transform _attractionPoint;
    [SerializeField] private Vector3 _attractionOffset = new Vector3(0, 0.5f, 0);

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem _magneticParticles;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _magneticSound;
    [SerializeField] private float _soundCooldown = 1f;

    private List<Rigidbody> _objectsInField = new List<Rigidbody>();
    private float _lastSoundTime = 0f;
    private BoxCollider _boxCollider;
    private Vector3 _attractionCenter;

    private void Start()
    {
        if (_attractionPoint == null)
            _attractionPoint = transform;

        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider == null)
        {
            Debug.LogError($"На магните {gameObject.name} нет BoxCollider!");
            return;
        }

        _boxCollider.isTrigger = true;

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        UpdateAttractionCenter();
    }

    private void UpdateAttractionCenter()
    {
        Vector3 center = _boxCollider.center;
        Vector3 size = _boxCollider.size;

        Vector3 localSurfacePoint = center + new Vector3(0, size.y / 2, 0);

        _attractionCenter = transform.TransformPoint(localSurfacePoint) + _attractionOffset;

        if (_attractionPoint != null && _attractionPoint != transform)
        {
            _attractionCenter = _attractionPoint.position + _attractionOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsMagneticObject(other)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !_objectsInField.Contains(rb))
        {
            _objectsInField.Add(rb);
            Debug.Log($"Объект {other.name} попал в магнитное поле");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_objectsInField.Contains(other.attachedRigidbody)) return;

        if (!IsMagneticObject(other)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !_objectsInField.Contains(rb))
        {
            _objectsInField.Add(rb);
            Debug.Log($"Объект {other.name} обнаружен в магнитном поле (Stay)");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsMagneticObject(other)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && _objectsInField.Contains(rb))
        {
            _objectsInField.Remove(rb);
            Debug.Log($"Объект {other.name} покинул магнитное поле");
        }
    }

    private void FixedUpdate()
    {
        UpdateAttractionCenter();

        for (int i = _objectsInField.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = _objectsInField[i];

            if (rb == null)
            {
                _objectsInField.RemoveAt(i);
                continue;
            }

            Vector3 direction = (_attractionCenter - rb.position).normalized;
            float distance = Vector3.Distance(rb.position, _attractionCenter);

            if (distance <= _stopDistance)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                continue;
            }

            rb.AddForce(direction * _magneticForce, ForceMode.Force);

            if (rb.linearVelocity.magnitude > _maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * _maxSpeed;
            }

            if (_magneticParticles != null && !_magneticParticles.isPlaying)
                _magneticParticles.Play();

            if (_audioSource != null && _magneticSound != null && Time.time > _lastSoundTime + _soundCooldown)
            {
                _audioSource.PlayOneShot(_magneticSound, 0.3f);
                _lastSoundTime = Time.time;
            }
        }
    }

    private bool IsMagneticObject(Collider other)
    {
        if (((1 << other.gameObject.layer) & _magneticLayers) == 0)
            return false;

        Selectable selectable = other.GetComponent<Selectable>();
        if (selectable == null && other.attachedRigidbody != null)
            selectable = other.attachedRigidbody.GetComponent<Selectable>();

        if (other.attachedRigidbody == null)
            return false;

        return selectable != null;
    }

    public void ReleaseAllObjects()
    {
        _objectsInField.Clear();
        Debug.Log("Магнит отпустил все объекты");
    }

    private void OnDrawGizmosSelected()
    {
        if (_boxCollider != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 center = transform.TransformPoint(_boxCollider.center);
            Vector3 size = _boxCollider.size;
            Vector3 topCenter = center + transform.up * (size.y / 2);

            Gizmos.DrawWireSphere(topCenter + _attractionOffset, _stopDistance);

            Gizmos.color = new Color(1f, 0f, 1f, 0.2f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_boxCollider.center, _boxCollider.size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(center, topCenter + _attractionOffset);
        }
    }
}