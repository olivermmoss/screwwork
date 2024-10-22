using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{
    float startOffset;
    float startAngle;
    public Sprite[] sprites;
    protected bool dragged;
    public float angle = 0;
    public SpriteRenderer sr;
    public Collider2D col;
    public bool grabbable = true;
    public int spriteId;
    public int CWfullRots = 0;
    float lastAngle = 0;
    const float HALFPI = 0.5f * Mathf.PI;
    const float THREEHALVESPI = 1.5f * Mathf.PI;
    const float TWOPI = 2 * Mathf.PI;
    public Transform lidSnap;
    SpriteRenderer maxHeight;
    AudioSource source;
    float lastMove = 0;
    public float stopAllowance;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if (lidSnap.childCount > 0)
            col.enabled = false;
        else
            col.enabled = true;

        if (Input.GetKeyDown(KeyCode.Mouse0) && grabbable)
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col.OverlapPoint(mousePoint))
            {
                maxHeight = sr;
                foreach (SpriteRenderer rend in GameObject.FindObjectsOfType<SpriteRenderer>())
                {
                    if (rend.gameObject.GetComponent<Draggable>() != null || rend.gameObject.GetComponent<Rotatable>() != null)
                    {
                        if (rend.sortingOrder > maxHeight.sortingOrder && rend.gameObject.GetComponent<Collider2D>().OverlapPoint(mousePoint))
                            maxHeight = rend;
                    }
                }
                if (sr == maxHeight)
                {
                    Vector3 relativeGrabPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    startOffset = Mathf.Atan2(relativeGrabPoint.x, relativeGrabPoint.y);
                    dragged = true;
                    startAngle = angle;
                }
            }
        }

        //angle = PutInRange(angle + Time.deltaTime);
        spriteId = Mathf.RoundToInt(sprites.Length * angle / TWOPI) % sprites.Length;
        //sr.sprite = sprites[spriteId];

        if (dragged && Input.GetKeyUp(KeyCode.Mouse0))
        {
            dragged = false;
        }

        if (dragged)
        {
            Vector3 relativeGrabPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float curAngle = Mathf.Atan2(relativeGrabPoint.x, relativeGrabPoint.y);
            angle = PutInRange(startAngle + curAngle - startOffset);

            if (angle > THREEHALVESPI && lastAngle < HALFPI)
            {
                CWfullRots--;
            }
            else if (lastAngle > THREEHALVESPI && angle < HALFPI)
            {
                CWfullRots++;
            }

            if (lastAngle != angle)
            {
                lastMove = Time.time;

                if (!source.isPlaying)
                    source.Play();
            }

            lastAngle = angle;

            spriteId = Mathf.RoundToInt(sprites.Length * angle / TWOPI) % sprites.Length;
            sr.sprite = sprites[spriteId];
            
        }

        if (source.isPlaying && lastMove + stopAllowance < Time.time)
        {
            source.Stop();
        }
    }

    float PutInRange(float theta)
    {
        return (theta >= 0) ? theta % TWOPI : (theta % TWOPI + TWOPI);
    }

    public float CWSpinFromStart()
    {
        return CWfullRots + angle/TWOPI;
    }
}
