using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InvHandler : MonoBehaviour
{
    public GameObject rightClickMenu;
    public GameObject readItemInfo;
    public TextMeshProUGUI itemDesc;
    public SlotUI theCursor;
    public bool isFollowing;
    public Vector3 offset;
    public Vector3 infoOffset;
    private SlotUI slotUI;
    public bool eliminateItem;

    private void Awake()
    {
        slotUI = gameObject.GetComponent<SlotUI>();
        isFollowing = true;
        readItemInfo.SetActive(false);
        rightClickMenu.SetActive(false);

    }


    private void Update()
    {
        if (isFollowing == true)
        {
            rightClickMenu.transform.position = Input.mousePosition + offset;
            readItemInfo.transform.position = Input.mousePosition + infoOffset;
        }
        else
        {
            //Debug.Log("menu not following mouse cursor");
        }
    }

    
    public void ReadItemInfo()
    {
        rightClickMenu.SetActive(false);
        readItemInfo.SetActive(true);
    }

    public void CloseInfo()
    {
        theCursor.DeselectAllItems();
        rightClickMenu.SetActive(false);
        readItemInfo.SetActive(false);
    }

    
    public void DropItem()
    {
        theCursor.DeselectAllItems();
        rightClickMenu.SetActive(false);
        readItemInfo.SetActive(false);
    } 


}






