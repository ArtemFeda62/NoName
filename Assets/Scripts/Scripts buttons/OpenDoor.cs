using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private ClickButton _button;
    private bool startOpen = false;
    [SerializeField] private Vector3 _positionOpen = new Vector3(0, 5f, 0); 
    private Vector3 _startPosition;
    void Start()
    {
        _startPosition = transform.position;
        if (_button != null)
        {
            _button.OpenDoors += StartOpenDoor;
            Debug.Log("Подписка на событие кнопки");
        }
    }

    public void StartOpenDoor()
    {
        startOpen = true;
        Debug.Log("Дверь начала открываться!");
    }

    public void Update()
    {
        if (startOpen)
        {
            Open();
        }
    }

    private void Open()
    {
        transform.position += new Vector3(0, 0.01f, 0);

        if (transform.position.y >= _positionOpen.y)
        { 
            startOpen = false;
            Debug.Log("Дверь полностью открыта");
        }
    }
}