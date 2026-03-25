using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    private string _currentTool = "size";
    private Selectable _currentSelectable;
    private Selectable _pickedItem;
    private bool _isItemPicked;
    private float _itemPositionOnCamera;

    private void LateUpdate()
    {
        LookAtSelectableObject();
    }

    private void LookAtSelectableObject()
    {
        Vector3 rayDirection = _cinemachineCamera.transform.forward;
        Ray ray = new Ray(_cinemachineCamera.transform.position, rayDirection);
        Debug.DrawRay(_cinemachineCamera.transform.position, rayDirection * 5f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Selectable selectable = hit.collider.GetComponent<Selectable>();
            HandleSelectable(selectable);
        }
        else
        {
            ClearCurrentSelection();
        }
    }

    private void HandleSelectable(Selectable selectable)
    {
        if (selectable)
        {
            if (_currentSelectable && _currentSelectable != selectable)
            {
                _currentSelectable.Deselect();
                _currentSelectable = null;
            }

            if (_currentSelectable == null)
            {
                _currentSelectable = selectable;
                _currentSelectable.Select();
            }
        }
        else
        {
            ClearCurrentSelection();
        }
    }

    private void ClearCurrentSelection()
    {
        if (_currentSelectable)
        {
            _currentSelectable.Deselect();
            _currentSelectable = null;
        }
    }

    public void OnToolSize()
    {
        _currentTool = "size";
        Debug.Log("Выбран инструмент меняющий размер");
    }

    public void OnToolGravity()
    {
        _currentTool = "gravity";
        Debug.Log("Выбран инструмент меняющий гравитацию");
    }

    public void OnInteract()
    {
        if (_currentTool == "gravity" && _currentSelectable != null)
        {
            GravityChanger gravityChanger = _currentSelectable.GetComponent<GravityChanger>();
            if (gravityChanger != null)
            {
                gravityChanger.ToggleGravity();
            }
        }
    }

    public void OnScaleDecrease()
    {
        if (_currentTool == "size" && _currentSelectable != null)
        {
            SizeChanger sizeChanger = _currentSelectable.GetComponent<SizeChanger>();
            if (sizeChanger != null)
            {
                sizeChanger.DecreaseSize();
            }
        }
    }

    public void OnScaleIncrease()
    {
        if (_currentTool == "size" && _currentSelectable != null)
        {
            SizeChanger sizeChanger = _currentSelectable.GetComponent<SizeChanger>();
            if (sizeChanger != null)
            {
                sizeChanger.IncreaseSize();
            }
        }
    }

    public void OnPickupButton()
    {
        if (!_isItemPicked && _currentSelectable != null)
        {
            PickupItem();
        }
        else if (_isItemPicked)
        {
            DropItem();
        }
    }

    private void PickupItem()
    {
        _currentSelectable.transform.SetParent(_cinemachineCamera.transform);
        _itemPositionOnCamera = _currentSelectable.transform.localScale.x + 2f;
        _currentSelectable.transform.localPosition = new Vector3(0, 0, _itemPositionOnCamera);
        _currentSelectable.transform.localRotation = Quaternion.identity;

        _currentSelectable.GetComponent<Rigidbody>().isKinematic = true;
        _currentSelectable.GetComponent<Collider>().enabled = false;

        _pickedItem = _currentSelectable;
        _isItemPicked = true;
        Debug.Log("Предмет поднят!");
    }

    private void DropItem()
    {
        _pickedItem.transform.SetParent(null);
        _pickedItem.GetComponent<Rigidbody>().isKinematic = false;
        _pickedItem.GetComponent<Collider>().enabled = true;

        _isItemPicked = false;
        _pickedItem = null;
        Debug.Log("Предмет отпущен!");
    }
}