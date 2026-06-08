using UnityEngine;

public class Selectable : MonoBehaviour
{
    [Header("Компоненты")]
    public Rigidbody rb;
    public float Mass;

    [Header("Звуки")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private float _minVelocityForSound = 5f;
    [SerializeField] private float _maxVolumeVelocity = 10f;

    [Header("Неоновые осколки")]
    [SerializeField] private GameObject _neonDebrisPrefab;  
    [SerializeField] private int _debrisCount = 6;        
    [SerializeField] private float _debrisForce = 4f;      
    [SerializeField] private float _minVelocityForDebris = 10f; 

    private Renderer _renderer;
    private Material _material;
    private Color _color;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            Mass = rb.mass;
        else
            Mass = 1f;

        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _material = _renderer.material;
            _color = _renderer.material.color;
        }

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float velocity = collision.relativeVelocity.magnitude;

        if (velocity >= _minVelocityForSound && _hitSound != null && _audioSource != null)
        {
            float volume = Mathf.Clamp01(velocity / _maxVolumeVelocity);
            _audioSource.PlayOneShot(_hitSound, volume);
        }

        if (velocity >= _minVelocityForDebris && _neonDebrisPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;
            SpawnNeonDebris(hitPoint, hitNormal);
        }
    }

    private void SpawnNeonDebris(Vector3 position, Vector3 normal)
    {
        for (int i = 0; i < _debrisCount; i++)
        {
            GameObject debris = Instantiate(_neonDebrisPrefab, position, Random.rotation);

            float randomScale = Random.Range(0.15f, 0.35f);
            debris.transform.localScale = Vector3.one * randomScale;

            Rigidbody rbDebris = debris.GetComponent<Rigidbody>();
            if (rbDebris != null)
            {
                Vector3 direction = normal + Random.insideUnitSphere;
                direction.Normalize();
                rbDebris.AddForce(direction * _debrisForce, ForceMode.Impulse);

                rbDebris.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);
            }
        }
    }

    public void Select()
    {
        if (_renderer != null)
            _renderer.material.color = Color.yellow;
    }

    public void Deselect()
    {
        if (_renderer != null)
        {
            _renderer.material = _material;
            _renderer.material.color = _color;
        }
    }
}