using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    protected Vector2 startOffset;
    public bool dragged;
    public bool snapped;
    public SpriteRenderer sr;
    public Collider2D col;
    public float snapDist;
    public Transform[] snapPoints;
    //if any of these have a fufilled snap, you cannot snap or unsnap this object.
    //This assumes that the other objects prevent every one of this object's snap points which isnt always true
    public Transform[] preventionSnaps;
    public bool grabbable = true;
    public bool hasSnapPoints;
    //this is a bad name lol.  it's like, when it's snapped, what's going on top
    public int snappingPriority = 0;
    //only works if camera is at origin
    public Vector2 clampedExtent;
    static SpriteRenderer maxHeight;
    static int lastGrabTime = 0;
    private Vector2 grabSpot;
    private Transform grabParent;
    public AudioClip[] pickupSounds;
    public AudioClip[] dropSounds;
    protected AudioSource source;
    private bool inAir = false;
    float timeInAir = 0f;
    public bool shelved;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        Camera cam = FindObjectOfType<Camera>();
        //breaks if screen is anything but 16:9
        clampedExtent = new Vector2(cam.orthographicSize * 4f / 3f - 0.25f, cam.orthographicSize - 0.25f);
    }

    protected virtual void Update()
    {
        ChangeFixed(); //I feel this could be more optimal but im not seeing it. cant go in OnMouseOver because it disables boxCol

        if(Input.GetKeyDown(KeyCode.Mouse0) && grabbable)
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(col.OverlapPoint(mousePoint))
            {
                if (lastGrabTime != Time.frameCount)
                {
                    lastGrabTime = Time.frameCount;
                    maxHeight = sr;
                    foreach (SpriteRenderer rend in GameObject.FindObjectsOfType<SpriteRenderer>())
                    {
                        if(rend.gameObject.GetComponent<Draggable>() != null || rend.gameObject.GetComponent<Rotatable>() != null)
                        {
                            if (rend.sortingOrder > maxHeight.sortingOrder && rend.gameObject.GetComponent<Collider2D>().OverlapPoint(mousePoint))
                                maxHeight = rend;
                        }
                    }
                }
                if(sr == maxHeight)
                {
                    startOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    dragged = true;
                    snapped = false;
                    sr.sortingOrder = 10;
                    grabSpot = transform.localPosition;
                    grabParent = transform.parent;
                    transform.parent = null;

                    if(source != null && pickupSounds.Length > 0)
                    {
                        source.clip = pickupSounds[Random.Range(0, pickupSounds.Length)];
                        source.Play();
                    }

                    //make it so this is the first draggable object on screen
                    Draggable[] objects = GameObject.FindObjectsOfType<Draggable>();
                    foreach (Draggable obj in objects)
                    {
                        if (obj == this)
                            continue;
                        else
                            obj.SendBack();
                    }

                    //!!!!! POTENTIAL FOR BUGS HERE FOR SURE
                    //just look out for children of objects like this
                    if (hasSnapPoints)
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (transform.GetChild(i).CompareTag("wheel"))
                                transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 1;

                            if (transform.GetChild(i).childCount == 0)
                                continue;

                            Draggable drag = transform.GetChild(i).GetChild(0).GetComponent<Draggable>();
                            if (drag != null)
                                drag.sr.sortingOrder = sr.sortingOrder + drag.snappingPriority;
                        }
                    }

                    OnGrabbed();
                }
            }
        }

        if (dragged && Input.GetKeyUp(KeyCode.Mouse0))
        {
            dragged = false;

            Vector2 goalPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - startOffset;
            goalPos = new Vector2(Mathf.Clamp(goalPos.x, -clampedExtent.x, clampedExtent.x), Mathf.Clamp(goalPos.y, -clampedExtent.y, clampedExtent.y));

            if (Snappable())
            {
                foreach (Transform point in snapPoints)
                {
                    if (Vector3.Distance(goalPos, point.position) < snapDist)
                    {
                        if (point.childCount == 0)
                        {
                            transform.position = point.position;
                            transform.parent = point;
                            snapped = true;
                            OnSnapped(point);
                            return;
                        }
                        else if(transform.CompareTag("Red Cube") || transform.CompareTag("Yellow Cube") || transform.CompareTag("Blue Cube"))
                        {
                            Transform otherBox = point.GetChild(0);
                            otherBox.parent = grabParent;
                            otherBox.localPosition = grabSpot;

                            transform.position = point.position;
                            transform.parent = point;
                            snapped = true;
                            OnSnapped(point);
                            return;
                        }
                    }
                }
            }

            if (source != null && dropSounds.Length > 0)
            {
                source.clip = dropSounds[Random.Range(0, dropSounds.Length)];
                source.Play();
            }
            OnDroppedNoSnap();
        }

        if (dragged)
        {
            Vector2 goalPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - startOffset;
            goalPos = new Vector2(Mathf.Clamp(goalPos.x, -clampedExtent.x, clampedExtent.x), Mathf.Clamp(goalPos.y, -clampedExtent.y, clampedExtent.y));

            transform.position = goalPos;
        }

        if (transform.name != "Screwdriver")
        {
            if (!dragged && !snapped && transform.position.y + sr.bounds.extents.y > 2.7f && !shelved)
            {
                if (!inAir)
                {
                    timeInAir = 0f;
                    inAir = true;
                }

                if (grabbable)
                    timeInAir += Time.deltaTime;
                transform.position -= new Vector3(0, 4, 0) * timeInAir;
            }

            if (dragged || snapped || transform.position.y + sr.bounds.extents.y <= 2.7f)
            {
                inAir = false;
            }
        }
        
    }

    protected virtual void OnGrabbed()
    {
        //override this
    }

    protected virtual void OnSnapped(Transform point)
    {
        if(transform.CompareTag("Red Cube") || transform.CompareTag("Yellow Cube") || transform.CompareTag("Blue Cube") || transform.CompareTag("lid"))
        {
            source.clip = dropSounds[Random.Range(0, dropSounds.Length)];
            source.Play();
        }
    }

    protected virtual void OnDroppedNoSnap()
    {
        //override this
    }

    //so ive decided to make this disable the boxcollider instead of just making it not grabbable because
    //when the player tries to pick up something like a lid that is screwed in, it will pick up the whole box
    private void ChangeFixed()
    {
        if (snapped)
        {
            foreach (Transform t in preventionSnaps)
            {
                if (t.childCount > 0)
                {
                    col.enabled = false;
                    return;
                }
            }
            col.enabled = true;
        }
    }
        

    private bool Snappable()
    {
        foreach (Transform t in preventionSnaps)
        {
            if (t.childCount > 0)
            {
                return false;
            }
        }
        return true;
    }

    public virtual void SendBack()
    {
        sr.sortingOrder -= 8;

        //!!!!! POTENTIAL FOR BUGS HERE FOR SURE
        //just look out for children of objects like this
        if (hasSnapPoints)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("wheel"))
                    transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder + 1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("shelf") && col.CompareTag("trinket"))
            shelved = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("shelf") && col.CompareTag("trinket"))
            shelved = false;
    }

}
