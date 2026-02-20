using UnityEngine;
public enum ItemType {Default, Food, Weapon}
public class ItemScriptableObject : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public double weight;
    public string itemDescription;
}
