using UnityEngine;

public class BreakableCube : MonoBehaviour
{
    [Header("Настройки разрушения")]
    public GameObject _destroyedPrefab;
    public float _explosionForce = 10f; 
    public float _explosionRadius = 5f; 
    public float _massThreshold = 2f; 

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (otherRigidbody == null) return;

        if (otherRigidbody.mass >= _massThreshold)
        {
            BreakCube(collision);
        }
    }

    void BreakCube(Collision collision)
    {
        GameObject brokenCube = Instantiate(_destroyedPrefab, transform.position,transform.localRotation);

        gameObject.SetActive(false);

        foreach (Transform piece in brokenCube.transform)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(_explosionForce, collision.contacts[0].point, _explosionRadius);
            }
        }

        Destroy(gameObject, 2f);
    }
}