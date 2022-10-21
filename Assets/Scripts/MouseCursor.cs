using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    private Canvas canvas;

    private void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
    }

    private void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void ToggleCursor(bool theValue)
    {
        //Debug.Log($"Item togled {theValue}");
        gameObject.SetActive(theValue);
    }
}
