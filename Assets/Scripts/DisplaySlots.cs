using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySlots : MonoBehaviour
{
    GameObject Slot;
    public Transform inventoryPanel; 
    public List<SlotContent> items = new List<SlotContent>();
    public List<SlotUI> UISlots = new List<SlotUI>();

    private void Start()
    {
        Slot = Resources.Load<GameObject>("Prefabs/Slot");
        Item[] tempItems = new Item[5];
        tempItems[0] = Resources.Load<Item>("Items/herbs");
        tempItems[1] = Resources.Load<Item>("Items/book");
        tempItems[2] = Resources.Load<Item>("Items/flask");
        tempItems[3] = Resources.Load<Item>("Items/apple");
        tempItems[4] = Resources.Load<Item>("Items/sword");

        for (int i = 0; i < 40; i++)
        {
            int index = Random.Range(0, 5);
            int amount = Random.Range(1, tempItems[index].maxStack);
            items.Add(new SlotContent(tempItems[index].name, amount));
        }
        DisplayItems(items);
    }
  
    public void DisplayItems(List<SlotContent> slots)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject newSlot = Instantiate(Slot, inventoryPanel);
            newSlot.name = i.ToString();
            UISlots.Add(newSlot.GetComponent<SlotUI>());
            slots[i].ConnectUI(UISlots[i]);
        }
    }
}
