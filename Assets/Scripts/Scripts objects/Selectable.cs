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

    private Renderer _renderer;
    private Color _defaultColor;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            Mass = rb.mass;
        else
            Mass = 1f;

        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _defaultColor = _renderer.material.color;

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
    }

    public void Select()
    {
        if (_renderer != null)
            _renderer.material.color = Color.yellow;
    }

    public void Deselect()
    {
        if (_renderer != null)
            _renderer.material.color = Color.gray;
    }
}