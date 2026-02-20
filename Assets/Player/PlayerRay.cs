using Unity.Cinemachine;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    private string toolKey = "size";
    [SerializeField] private CinemachineCamera _cinCam;
    public Selectable CurrentSelectable;
    private bool pickupitem = false;
    private Selectable pickItem;
    private float positonItemOnCamera;
    void LateUpdate()
    {
        LookOnSelectebleObject();
    }
    private void LookOnSelectebleObject()
    {
        Vector3 rayDirection = _cinCam.transform.forward;
        Ray ray = new Ray(_cinCam.transform.position, rayDirection);

        Debug.DrawRay(_cinCam.transform.position, rayDirection * 5f, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,5f))
        {
            Selectable selectable = hit.collider.gameObject.GetComponent<Selectable>();
            if (selectable)
            {
                if (CurrentSelectable && CurrentSelectable != selectable)
                {
                    CurrentSelectable.Deselect();
                    CurrentSelectable = null;
                }
                CurrentSelectable = selectable;
                selectable.Select();
            }
            else
            {
                if (CurrentSelectable)
                {
                    CurrentSelectable.Deselect();
                    CurrentSelectable = null;
                }
            }
        }
        else
        {
            if (CurrentSelectable)
            {
                CurrentSelectable.Deselect();
                CurrentSelectable = null;
            }
        }
    }
    public void OnButton1()
    {
        toolKey = "size";
        Debug.Log("Выбран инструмен меняющий размер");
    }
    public void OnButton2()
    {
        toolKey = "gravity";
        Debug.Log("Выбран инструмен меняющий гравитацию");
    }
    public void OnInteract()
    {
        if (toolKey == "gravity" && CurrentSelectable != null)
        {
            ChangedGravity gravity = CurrentSelectable.GetComponent<ChangedGravity>();
            if (gravity != null)
            {
                gravity.ChangeGravity();
            }
        }
    }

    public void OnMouseBack()
    {
        if (toolKey == "size" && CurrentSelectable != null)
        {
            Resising resize = CurrentSelectable.GetComponent<Resising>();
            if (resize != null)
            {
                resize.ReducingSize();
            }
        }
    }

    public void OnMouseForward()
    {
        if (toolKey == "size" && CurrentSelectable != null)
        {
            Resising resize = CurrentSelectable.GetComponent<Resising>();
            if (resize != null)
            {
                resize.IncreasingSize();
            }
        }
    }
    public void OnPickupButton()
    {
            if (pickupitem == false && CurrentSelectable != null)
            {
                CurrentSelectable.transform.SetParent(_cinCam.transform);
                positonItemOnCamera = CurrentSelectable.transform.localScale.x + 2f;
                CurrentSelectable.transform.localPosition = new Vector3(0, 0, positonItemOnCamera);
                CurrentSelectable.transform.localRotation = Quaternion.identity;
                CurrentSelectable.rb.isKinematic = true;
                CurrentSelectable.GetComponent<Collider>().enabled = false;
                pickItem = CurrentSelectable;
                pickupitem = true;
                Debug.Log("Предмет поднят!");
            }
            else if (pickupitem == true)
            {
                pickItem.transform.SetParent(null);
                pickItem.rb.isKinematic = false;
                pickItem.GetComponent<Collider>().enabled = true;
                pickupitem = false;
                Debug.Log("Предмет отпущен!");
            }
    }
}