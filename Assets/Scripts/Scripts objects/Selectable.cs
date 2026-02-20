using Unity.Cinemachine;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Select()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }
    public void Deselect()
    {
        GetComponent<Renderer>().material.color = Color.gray;
    } 
}
