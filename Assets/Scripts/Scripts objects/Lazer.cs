using UnityEngine;
using UnityEngine.SceneManagement;

public class Laser : MonoBehaviour
{
    [Header("Настройки лазера")]
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _collisionLayers = -1;
    [SerializeField] private bool _infiniteLength = true;

    [Header("Визуал")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Material _laserMaterial;
    [SerializeField] private Color _laserColor = Color.red;
    [SerializeField] private float _laserWidth = 0.05f;

    [Header("Эффекты")]
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private AudioClip _hitSound;
    private AudioSource _audioSource;

    private Vector3 _laserEndPoint;
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

            // Проверяем игрока
            PlayerController player = hit.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                PlayerDeath playerDeath = hit.collider.GetComponent<PlayerDeath>();
                if (playerDeath != null)
                {
                    playerDeath.TeleportToCheckpoint();
                }
                return;
            }

            // Проверяем объект с LaserDestroyable
            LaserDestroyable destroyable = hit.collider.GetComponent<LaserDestroyable>();
            if (destroyable != null)
            {
                destroyable.OnLaserHit();

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
                if (_hitParticles != null && _hitParticles.isPlaying)
                    _hitParticles.Stop();
            }
        }
        else
        {
            _laserEndPoint = startPoint + direction * _maxDistance;

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