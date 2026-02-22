using System;
using UnityEngine;

public class ClickButton : MonoBehaviour
{
    [SerializeField] private float massForButton = 1.0f;
    private bool buttonClicked = false;
    public event Action OpenDoors;
    public void Update()
    {
        if (buttonClicked == false)
        {
            CheckButtonPress();
        }
    }

    private void CheckButtonPress()
    {

        Vector3 rayDirection = transform.up;
        Ray ray = new Ray(transform.position, rayDirection);

        Debug.DrawRay(transform.position, rayDirection * 0.2f, Color.blue);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.2f))
        {
            Selectable obj = hit.collider.gameObject.GetComponent<Selectable>();

            if (obj != null)
            {
                Rigidbody rb_obj = obj.GetComponent<Rigidbody>();
                if (rb_obj != null && rb_obj.mass >= massForButton)
                {
                    transform.position += new Vector3(0, -0.1f, 0);
                    buttonClicked = true;
                    OpenDoors?.Invoke();
                    Debug.Log("Кнопка нажата! Событие вызвано");
                }
            }
        }
    }
}