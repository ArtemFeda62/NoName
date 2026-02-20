using UnityEngine;

public class Resising : MonoBehaviour
{
    private Vector3 objectSize;
    private Vector3 nowSize;
    public void Start()
    {
        objectSize = transform.localScale;
        nowSize = transform.localScale;
    }
    public void ReducingSize()
    {
        if (nowSize.x > objectSize.x)
        {
            nowSize = new Vector3(nowSize.x - 0.1f, nowSize.y - 0.1f, nowSize.z - 0.1f);
        }
        else
        {
            Debug.Log("Нельзя сделать меньше размер");
        }
    }
    public void FixedUpdate()
    {
        transform.localScale = nowSize;
    }
    public void IncreasingSize()
    {
        if (nowSize.x < 10)
        {
            nowSize = new Vector3(nowSize.x + 0.1f, nowSize.y + 0.1f, nowSize.z + 0.1f);
        }
        else
        {
            Debug.Log("Нельзя сделать  больше размер");
        }
    }
}
