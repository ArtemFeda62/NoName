using UnityEngine;

public class ChangerSize : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Объект {other.gameObject.name} вошел в триггер");

        SizeChanger sizeChanger = other.gameObject.GetComponent<SizeChanger>();
        if (sizeChanger != null)
        {
            sizeChanger.SetOriginalScale();
            Debug.Log("SetOriginalScale вызван");
        }
        else
        {
            Debug.Log($"SizeChanger не найден на {other.gameObject.name}");
        }
    }
}