using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BackPackManager : MonoBehaviour
{
    public Transform backPack;
    public List<BackPackSlot> slots = new List<BackPackSlot>();
    void Start()
    {
        for (int i = 0; i < backPack.childCount; i++)
        {
            if(backPack.GetChild(i).GetComponent<BackPackSlot>() != null)
            {
                slots.Add(backPack.GetChild(i).GetComponent<BackPackSlot>());
            }
        }
    }


    void Update()
    {
        
    }
}
