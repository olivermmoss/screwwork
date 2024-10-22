using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class tooltipText : MonoBehaviour
{
    public RectTransform bg;
    RectTransform rect;
    TextMeshProUGUI tmp;
    public float offset;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rect.localPosition = bg.localPosition + new Vector3(-bg.localScale.x/2f + 10, 16);
    }
}
