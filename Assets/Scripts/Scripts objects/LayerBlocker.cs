using UnityEngine;

public class LayerBlocker : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private LayerMask _allowedLayers;
    [SerializeField] private float _pushForce = 1000f;

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        if (((1 << other.gameObject.layer) & _allowedLayers) != 0)
        {
            return; 
        }

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Vector3 direction = (rb.position - transform.position).normalized;
            rb.AddForce(direction * _pushForce, ForceMode.Force);
        }
        else
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                Vector3 pushBack = (other.transform.position - transform.position).normalized;
                cc.Move(pushBack * Time.deltaTime * _pushForce);
            }
            else
            {
                other.transform.position = Vector3.MoveTowards(
                    other.transform.position,
                    transform.position - (other.transform.position - transform.position).normalized * 0.5f,
                    Time.deltaTime * _pushForce
                );
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (((1 << other.gameObject.layer) & _allowedLayers) != 0)
        {
            Debug.Log($"Объект {other.name} (слой {LayerMask.LayerToName(other.gameObject.layer)}) прошёл через барьер");
            return;
        }

        Debug.Log($"Объект {other.name} (слой {LayerMask.LayerToName(other.gameObject.layer)}) заблокирован");

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Vector3 direction = (rb.position - transform.position).normalized;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * _pushForce * 2f, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}