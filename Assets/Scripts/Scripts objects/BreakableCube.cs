using UnityEngine;
using System.Collections;

public class BreakableCube : MonoBehaviour
{
    [SerializeField] private GameObject _destroyedCubePrefab;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _breakSound;
    [SerializeField] private float _breakForce = 25f;
    [SerializeField] private float _minVelocityToBreak = 3f;
    [SerializeField] private float _destroyDelay = 0.5f;
    [SerializeField] private Object _visualWall;

    private bool isDestroy = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroy) return;

        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity >= _minVelocityToBreak)
        {
            Destroy(_visualWall);
            BreakCube();
        }
    }

    private void BreakCube()
    {
        if (isDestroy) return;
        isDestroy = true;

        if (_breakSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_breakSound, 1f);
            Debug.Log("Звук разрушения воспроизведён");
        }
        else
        {
            Debug.LogWarning("Звук или AudioSource не назначены!");
        }

        GameObject destroyedCube = Instantiate(_destroyedCubePrefab, transform.position, transform.rotation);

        Rigidbody[] pieces = destroyedCube.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody piece in pieces)
        {
            piece.isKinematic = false;
            piece.AddForce(Random.onUnitSphere * _breakForce, ForceMode.Impulse);
        }

        Destroy(gameObject, _destroyDelay);
    }
}