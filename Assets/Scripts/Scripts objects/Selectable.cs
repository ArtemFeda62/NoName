using UnityEngine;

public class Selectable : MonoBehaviour
{
    public Rigidbody rb;
    public float Mass;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        Mass = rb.mass;
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