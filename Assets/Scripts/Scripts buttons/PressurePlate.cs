using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.InputSystem.Controls.AxisControl;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private float _massObject = 20f;
    [SerializeField] private Vector3 _positionDown;
    [SerializeField] private Vector3 _positionStart;
    [SerializeField] private bool _isPressed = false;

    [Header("События")]
    public UnityEvent _onPressed; 
    public UnityEvent _onReleased;

    private void Start()
    {
        _positionStart = transform.position;
        _positionDown = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody.mass >= _massObject)
        {
            _isPressed = true;
            UpdateButton();
            _onPressed?.Invoke();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        _isPressed = false;
        UpdateButton();
        _onReleased?.Invoke();
    }

    private void UpdateButton()
    {
        if(_isPressed)
        {
            gameObject.transform.localPosition = Vector3.Lerp(transform.position, _positionDown, Time.deltaTime * 2f);
        }
        else if(_isPressed==false)
        {
            gameObject.transform.localPosition = Vector3.Lerp(transform.position, _positionStart, Time.deltaTime * 2f);
        }
    }
}