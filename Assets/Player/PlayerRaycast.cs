using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Transform _grabPoint;
    [SerializeField] private LayerMask _ignoreMask;
    [SerializeField] private Animator _handAnimator;

    private string _currentTool = "size";
    private Selectable _currentSelectable;
    private Selectable _pickedItem;
    private bool _isItemPicked;
    private const float _rayDistance = 5f;

    private ConfigurableJoint _grabJoint;

    public Selectable CurrentSelectable => _currentSelectable;
    public string CurrentTool => _currentTool;
    public bool IsItemPicked => _isItemPicked;

    private void Start()
    {
        _handAnimator = GetComponent<Animator>();

        if (_handAnimator != null)
        {
            Debug.Log("Animator найден на объекте: " + gameObject.name);
            _handAnimator.SetBool("isHolding", false);
        }
        else
        {
            Debug.LogError("Animator НЕ найден на объекте: " + gameObject.name + "! Перетащите Animator в поле Hand Animator в инспекторе");
        }
    }

    private void LateUpdate()
    {
        LookAtSelectableObject();
    }

    public bool CanPickupCurrent()
    {
        return !_isItemPicked && _currentSelectable != null && _currentSelectable.GetComponent<Rigidbody>() != null;
    }

    private void LookAtSelectableObject()
    {
        if (_isItemPicked) return;

        Vector3 rayDirection = _cinemachineCamera.transform.forward;
        Ray ray = new Ray(_cinemachineCamera.transform.position, rayDirection);
        Debug.DrawRay(_cinemachineCamera.transform.position, rayDirection * _rayDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, ~_ignoreMask))
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
        Debug.Log("Выбран инструмент: изменение размера");
    }

    public void OnToolGravity()
    {
        _currentTool = "gravity";
        Debug.Log("Выбран инструмент: изменение гравитации");
    }

    public void OnInteract()
    {
        if (_isItemPicked) return;

        if (_currentTool == "gravity" && _currentSelectable != null)
        {
            GravityChanger gravityChanger = _currentSelectable.GetComponent<GravityChanger>();
            if (gravityChanger != null)
                gravityChanger.ToggleGravity();
        }
    }

    public void OnScaleDecrease()
    {
        if (_isItemPicked) return;
        if (_currentTool == "size" && _currentSelectable != null)
        {
            SizeChanger sizeChanger = _currentSelectable.GetComponent<SizeChanger>();
            if (sizeChanger != null)
                sizeChanger.DecreaseSize();
        }
    }

    public void OnScaleIncrease()
    {
        if (_isItemPicked) return;
        if (_currentTool == "size" && _currentSelectable != null)
        {
            SizeChanger sizeChanger = _currentSelectable.GetComponent<SizeChanger>();
            if (sizeChanger != null)
                sizeChanger.IncreaseSize();
        }
    }

    public void OnPickupButton()
    {
        Debug.Log("OnPickupButton вызван, isItemPicked = " + _isItemPicked + ", currentSelectable = " + (_currentSelectable != null ? _currentSelectable.name : "null"));

        if (!_isItemPicked && _currentSelectable != null)
        {
            PickupItemPhysical();
        }
        else if (_isItemPicked)
        {
            DropItemPhysical();
        }
    }

    private void PickupItemPhysical()
    {
        Debug.Log("PickupItemPhysical начат");

        Rigidbody rb = _currentSelectable.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("У предмета нет Rigidbody");
            return;
        }

        _currentSelectable.transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        _grabJoint = _currentSelectable.gameObject.AddComponent<ConfigurableJoint>();
        _grabJoint.connectedBody = _grabPoint.GetComponent<Rigidbody>();

        _grabJoint.xMotion = ConfigurableJointMotion.Limited;
        _grabJoint.yMotion = ConfigurableJointMotion.Limited;
        _grabJoint.zMotion = ConfigurableJointMotion.Limited;

        _grabJoint.linearLimit = new SoftJointLimit { limit = 0.1f };
        _grabJoint.linearLimitSpring = new SoftJointLimitSpring { spring = 1000f, damper = 500f };

        JointDrive positionDrive = new JointDrive
        {
            positionSpring = 1000f,
            positionDamper = 100f,
            maximumForce = Mathf.Infinity
        };

        _grabJoint.xDrive = positionDrive;
        _grabJoint.yDrive = positionDrive;
        _grabJoint.zDrive = positionDrive;

        _grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularZMotion = ConfigurableJointMotion.Locked;

        _grabJoint.anchor = Vector3.zero;
        _grabJoint.autoConfigureConnectedAnchor = false;
        _grabJoint.connectedAnchor = Vector3.zero;
        _grabJoint.enableCollision = true;

        _pickedItem = _currentSelectable;
        _isItemPicked = true;

        if (_currentSelectable != null)
        {
            _currentSelectable.Deselect();
            _currentSelectable = null;
        }

        Debug.Log("Попытка включить анимацию, _handAnimator = " + (_handAnimator != null ? "не null" : "null"));
        if (_handAnimator != null)
        {
            _handAnimator.SetBool("isHolding", true);
            Debug.Log("Анимация включена (isHolding = true)");
        }
        else
        {
            Debug.LogError("_handAnimator == null! Не удалось включить анимацию");
        }

        Debug.Log("Предмет поднят");
    }

    private void DropItemPhysical()
    {
        Debug.Log("DropItemPhysical начат");

        if (_pickedItem == null) return;

        if (_grabJoint != null)
            Destroy(_grabJoint);

        Rigidbody rb = _pickedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        _pickedItem = null;
        _isItemPicked = false;
        _grabJoint = null;

        if (_handAnimator != null)
        {
            _handAnimator.SetBool("isHolding", false);
            Debug.Log("Анимация выключена (isHolding = false)");
        }
        else
        {
            Debug.LogError("_handAnimator == null! Не удалось выключить анимацию");
        }

        Debug.Log("Предмет отпущен");
    }
}