using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ButtonTrigger _buttonTrigger;
    [SerializeField] private Vector3 _openPosition = new Vector3(0, 5f, 0);
    [SerializeField] private float _openSpeed = 0.01f;

    private Vector3 _startPosition;
    private bool _isOpening;

    private void Start()
    {
        _startPosition = transform.position;

        if (_buttonTrigger != null)
        {
            _buttonTrigger.OnDoorOpen += StartOpening;
        }
    }

    public void StartOpening()
    {
        _isOpening = true;
    }

    private void Update()
    {
        if (_isOpening)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        transform.position += new Vector3(0, _openSpeed, 0);

        if (transform.position.y >= _openPosition.y)
        {
            _isOpening = false;
        }
    }

    private void OnDestroy()
    {
        if (_buttonTrigger != null)
        {
            _buttonTrigger.OnDoorOpen -= StartOpening;
        }
    }
}