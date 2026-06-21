using UnityEngine;
using System.Collections.Generic;

public class MagneticField : MonoBehaviour
{
    [Header("Настройки магнита")]
    [SerializeField] private float _magneticForce = 20f;
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _stopDistance = 0.3f;
    [SerializeField] private LayerMask _magneticLayers = -1;

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem _magneticParticles;
    [SerializeField] private AudioClip _magneticSound;
    [SerializeField] private float _soundCooldown = 1f;
    [SerializeField] private AudioSource _audioSource;

    private List<Rigidbody> _objectsInField = new List<Rigidbody>();
    private float _lastSoundTime = 0f;
    private BoxCollider _boxCollider;

    private bool _isActive = true;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider == null)
        {
            Debug.LogError($"На магните {gameObject.name} нет BoxCollider!");
            return;
        }

        _boxCollider.isTrigger = true;

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    public void SetActive(bool active)
    {
        _isActive = active;

        if (!active)
        {
            ReleaseAllObjects();
            if (_magneticParticles != null && _magneticParticles.isPlaying)
                _magneticParticles.Stop();
        }
        else
        {
            if (_magneticParticles != null && !_magneticParticles.isPlaying)
                _magneticParticles.Play();
        }

        Debug.Log($"Магнит {gameObject.name} {(active ? "включен" : "выключен")}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        if (!IsMagneticObject(other)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !_objectsInField.Contains(rb))
        {
            _objectsInField.Add(rb);
            if (_audioSource != null && _magneticSound != null)
                _audioSource.PlayOneShot(_magneticSound, 0.5f);
            Debug.Log($"Объект {other.name} попал в магнитное поле");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive) return;
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
        if (!_isActive) return;

        for (int i = _objectsInField.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = _objectsInField[i];

            if (rb == null)
            {
                _objectsInField.RemoveAt(i);
                continue;
            }

            Vector3 closestPoint = GetClosestPointOnSurface(rb.position);
            float distance = Vector3.Distance(rb.position, closestPoint);

            if (distance <= _stopDistance)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                continue;
            }

            Vector3 direction = (closestPoint - rb.position).normalized;
            rb.AddForce(direction * _magneticForce, ForceMode.Force);

            if (rb.linearVelocity.magnitude > _maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * _maxSpeed;
            }

            if (_audioSource != null && _magneticSound != null && Time.time > _lastSoundTime + _soundCooldown)
            {
                _lastSoundTime = Time.time;
            }
        }
    }

    private Vector3 GetClosestPointOnSurface(Vector3 point)
    {
        Vector3 localPoint = transform.InverseTransformPoint(point);
        Vector3 halfSize = _boxCollider.size / 2f;
        Vector3 center = _boxCollider.center;

        float clampedX = Mathf.Clamp(localPoint.x, center.x - halfSize.x, center.x + halfSize.x);
        float clampedY = Mathf.Clamp(localPoint.y, center.y - halfSize.y, center.y + halfSize.y);
        float surfaceZ = center.z - halfSize.z;

        Vector3 closestLocal = new Vector3(clampedX, clampedY, surfaceZ);
        return transform.TransformPoint(closestLocal);
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
            Gizmos.color = new Color(1f, 0f, 1f, 0.2f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_boxCollider.center, _boxCollider.size);

            Gizmos.color = Color.blue;
            Vector3 center = transform.TransformPoint(_boxCollider.center);
            Gizmos.DrawRay(center, -transform.forward * 2f);
        }
    }
}