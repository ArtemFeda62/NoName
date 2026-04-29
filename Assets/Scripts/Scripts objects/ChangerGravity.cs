using UnityEngine;

public class ChangerGravity : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Объект {other.gameObject.name} вошел в триггер");

        GravityChanger gravityChanger = other.gameObject.GetComponent<GravityChanger>();
        if (gravityChanger != null)
        {
            gravityChanger.ToggleGravity();
            Debug.Log("ToggleGravity вызван");
        }
        else
        {
            Debug.Log($"GravityChanger не найден на {other.gameObject.name}");
        }
    }
}