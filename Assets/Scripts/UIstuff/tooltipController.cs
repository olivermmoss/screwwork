using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class tooltipController : MonoBehaviour
{
    public TextMeshProUGUI tooltip;
    public RectTransform tooltipBg;
    public string text;
    bool mousedOverThisFrame = false;

    protected void OnMouseEnter()
    {
        tooltip.text = text;
        mousedOverThisFrame = true;
        tooltip.gameObject.SetActive(true);
        tooltipBg.gameObject.SetActive(true);
        ExtraOnMouseEnter();
    }

    protected void OnMouseExit()
    {
        tooltip.gameObject.SetActive(false);
        tooltipBg.gameObject.SetActive(false);
        ExtraOnMouseExit();
    }

    private void OnGUI()
    {
        //even with this jank setup, it still jitters sometimes :(
        if (mousedOverThisFrame)
        {
            //print("set position");
            tooltipBg.GetComponent<RectTransform>().localScale = tooltip.textBounds.size + 20 * Vector3.one;
            //tooltip.textBounds.center = tooltipBg.position;
            mousedOverThisFrame = false;
        }
    }

    private void Start()
    {
        text.Replace("<br>", "\n");
    }

    protected virtual void ExtraOnMouseEnter()
    {}

    protected virtual void ExtraOnMouseExit()
    {}
}
