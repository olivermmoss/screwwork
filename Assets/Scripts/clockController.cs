using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clockController : MonoBehaviour
{
    public bool going = false;
    public Sprite[] clockframes;
    float startTime;
    float dayLength;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void StartClock(float len)
    {
        dayLength = len;
        startTime = Time.time;
        going = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(going)
        {
            sr.sprite = clockframes[Mathf.Clamp(Mathf.FloorToInt(clockframes.Length * (Time.time - startTime) / dayLength), 0, clockframes.Length - 1)];
        }
    }
}
