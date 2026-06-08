using UnityEngine;
using UnityEngine.Events;

public class Lamp : MonoBehaviour
{
    [SerializeField] private Material _materialOn;
    [SerializeField] private Material _materialOff;
    [SerializeField] private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void OnMaterial()
    {
        _renderer.material = _materialOn;
    }

    public void OffMaterial()
    {
        _renderer.material = _materialOff;
    }
}