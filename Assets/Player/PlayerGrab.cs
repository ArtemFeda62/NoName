using UnityEngine;

public class REPOStyleGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _grabPoint; // Точка перед камерой/рукой

    private ConfigurableJoint _grabJoint;
    private Rigidbody _grabbedRigidbody;
    private bool _isGrabbing;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isGrabbing)
        {
            TryGrab();
        }
        else if (Input.GetMouseButtonUp(0) && _isGrabbing)
        {
            Release();
        }
    }

    void TryGrab()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                _grabbedRigidbody = rb;
                _grabbedRigidbody.useGravity = true; // Гравитация продолжает действовать!
                _grabJoint = _grabbedRigidbody.gameObject.AddComponent<ConfigurableJoint>();
                _grabJoint.connectedBody = GetComponent<Rigidbody>(); // Прикрепляем к игроку

                // Настройка Joint
                _grabJoint.xMotion = ConfigurableJointMotion.Limited;
                _grabJoint.yMotion = ConfigurableJointMotion.Limited;
                _grabJoint.zMotion = ConfigurableJointMotion.Limited;
                _grabJoint.angularXMotion = ConfigurableJointMotion.Limited;
                _grabJoint.angularYMotion = ConfigurableJointMotion.Limited;
                _grabJoint.angularZMotion = ConfigurableJointMotion.Limited;

                _grabJoint.anchor = _grabPoint.localPosition;
                _grabJoint.autoConfigureConnectedAnchor = false;
                _grabJoint.connectedAnchor = Vector3.zero;

                // Параметры пружины (делающие движение упругим)
                _grabJoint.linearLimit = new SoftJointLimit() { limit = 0.1f };
                _grabJoint.linearLimitSpring = new SoftJointLimitSpring() { spring = 1000, damper = 100 };

                _isGrabbing = true;
            }
        }
    }

    void Release()
    {
        if (_grabJoint != null) Destroy(_grabJoint);
        _grabbedRigidbody = null;
        _isGrabbing = false;
    }
}