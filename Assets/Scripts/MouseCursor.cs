using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
   // private InventoryItem item;
    private Canvas canvas;

    private void Awake()
    {
        // item = GetComponentInChildren<InventoryItem>();
        canvas = transform.root.GetComponent<Canvas>();
    }

    public void SetData(Sprite itemIcon, int amount)
    {
        //item.SetData(itemIcon, amount);
    }

    private void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void ToggleCursor(bool theValue)
    {
        Debug.Log($"Item togled {theValue}");
        gameObject.SetActive(theValue);
    }
}
