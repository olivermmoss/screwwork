using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    private BoxCollider2D col;

    public GameObject prefabToAdd; //either screw or spring
    Draggable prefabsDraggable;

    QuotaButton qButton;
    public GameObject mainBox;


    public Transform redSnap;
    public Transform blueSnap;

    public Transform lid;

    public AudioClip[] grabSounds;
    //public AudioClip deleteSounds;
    AudioSource source;

    void Start()
    {
        qButton = (QuotaButton)FindObjectOfType(typeof(QuotaButton)).GetComponent<QuotaButton>();
        col = GetComponent<BoxCollider2D>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col.OverlapPoint(mousePoint))
            {
                GameObject item = Instantiate(prefabToAdd, mousePoint, Quaternion.identity);
                prefabsDraggable = item.GetComponent<Draggable>();
                prefabsDraggable.snapPoints = new Transform[] { redSnap, blueSnap };
                prefabsDraggable.preventionSnaps = new Transform[] { lid };
                item.tag = "SortItem";
                item.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + prefabsDraggable.snappingPriority;
                if (item.GetComponent<SpriteRenderer>().sortingOrder < mainBox.GetComponent<SpriteRenderer>().sortingOrder)
                        item.GetComponent<SpriteRenderer>().sortingOrder = mainBox.GetComponent<SpriteRenderer>().sortingOrder + prefabsDraggable.snappingPriority;

                prefabsDraggable.snapDist = 1.25f;
                prefabsDraggable.dragged = true;

                source.clip = grabSounds[Random.Range(0, grabSounds.Length)];
                source.Play();
            }

            
            

            //prefabsDraggable.

        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> results = new();
            col.OverlapCollider(filter, results);

            int sortOrd = GetComponent<SpriteRenderer>().sortingOrder;

            foreach (Collider2D coll in results)
            {
                if (coll.CompareTag("SortItem") && coll.GetComponent<SpriteRenderer>().sortingOrder > sortOrd) 
                {
                    Destroy(coll.gameObject);
                }
            }
        }
    }
}
