using UnityEngine;

[CreateAssetMenu(fileName="New Item", menuName ="New inventory Item")]
public class Item : ScriptableObject {
    public string itemName;
    [TextArea]
    public string itemInfo;
    public string itemAction;
    public Sprite itemIcon;
    public string buttonText;
    public int itemID;
    public int maxStack;
    public bool itemStackable { get { return (maxStack > 1); } }
}
