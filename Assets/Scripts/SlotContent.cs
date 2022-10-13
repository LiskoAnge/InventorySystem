using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotContent
{
    private SlotUI uiItemSlot;
    public bool isFull { get { return (item != null); } }
    public Item item;
    private int _amount;
    public int amount
    {
        get { return _amount; }
        set
        {
            if (item == null)
            {
                _amount = 0;
            }
            else if (value > item.maxStack)
            {
                _amount = item.maxStack;
            }
            else if (value < 1)
            {
                _amount = 0;
            }
            else {
                _amount = value;
            }
            RefreshSlotUI();
        }
    }



    private Item FindByName(string itemName)
    {
        itemName = itemName.ToLower(); 
        Item _item = Resources.Load<Item>(string.Format("Items/{0}", itemName)); 
        if (_item == null)
        {
            Debug.LogWarning(string.Format("The item couldn't be found! \"{0}\". The item is empty.", itemName));
        }
        return _item;
    }

    public static bool CheckSlot(SlotContent slot1, SlotContent slot2)
    {
        if (slot1.item != slot2.item)
        {
            return false;
        }
        return true;
    }

    public static void SwapSlots(SlotContent slot1, SlotContent slot2)
    {
        Item _item = slot1.item;
        int _amount = slot1.amount;
        slot1.item = slot2.item;
        slot1.amount = slot2.amount;
        slot2.item = _item;
        slot2.amount = _amount;
        slot1.RefreshSlotUI();
        slot2.RefreshSlotUI();
    }

    public void ConnectUI(SlotUI uiSlot)
    {
        uiItemSlot = uiSlot;
        uiItemSlot.slotContent = this;
        RefreshSlotUI();
    }

    public void DetachUI()
    {
        uiItemSlot.ClearSlot();
        uiItemSlot = null;
    }

    private bool isConnectedToUI { get { return (uiItemSlot != null); } }

    public void RefreshSlotUI()
    {
        if (!isConnectedToUI)
        {
            return;
        }
        uiItemSlot.RefreshTheSlot();
    }

    public void Clear()
    {
        item = null;
        amount = 0;
        RefreshSlotUI();
    }

    public SlotContent(string itemName, int _amount = 1)
    {
        Item _item = FindByName(itemName);

        if (_item == null)
        { 
            item = null;
            amount = 0;
            return;
        }
        else
        { 
            item = _item;
            amount = _amount;
        }
    }
    public SlotContent()
    {
        item = null;
        amount = 0;
    }
}