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
    public SlotContent slotContent, newContent;
    public Image icon;
    public TextMeshProUGUI amount;
    public Image slotSelected;
    public bool canSelect = false;
    private DisplaySlots displaySlots;

    public GameObject consumeButton;
    public GameObject splitButton;
    public GameObject readButton;
    public GameObject middleSpace;

    [Header ("RCM Buttons")]
    private Button dropItemButton;
    private Button consumeItemButton;
    private Button splitItemButton;

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
        /*

        if (slotUI.slotContent.item.itemStackable)
        {

            //DropStackable();

        } else if (slotUI.slotContent.item.itemStackable == false)
        {
            //DropNonStackable();
        } */
    }

    /*   ----------------------------------- CONSUME ---------------------------------------
    public void DropItemStackable()
    {
        if (!invHandler.rightClickMenu.activeSelf)
        {
            invHandler.rightClickMenu.SetActive(true);
        }
        removeItemButton = FindObjectOfType<ButtonReference>().buttonRef;
        removeItemButton.onClick.AddListener(ReduceAmount);


    }

    public void ReduceAmount()
    {
        slotContent.amount--;

        if (slotContent.amount == 0)
        {
            slotContent.Clear();
            DeselectAllItems();
        }
    }

    public void DropItemNonStackable()
    {
        if (!invHandler.rightClickMenu.activeSelf)
        {
            invHandler.rightClickMenu.SetActive(true);
        }
        removeItemButton = FindObjectOfType<ButtonReference>().buttonRef;
        removeItemButton.onClick.AddListener(RemoveItem);
    }

    public void RemoveItem()
    {
        slotContent.Clear();
        DeselectAllItems();

    } */

    public void SelectSlot()
    {
        DeselectAllItems();
        StartCoroutine("SlotSelected");
    }

    IEnumerator SlotSelected()
    {
        invHandler.isFollowing = true;
        yield return new WaitForSeconds(.001f);
        invHandler.rightClickMenu.SetActive(true);
        invHandler.isFollowing = false;

        dropItemButton = FindObjectOfType<ButtonReference>().buttonRef;
        dropItemButton.onClick.RemoveAllListeners();


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
            middleSpace = GameObject.Find("Canvas/rightClickMenu/Decorations/middle");
            middleSpace.SetActive(false);

        } else
        {
            consumeButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ConsumeButton");
            consumeButton.SetActive(true);
            splitButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/SplitButton");
            splitButton.SetActive(true);
            readButton = GameObject.Find("Canvas/rightClickMenu/ButtonsContainer/ReadButton");
            readButton.SetActive(false);
            middleSpace = GameObject.Find("Canvas/rightClickMenu/Decorations/middle");
            middleSpace.SetActive(true);

            consumeItemButton = FindObjectOfType<ConsumeButton>().consumeRef;
            consumeItemButton.onClick.RemoveAllListeners();
        }

        dropItemButton = FindObjectOfType<ButtonReference>().buttonRef;
        dropItemButton.onClick.AddListener(DropItem);

        if (slotContent.item.itemStackable)
        {
            consumeItemButton = FindObjectOfType<ConsumeButton>().consumeRef;
            consumeItemButton.onClick.AddListener(ConsumeItem);

           // splitItemButton = FindObjectOfType<SplitButton>().splitRef;
            //splitItemButton.onClick.AddListener(SplitItem);
        } 
    }

    public void SplitItem()
    {
        /*
        if (slotContent.amount < 1)
        {
            Debug.Log("non e possibile splittare");
        } */

        foreach (SlotUI item in displaySlots.UISlots)
        {
            if (item.slotContent.amount == 0)
            {
                Debug.Log("c'e una slot libera");
                int halfStack = Mathf.RoundToInt(slotContent.amount / 2);

                slotContent.RemoveFromStack(halfStack);

                
                //SlotContent splitStack = new SlotContent(slotContent.item.itemName, slotContent.newAmount);
                //Debug.Log(nSlot.item.itemName + slotContent.newAmount);

        
               // displaySlots.items.Add(new SlotContent(splitStack.item.itemName, slotContent.newAmount));

    

            }
            else 
            {
                invHandler.readItemInfo.SetActive(true);
                invHandler.itemDesc.text = "You don't have any room to do that!";
                invHandler.isFollowing = false;
            }
        }
        /*
        for (int i = 0; i < displaySlots.UISlots.Count; i++)
        {
            if (slotContent.amount == 0)
            {
                Debug.Log("c'e una slot libera");
            } else
            {
                Debug.Log("tutte le slot sono occupate");
            }

        } */

        //newContent = new SlotContent(slotContent.item.itemName, halfStack);

        
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
