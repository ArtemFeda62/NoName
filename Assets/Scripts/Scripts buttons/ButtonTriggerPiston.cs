using System;
using UnityEngine;

public class ButtonTriggerPiston : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _requiredMass = 1.0f;
    [SerializeField] private float _pressDistance = 0.1f;

    public event Action OnPiston;

    private bool _isPressed;

    private void Update()
    {
        if (!_isPressed)
        {
            CheckForButtonPress();
        }
    }

    private void CheckForButtonPress()
    {
        Vector3 rayDirection = transform.up;
        Ray ray = new Ray(transform.position, rayDirection);
        Debug.DrawRay(ray.origin, rayDirection * 0.2f, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.2f))
        {
            Selectable selectable = hit.collider.GetComponent<Selectable>();

            if (selectable != null)
            {
                Rigidbody rb = selectable.GetComponent<Rigidbody>();
                if (rb != null && rb.mass >= _requiredMass)
                {
                    PressButton();
                }
            }
        }
    }

    private void PressButton()
    {
        transform.position += new Vector3(0, -_pressDistance, 0);
        _isPressed = true;
        OnPiston?.Invoke();
    }
}