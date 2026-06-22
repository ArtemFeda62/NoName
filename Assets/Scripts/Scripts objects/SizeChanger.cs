using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    [Header("Свойства для размеров")]
    [SerializeField] private float _scaleStep = 5f;
    [SerializeField] private float _minScale = 50f;
    [SerializeField] private float _maxScale = 250f;

    [Header("Проверка столкновений")]
    [SerializeField] private LayerMask _obstacleLayer = ~0;
    [SerializeField] private float _checkDistance = 0.1f;

    private Vector3 _originalScale;
    private Vector3 _currentScale;

    void Start()
    {
        _originalScale = transform.localScale;
        _currentScale = transform.localScale;
    }

    void FixedUpdate()
    {
        transform.localScale = _currentScale;
    }

    public void IncreaseSize()
    {
        if (_currentScale.x >= _maxScale) return;

        Vector3 newScale = _currentScale + Vector3.one * _scaleStep;
        newScale = Vector3.Min(newScale, Vector3.one * _maxScale);

        if (!IsSqueezed(newScale))
        {
            _currentScale = newScale;
        }
    }

    public void DecreaseSize()
    {
        if (_currentScale.x > _minScale)
        {
            _currentScale -= Vector3.one * _scaleStep;
            _currentScale = Vector3.Max(_currentScale, Vector3.one * _minScale);
        }
    }

    public void ResetToOriginalSize()
    {
        _currentScale = _originalScale;
    }

    private bool IsSqueezed(Vector3 newScale)
    {
        Vector3 oldScale = transform.localScale;
        transform.localScale = newScale;

        float halfSize = GetComponent<Collider>().bounds.extents.x;

        bool squeezedX = HasOppositeObjects(Vector3.right, halfSize) && HasOppositeObjects(Vector3.left, halfSize);
        bool squeezedY = HasOppositeObjects(Vector3.up, halfSize) && HasOppositeObjects(Vector3.down, halfSize);
        bool squeezedZ = HasOppositeObjects(Vector3.forward, halfSize) && HasOppositeObjects(Vector3.back, halfSize);

        transform.localScale = oldScale;

        return squeezedX || squeezedY || squeezedZ;
    }

    private bool HasOppositeObjects(Vector3 direction, float distance)
    {
        return Physics.Raycast(
            transform.position,
            direction,
            distance + _checkDistance,
            _obstacleLayer
        );
    }

    public float GetCurrentScale() => _currentScale.x;
    public float GetScaleProgress() => (_currentScale.x - _minScale) / (_maxScale - _minScale);
    public void SetOriginalScale() => _currentScale = _originalScale;
}