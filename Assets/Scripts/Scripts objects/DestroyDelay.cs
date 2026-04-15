using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 5f;

    private void Start()
    {
        Destroy(gameObject, _destroyTime);
    }
}