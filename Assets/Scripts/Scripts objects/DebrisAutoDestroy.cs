using UnityEngine;

public class DebrisAutoDestroy : MonoBehaviour
{
    [SerializeField] private float _lifetime = 2f; 

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}