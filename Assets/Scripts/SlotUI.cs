using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class SlotUI : MonoBehaviour,IPointerClickHandler, IPointerUpHandler
{
    private InvHandler invHandler;
    private GameObject consumeButton;
    private GameObject splitButton;
    private GameObject actionButton;
    private GameObject middleSpace;

    public bool isMouseCursor = false;
    public bool isSlotSelected;

    public SlotContent slotContent;
    private DisplaySlots displaySlots;
    public Image slotSelected;
    public Image icon;
    public TextMeshProUGUI amount;

    private Button dropItemButton;
    private Button consumeItemButton;
    private Button splitItemButton;
    private Button inspectItemButton;

    private void Awake()
    {
        icon.SetNativeSize();
        icon.preserveAspect = true;
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
        DeselectAllItems();

        if (click == null)
        {
            //Debug.Log("No SlotUI component in UI element tagged as SlotUI");
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

        if (isSlotSelected && slotContent.amount != 0)
        {
            SelectSlot();
        } 
        else
        {
            DeselectAllItems();
        }
    }

    public void SelectSlot()
    {
        DeselectAllItems();
        StartCoroutine("SlotSelected");
    }

    IEnumerator SlotSelected()
    {
        invHandler.isFollowing = true;
        yield return new WaitForEndOfFrame();
        invHandler.rcm.SetActive(true);
        invHandler.isFollowing = false;
        dropItemButton = FindObjectOfType<DropButton>().dropRef;
        dropItemButton.onClick.RemoveAllListeners();

        inspectItemButton = FindObjectOfType<InspectButton>().inspectRef;
        inspectItemButton.onClick.RemoveAllListeners();

        this.slotSelected.GetComponent<Image>().color = new Color32(255, 255, 225, 255);

        invHandler.itemDesc.text = slotContent.item.itemAction;

        if (!slotContent.item.itemStackable)
        {
            LoadStackButtons();
        
        } else
        {
            LoadNonStackButtons();
        }
        dropItemButton = FindObjectOfType<DropButton>().dropRef;
        dropItemButton.onClick.AddListener(DropItem);

        inspectItemButton = FindObjectOfType<InspectButton>().inspectRef;
        inspectItemButton.onClick.AddListener(InspectItem);

        if (slotContent.item.itemStackable)
        {
            consumeItemButton = FindObjectOfType<ConsumeButton>().consumeRef;
            consumeItemButton.onClick.AddListener(ConsumeItem);

            splitItemButton = FindObjectOfType<SplitButton>().splitRef;
            splitItemButton.onClick.AddListener(SplitItem);
        } 
    }

    public void LoadStackButtons()
    {
        consumeButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ConsumeButton");
        consumeButton.SetActive(false);
        splitButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/SplitButton");
        splitButton.SetActive(false);
        actionButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ReadButton");
        actionButton.SetActive(true);
        if (actionButton != null)
        {
            TextMeshProUGUI actionText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
            actionText.text = slotContent.item.buttonText;
        }
        middleSpace = GameObject.Find("Canvas/rightClickMenu/Decorations/middle");
        middleSpace.SetActive(false);
    }

    public void LoadNonStackButtons()
    {
        consumeButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ConsumeButton");
        consumeButton.SetActive(true);
        splitButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/SplitButton");
        splitButton.SetActive(true);
        actionButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ReadButton");
        actionButton.SetActive(false);
        middleSpace = GameObject.Find("Canvas/rightClickMenu/Decorations/middle");
        middleSpace.SetActive(true);

        consumeItemButton = FindObjectOfType<ConsumeButton>().consumeRef;
        consumeItemButton.onClick.RemoveAllListeners();
        splitItemButton = FindObjectOfType<SplitButton>().splitRef;
        splitItemButton.onClick.RemoveAllListeners();
    }

    public void InspectItem()
    {
        invHandler.itemDesc.text = slotContent.item.itemInfo;
    }

    public void SplitItem()
    {
        foreach (SlotUI i in displaySlots.UISlots)    
        {
            if (!i.slotContent.isFull && slotContent.amount > 1)   
            {
                invHandler.infoItem.SetActive(false);

                if (slotContent.amount % 2 == 0)
                {
                    int halfStack = slotContent.amount / 2;         
                    slotContent.RemoveEvenNumStack(halfStack);
                    SlotContent.SplitSlots(slotContent, i.slotContent);
                }
                else if (slotContent.amount % 2 == 1)
                {
                    int halfStack = slotContent.amount / 2;         
                    slotContent.RemoveOddNumStack(halfStack);
                    SlotContent.SplitSlots(slotContent, i.slotContent);
                    i.slotContent.amount -= 1;
                } 
                return;
            }
            else if (slotContent.amount <= 1)
            {
                //Debug.Log("you can not split this");
                invHandler.infoItem.SetActive(true);
                invHandler.itemDesc.text = "You cannot split this item!";
                invHandler.isFollowing = false;
            }
            else
            {
                //Debug.Log("all slots full");
                invHandler.infoItem.SetActive(true);
                invHandler.itemDesc.text = "You don't have any room to do that!";
                invHandler.isFollowing = false;
            }
        }
    }
         
    public void DropItem()
    {
        slotContent.Clear();
        DeselectAllItems();
    }

    public void ConsumeItem()
    {
        slotContent.amount--;

        if (slotContent.amount == 0)
        {
            slotContent.Clear();
            DeselectAllItems();
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
        invHandler.rcm.SetActive(false);
        invHandler.infoItem.SetActive(false);
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
        if (slotContent == null || !slotContent.isFull || slotContent.amount == 1)   
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
