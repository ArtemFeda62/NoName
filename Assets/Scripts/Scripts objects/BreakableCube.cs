using UnityEngine;

public class BreakableCube : MonoBehaviour
{
    [SerializeField] private GameObject _destroyedCubePrefab;
    private bool isDestroy = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDestroy)
        {
            isDestroy = true;
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = transform.rotation;
            GameObject destroyedCube = Instantiate(_destroyedCubePrefab, spawnPosition, spawnRotation);
            Rigidbody[] pieces = destroyedCube.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody piece in pieces)
            {
                piece.isKinematic = false;
                piece.AddForce(Random.onUnitSphere * 5f, ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
    }
}