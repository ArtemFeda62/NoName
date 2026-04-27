using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [SerializeField] private float _openHeight = 3f; 
    [SerializeField] private float _moveDuration = 0.5f;

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private Coroutine _currentMove;

    private void Start()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + transform.up * _openHeight;
    }

    public void Open()
    {
        if (_currentMove != null) StopCoroutine(_currentMove);
        _currentMove = StartCoroutine(MoveTo(_openPos));
    }

    public void Close()
    {
        if (_currentMove != null) StopCoroutine(_currentMove);
        _currentMove = StartCoroutine(MoveTo(_closedPos));
    }

    private IEnumerator MoveTo(Vector3 targetPos)
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
        _currentMove = null;
    }
}