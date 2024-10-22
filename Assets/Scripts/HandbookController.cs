using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HandbookController : MonoBehaviour
{

    public Sprite[] version1;
    public Sprite[] version2;
    public Sprite smallIcon;
    private BoxCollider2D col;
    private BoxCollider2D forwardCol;
    private BoxCollider2D backCol;
    private SpriteRenderer sr;
    public QuotaButton quotaButton;
    public Transform handbookSnap;
    public int page;

    private Vector2 previousOffset;
    private Vector2 previousSize;

    private bool small = false;

    AudioSource source;
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        forwardCol = transform.GetChild(0).GetComponent<BoxCollider2D>();
        backCol = transform.GetChild(1).GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        page = 0;

        source = GetComponent<AudioSource>();

        if (quotaButton.day >= 2)
            sr.sprite = version2[page];
        else
            sr.sprite = version1[page];
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x - handbookSnap.position.x <= 4f && transform.position.y - handbookSnap.position.y <= 4f)
        {
            sr.sprite = smallIcon;

            previousOffset = col.offset;
            previousSize = col.size;
            col.offset = new Vector2(0, 0);
            col.size = new Vector2(2.166646f, 2.6f);

            forwardCol.enabled = false;
            backCol.enabled = false;

            small = true;
        }
        else if (small)
        {
            if (quotaButton.day >= 2)
                sr.sprite = version2[page];
            else
                sr.sprite = version1[page];

            col.offset = previousOffset;
            col.size = previousSize;

            forwardCol.enabled = true;
            if (page != 0)
                backCol.enabled = true;

            small = false;
        }

        if (page == 0)
        {
            backCol.enabled = false;
            col.offset = new Vector2(2.125f, 0);
            col.size = new Vector2(4.24f, 5.125f);
        }
        else
        {
            backCol.enabled = true;
            col.offset = new Vector2(0, 0);
            col.size = new Vector2(8.5f, 5.125f);
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (forwardCol.OverlapPoint(mousePoint))
            {
                if (quotaButton.day >= 2){
                    if (page < 7)
                    {
                        page++;
                        source.clip = clips[Random.Range(0, clips.Length)];
                        source.Play();
                    }
                        

                    sr.sprite = version2[page];
                }

                else
                {
                    if (page < 6)
                    {
                        page++;
                        source.clip = clips[Random.Range(0, clips.Length)];
                        source.Play();
                    }

                    sr.sprite = version1[page];
                }

                
            }

            else if (backCol.OverlapPoint(mousePoint))
            {
                if (page > 0)
                {
                    page--;
                    source.clip = clips[Random.Range(0, clips.Length)];
                    source.Play();
                }

                if (quotaButton.day >= 2)
                {
                    sr.sprite = version2[page];
                }

                else
                {
                    sr.sprite = version1[page];
                }
            }
        }
    }
}
