using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIupdown : MonoBehaviour, IPointerClickHandler
{
    public Sprite upSprite;
    public Sprite downSprite;
    Button butt;
    bool up = true;
    public RectTransform child;

    private void Start()
    {
        butt = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        child.localPosition = new Vector3(0, 5);
    }

    public void OnMouseUp()
    {
        child.localPosition = new Vector3(0, 0);
    }

    public void OnMouseExit()
    {
        child.localPosition = new Vector3(0, 0);
    }
}
