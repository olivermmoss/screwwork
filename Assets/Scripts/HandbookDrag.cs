using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandbookDrag : Draggable
{
    protected override void OnSnapped(Transform point)
    {
        sr.sortingLayerName = "Background";
        sr.sortingOrder = -15;
    }

    protected override void OnGrabbed()
    {
        sr.sortingLayerName = "Default";
    }

    public override void SendBack()
    {
        if (!snapped)
            sr.sortingOrder -= 8;
    }
}
