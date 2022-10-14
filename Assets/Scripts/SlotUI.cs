using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class SlotUI : MonoBehaviour,IPointerClickHandler, IPointerUpHandler
{
    public bool isSlotSelected;

    private InvHandler invHandler;
    public bool isMouseCursor = false;
    public SlotContent slotContent;
    public Image icon;
    public TextMeshProUGUI amount;
    public Image slotSelected;
    public bool canSelect = false;
    private DisplaySlots displaySlots;

    public Button removeItemButton;

    public GameObject consumeButton;
    public GameObject splitButton;
    public GameObject readButton;

    private void Awake()
    {
        displaySlots = FindObjectOfType<DisplaySlots>();
        invHandler = FindObjectOfType<InvHandler>();
        slotContent = new SlotContent();
    }
    private void Update()
    {
        invHandler.theCursor.transform.position = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        invHandler.theCursor.transform.position = eventData.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            RightMouseClick(gameObject.GetComponent<SlotUI>());

        }
        else
        {
            invHandler.theCursor.transform.position = eventData.position;
            LeftMouseClick(gameObject.GetComponent<SlotUI>());
        }
    }

    private void LeftMouseClick(SlotUI click)
    {     
        if (click == null)
        {
            Debug.Log("No SlotUI component in UI element tagged as SlotUI");
            return;
        }

        if (!SlotContent.CheckSlot(invHandler.theCursor.slotContent, click.slotContent))
        {
            SlotContent.SwapSlots(invHandler.theCursor.slotContent, click.slotContent);
            invHandler.theCursor.RefreshTheSlot();
            return;
        }
        if (SlotContent.CheckSlot(invHandler.theCursor.slotContent, click.slotContent))
        {
            if (invHandler.theCursor.slotContent.item == null)
            {
                return;
            }
            if (!invHandler.theCursor.slotContent.item.itemStackable)
            {
                return;
            }

            if (click.slotContent.amount == click.slotContent.item.maxStack)
            {
                return;
            }

            int total = invHandler.theCursor.slotContent.amount + click.slotContent.amount;
            int maxStack = invHandler.theCursor.slotContent.item.maxStack;

            if (total <= maxStack)
            {
                click.slotContent.amount = total;
                invHandler.theCursor.slotContent.Clear();
            }
            else
            {
                click.slotContent.amount = maxStack;
                invHandler.theCursor.slotContent.amount = total - maxStack;
            }
            invHandler.theCursor.RefreshTheSlot();

           
        }
    }


   private void RightMouseClick(SlotUI slotUI)
   {
        isSlotSelected = !isSlotSelected;

        if (isSlotSelected && slotContent.amount != 0)    //do not select slot if already selected  empty
        {
            SelectSlot();
        } 
        else
        {
            DeselectAllItems();
        }

        if (slotUI.slotContent.item.itemStackable)
        {
            RemoveItemStackable();

        } else if (slotUI.slotContent.item.itemStackable == false)
        {
            RemoveItemNonStackable();
        } else
        {
            return;
        }
    }


    public void RemoveItemStackable()
    {
        if (!invHandler.rightClickMenu.activeSelf)
        {
            invHandler.rightClickMenu.SetActive(true);
        }
        removeItemButton = FindObjectOfType<ButtonReference>().removeButtonReference;
        removeItemButton.onClick.AddListener(ReduceAmount);


    }

    public void ReduceAmount()
    {
        slotContent.amount--;

        if (slotContent.amount == 0)
        {
            slotContent.Clear();
            DeselectAllItems();
            //removeItemButton.onClick.RemoveAllListeners();
        }
    }

    public void RemoveItemNonStackable()
    {
        if (!invHandler.rightClickMenu.activeSelf)
        {
            invHandler.rightClickMenu.SetActive(true);
        }
        removeItemButton = FindObjectOfType<ButtonReference>().removeButtonReference;
        removeItemButton.onClick.AddListener(RemoveItem);
    }

    public void RemoveItem()
    {
        slotContent.Clear();
        DeselectAllItems();
    }

    public void SelectSlot()
    {
        DeselectAllItems();

        StartCoroutine("SlotSelected");
    }

    IEnumerator SlotSelected()
    {
        invHandler.isFollowing = true;
        yield return new WaitForSeconds(.1f);
        invHandler.rightClickMenu.SetActive(true);
        invHandler.isFollowing = false;
        this.slotSelected.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
        invHandler.itemDesc.text = this.slotContent.item.itemInfo;

        if (!slotContent.item.itemStackable)
        {
            consumeButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ConsumeButton");
            consumeButton.SetActive(false);
            splitButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/SplitButton");
            splitButton.SetActive(false);
            readButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ReadButton");
            readButton.SetActive(true);
        } else
        {
            consumeButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ConsumeButton");
            consumeButton.SetActive(true);
            splitButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/SplitButton");
            splitButton.SetActive(true);
            readButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ReadButton");
            readButton.SetActive(false);
        }
      
    }
  
    public void DeselectAllItems()
    {
        foreach (SlotUI item in displaySlots.UISlots)
        {
            item.DeselectSlot();
        }
    }

    public void DeselectSlot()
    {
        invHandler.isFollowing = true;
        invHandler.rightClickMenu.SetActive(false);
        invHandler.readItemInfo.SetActive(false);
        this.slotSelected.GetComponent<Image>().color = new Color32(255, 255, 225, 0);
    }


    public void RefreshTheSlot()
    {
        UpdatingAmount();
        UpdatingIcon();
    }

    public void ClearSlot()
    {
        slotContent = null;
        RefreshTheSlot();
    }


    public void UpdatingIcon()
    {
        if (slotContent == null || !slotContent.isFull)
        {
            icon.enabled = false;
        }
        else
        {
            icon.enabled = true;
            icon.sprite = slotContent.item.itemIcon;
        }
    }

    public void UpdatingAmount()
    {
        if (slotContent == null || !slotContent.isFull || slotContent.amount < 2)
        {
            amount.enabled = false;
        }
        else
        {
            amount.enabled = true;
            amount.text = slotContent.amount.ToString();
        }
    }
}
