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
            Vector3.down * gravityForce:
            Vector3.up * gravityForce;

        rb.AddForce(gravity, ForceMode.Impulse);
    }
    public void ChangeGravity()
    {
        isGravityDown = !isGravityDown;
    }
}
