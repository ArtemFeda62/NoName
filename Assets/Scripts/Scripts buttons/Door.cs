using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [SerializeField] private float _openHeight = 3f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private bool _startOpen = false;

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private Coroutine _currentMove;
    private bool _isOpen = false;

    private void Start()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + transform.up * _openHeight;

        if (_startOpen)
        {
            transform.position = _openPos;
            _isOpen = true;
        }
    }

    public void Open()
    {
        if (_isOpen) return;
        if (_currentMove != null) StopCoroutine(_currentMove);
        _currentMove = StartCoroutine(MoveTo(_openPos, true));
    }

    public void Close()
    {
        if (!_isOpen) return;
        if (_currentMove != null) StopCoroutine(_currentMove);
        _currentMove = StartCoroutine(MoveTo(_closedPos, false));
    }

    public void Toggle()
    {
        if (_isOpen)
            Close();
        else
            Open();
    }

    private IEnumerator MoveTo(Vector3 targetPos, bool open)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < _moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _moveDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        _isOpen = open;
        _currentMove = null;
    }
}