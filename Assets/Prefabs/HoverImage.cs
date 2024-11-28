using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOver = false;
    private Image image;
    public Color hover;

    void Start()
    {
        // Get the Image component attached to this GameObject
        image = GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError("No Image component found on this GameObject. Please attach an Image component.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse enter");
        isOver = true;
        if (image != null)
        {
            image.color = hover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exit");
        isOver = false;
        if (image != null)
        {
            image.color = Color.black;
        }
    }
}
