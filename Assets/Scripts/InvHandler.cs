using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InvHandler : MonoBehaviour
{
    [Header("Right Click Menu")]
    public GameObject rcm;
    public GameObject infoItem;
   public TextMeshProUGUI itemDesc;

    [Header("Mouse Cursor")]
    public SlotUI theCursor;
    public bool isFollowing;
    public Vector3 offset;
    public Vector3 infoOffset;

    private void Awake()
    {
        isFollowing = true;
        infoItem.SetActive(false);
        rcm.SetActive(false);
    }

    private void Update()
    {
        if (isFollowing == true)
        {
            rcm.transform.position = Input.mousePosition + offset;
            infoItem.transform.position = Input.mousePosition + infoOffset;
        }
    }

    public void ItemAction()
    {
        rcm.SetActive(false);
        infoItem.SetActive(true);
    }

    public void InspectItem()
    {
        rcm.SetActive(false);
        infoItem.SetActive(true);
    }

    public void CloseInfo()
    {
        theCursor.DeselectAllItems();
        rcm.SetActive(false);
        infoItem.SetActive(false);
    }

    public void DropItem()
    {
        theCursor.DeselectAllItems();
        rcm.SetActive(false);
        infoItem.SetActive(false);
    } 
}






