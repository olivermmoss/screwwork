using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tooltipFollowMouse : MonoBehaviour
{
    public Canvas can; 

    // Update is called once per frame
    void Update()
    {
        float extremeX = can.transform.GetComponent<RectTransform>().sizeDelta.x / 2f - transform.localScale.x / 2f;
        float extremeY = can.transform.GetComponent<RectTransform>().sizeDelta.y / 2f;
        Vector3 scaledMousePos = (Input.mousePosition - can.transform.position) / can.scaleFactor;

        Vector3 desiredPos = new Vector3(Mathf.Clamp(scaledMousePos.x, -extremeX, extremeX), Mathf.Clamp(scaledMousePos.y + 10, -extremeY, extremeY - transform.localScale.y));
        transform.localPosition = desiredPos;
    }
}
