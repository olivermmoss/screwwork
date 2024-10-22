using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;


public class Driver : Draggable
{
    public Screw screwScript;

    public Animator animator;
    public CircleCollider2D circleCollider;

    public Collider2D screwCollider;

    public AudioClip[] clips;
    public AudioClip[] startSnappin;

    private bool dropped = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        col = GetComponent<BoxCollider2D>();
        source = GetComponent<AudioSource>();
    }

    public float speed = 0.67f;

    protected override void OnDroppedNoSnap()
    {
        if (screwScript != null && screwScript.animator.GetBool("Snapped"))
        {
            if (!screwScript.animator.GetBool("Screwed"))
            {
                grabbable = false; //undone by "Drive" animation
                transform.position = new Vector3(screwScript.transform.position.x, screwScript.transform.position.y + 2.2f, transform.position.z);

                animator.Play("Drive");
                animator.SetBool("Flipped", true);
                Shift(-0.35f, speed);
                source.clip = clips[0];
                source.Play();
            }

            else
            {
                grabbable = false; //undone by "Undrive" animation
                transform.position = new Vector3(screwScript.transform.position.x, screwScript.transform.position.y + 1.85f, transform.position.z);

                animator.Play("Undrive");
                animator.SetBool("Flipped", false);
                Shift(0.35f, speed);
                source.clip = clips[1];
                source.Play();
            }

            screwScript.OnScrewed();

        }
        else if (!screwScript)
        {
            animator.Play("Dropped");
            dropped = true;
        }
            
    }

    protected override void OnGrabbed()
    {
        if (dropped)
            animator.Play("Undropped");

        dropped = false;
    }

    //This will be changed later if there is time because if you move the box the screw is being fixed into, the screw moves but not the screwdriver
    IEnumerator ShiftCoroutine(Vector3 targetPosition, float duration)
    {
        float timeElapsed = 0f;
        Vector3 startingPosition = transform.position;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }


    private void Shift(float distance, float duration)
    {
        Vector3 targetPosition = transform.position + new Vector3(0, distance, 0);
        StartCoroutine(ShiftCoroutine(targetPosition, duration));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("screw") && dragged)
        {
            screwScript = other.GetComponent<Screw>();
            if (screwScript.animator.GetBool("Screwed"))
                animator.SetBool("Flipped", true);
            else
                animator.SetBool("Flipped", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("screw") && dragged)
        {
            screwScript = null;
            animator.SetBool("Flipped", false);
        }
    }

    protected override void OnSnapped(Transform point)
    {
        source.clip = startSnappin[Random.Range(0, startSnappin.Length)];
        source.Play();
    }
}
