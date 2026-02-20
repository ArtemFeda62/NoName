using UnityEngine;

public class ChangedGravity : MonoBehaviour
{
    private bool isGravityDown = true;
    public float gravityForce = 9.81f;
    public Rigidbody rb;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    public void FixedUpdate()
    {
        Vector3 gravity = isGravityDown ?
            Vector3.down * gravityForce * rb.mass :
            Vector3.up * gravityForce * rb.mass;

        rb.AddForce(gravity, ForceMode.Force);
    }
    public void ChangeGravity()
    {
        isGravityDown = !isGravityDown;
    }
}
