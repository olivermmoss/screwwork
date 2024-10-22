using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchCard : Draggable
{
    public Sprite punched;
    private float moveSpeed = 0;
    bool stamped = false;

    protected override void OnSnapped(Transform point)
    {
        sr.sortingLayerName = "Background";
        sr.sortingOrder = -15;
        if (!stamped && point.gameObject.name == "punchClockSnap")
        {
            grabbable = false;
            stamped = true;
            StartCoroutine("punchClockEats");
        }
        else if (stamped && point.gameObject.name == "cardHolderSnap")
        {
            if (source != null && dropSounds.Length > 0)
            {
                source.clip = dropSounds[Random.Range(0, dropSounds.Length)];
                source.Play();
            }
            FindObjectOfType<GameController>().ClockOut();
        }
    }

    protected override void OnGrabbed()
    {
        sr.sortingLayerName = "Default";
    }

    IEnumerator punchClockEats()
    {
        yield return new WaitForSeconds(0.25f);
        transform.parent.GetComponent<AudioSource>().Play();
        moveSpeed = 2f;
        yield return new WaitForSeconds(1f);
        moveSpeed = 0;
        sr.sprite = punched;
        yield return new WaitForSeconds(0.5f);
        moveSpeed = -2f;
        yield return new WaitForSeconds(1f);
        grabbable = true;
        moveSpeed = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if(moveSpeed != 0)
        {
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
        }
    }

    public override void SendBack()
    {
        if(!snapped)
            sr.sortingOrder -= 8;
    }
}
