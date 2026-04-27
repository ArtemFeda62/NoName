using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Transform _grabPoint;     // точка захвата (дочерний объект камеры с Rigidbody Kinematic)

    private string _currentTool = "size";
    private Selectable _currentSelectable;
    private Selectable _pickedItem;
    private bool _isItemPicked;

    // Joint для захваченного предмета
    private ConfigurableJoint _grabJoint;

    private void LateUpdate()
    {
        LookAtSelectableObject();
    }

    private void LookAtSelectableObject()
    {
        // Когда предмет в руках — не подсвечиваем новые объекты
        if (_isItemPicked) return;

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

    // ----- Инструменты -----
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

    // ----- Захват / бросок -----
    public void OnPickupButton()
    {
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
        Rigidbody rb = _currentSelectable.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("У предмета нет Rigidbody! Нельзя поднять.");
            return;
        }

        // Открепляем от возможного родителя
        _currentSelectable.transform.SetParent(null);

        // Настройка физики предмета
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Collider col = _currentSelectable.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // Создаём ConfigurableJoint
        _grabJoint = _currentSelectable.gameObject.AddComponent<ConfigurableJoint>();
        _grabJoint.connectedBody = _grabPoint.GetComponent<Rigidbody>();

        // ----- Линейные ограничения (свободное движение с возвратом) -----
        // ----- Линейные ограничения (минимальный люфт, сильное гашение) -----
        _grabJoint.xMotion = ConfigurableJointMotion.Limited;
        _grabJoint.yMotion = ConfigurableJointMotion.Limited;
        _grabJoint.zMotion = ConfigurableJointMotion.Limited;

        _grabJoint.linearLimit = new SoftJointLimit { limit = 0.01f };
        _grabJoint.linearLimitSpring = new SoftJointLimitSpring { spring = 150f, damper = 100f };

        // ----- Угловые ограничения (полная блокировка) -----
        _grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularZMotion = ConfigurableJointMotion.Locked;

        // ----- Добавляем стабилизацию привода (опционально) -----
        _grabJoint.xDrive = new JointDrive { positionSpring = 0f, positionDamper = 0f, maximumForce = Mathf.Infinity };
        _grabJoint.yDrive = new JointDrive { positionSpring = 0f, positionDamper = 0f, maximumForce = Mathf.Infinity };
        _grabJoint.zDrive = new JointDrive { positionSpring = 0f, positionDamper = 0f, maximumForce = Mathf.Infinity };

        // Настройка привязки
        _grabJoint.anchor = Vector3.zero;
        _grabJoint.autoConfigureConnectedAnchor = false;
        _grabJoint.connectedAnchor = Vector3.zero;

        // Разрешить столкновения предмета с точкой захвата (не обязательно, но полезно)
        _grabJoint.enableCollision = true;

        _pickedItem = _currentSelectable;
        _isItemPicked = true;

        // Снимаем выделение после поднятия
        if (_currentSelectable != null)
        {
            _currentSelectable.Deselect();
            _currentSelectable = null;
        }

        Debug.Log("Предмет поднят (свобода движения, вращение заблокировано)");
    }

    private void DropItemPhysical()
    {
        if (_pickedItem == null) return;

        // Удаляем joint
        if (_grabJoint != null)
            Destroy(_grabJoint);

        // Восстанавливаем физику (на всякий случай)
        Rigidbody rb = _pickedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        Collider col = _pickedItem.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        _pickedItem = null;
        _isItemPicked = false;
        _grabJoint = null;

        Debug.Log("Предмет отпущен");
    }
}