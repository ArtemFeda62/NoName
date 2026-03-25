using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    [Header("Свойства для размеров")]
    [SerializeField] private float _scaleStep = 0.1f;
    [SerializeField] private float _minScale = 0.1f;
    [SerializeField] private float _maxScale = 10f;

    private Vector3 _originalScale;
    private Vector3 _currentScale;

    private void Start()
    {
        InitializeSize();
    }

    private void InitializeSize()
    {
        _originalScale = transform.localScale;
        _currentScale = transform.localScale;
    }

    public void IncreaseSize()
    {
        if (_currentScale.x < _maxScale)
        {
            _currentScale += Vector3.one * _scaleStep;
            _currentScale = Vector3.Min(_currentScale, Vector3.one * _maxScale);
            Debug.Log($"Size increased to: {_currentScale.x}");
        }
        else
        {
            Debug.Log("Cannot increase size further - maximum size reached");
        }
    }

    public void DecreaseSize()
    {
        if (_currentScale.x > _minScale)
        {
            _currentScale -= Vector3.one * _scaleStep;
            _currentScale = Vector3.Max(_currentScale, Vector3.one * _minScale);
            Debug.Log($"Size decreased to: {_currentScale.x}");
        }
        else
        {
            Debug.Log("Cannot decrease size further - minimum size reached");
        }
    }

    public void ResetToOriginalSize()
    {
        _currentScale = _originalScale;
        Debug.Log("Size reset to original");
    }

    private void FixedUpdate()
    {
        ApplySizeChange();
    }

    private void ApplySizeChange()
    {
        if (transform.localScale != _currentScale)
        {
            transform.localScale = _currentScale;
        }
    }

    public float GetCurrentScale()
    {
        return _currentScale.x;
    }

    public float GetScaleProgress()
    {
        return (_currentScale.x - _minScale) / (_maxScale - _minScale);
    }
}