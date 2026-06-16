using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Настройки лазера")]
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _collisionLayers = -1;
    [SerializeField] private bool _infiniteLength = true;
    [SerializeField] private float _damageInterval = 0.1f;

    [Header("Визуал")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Material _laserMaterial;
    [SerializeField] private Color _laserColor = Color.red;
    [SerializeField] private float _laserWidth = 0.05f;

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hitSound;

    private Vector3 _laserEndPoint;
    private GameObject _currentHitObject;
    private LaserDestroyable _currentDestroyable;
    private float _damageTimer = 0f;
    private bool _isActive = true;

    private void Start()
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    private void SetupLineRenderer()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;

        if (_laserMaterial != null)
            _lineRenderer.material = _laserMaterial;

        _lineRenderer.startColor = _laserColor;
        _lineRenderer.endColor = _laserColor;
        _lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        if (!_isActive) return;

        UpdateLaser();

        if (_currentDestroyable != null)
        {
            _damageTimer += Time.deltaTime;

            if (_damageTimer >= _damageInterval)
            {
                _damageTimer = 0f;
                _currentDestroyable.OnLaserHit();
            }
        }
    }

    private void UpdateLaser()
    {
        Vector3 startPoint = transform.position;
        Vector3 direction = transform.forward;

        Ray ray = new Ray(startPoint, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxDistance, _collisionLayers))
        {
            _laserEndPoint = hit.point;
            _currentHitObject = hit.collider.gameObject;

            _currentDestroyable = _currentHitObject.GetComponent<LaserDestroyable>();

            if (_hitParticles != null)
            {
                _hitParticles.transform.position = hit.point;
                _hitParticles.transform.rotation = Quaternion.LookRotation(hit.normal);

                if (!_hitParticles.isPlaying)
                    _hitParticles.Play();
            }

            if (_audioSource != null && _hitSound != null && !_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(_hitSound, 0.3f);
            }
        }
        else
        {
            if (_infiniteLength)
                _laserEndPoint = startPoint + direction * _maxDistance;
            else
                _laserEndPoint = startPoint + direction * _maxDistance;

            _currentHitObject = null;
            _currentDestroyable = null;

            if (_hitParticles != null && _hitParticles.isPlaying)
                _hitParticles.Stop();
        }

        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, _laserEndPoint);
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        _lineRenderer.enabled = active;

        if (!active)
        {
            if (_hitParticles != null && _hitParticles.isPlaying)
                _hitParticles.Stop();
            _currentHitObject = null;
            _currentDestroyable = null;
        }
    }

    public void ToggleLaser()
    {
        SetActive(!_isActive);
    }

    public void SetColor(Color color)
    {
        _laserColor = color;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    public void SetWidth(float width)
    {
        _laserWidth = width;
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * _maxDistance);
    }
}