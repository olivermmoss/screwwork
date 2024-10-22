
using UnityEngine;

public class Screw : Draggable
{
    public Animator animator;
    public bool startScrewed;
    public AudioClip[] startSnappin;
    public AudioClip intoTheAbyss;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        //snappingPriority = 3;
        animator.SetBool("Snapped", snapped);
        animator.SetBool("Screwed", startScrewed);
        source = GetComponent<AudioSource>();
    }

    protected override void OnGrabbed()
    {
        animator.SetBool("Snapped", false);
        animator.SetBool("Dragged", true);
    }

    protected override void OnSnapped(Transform point)
    {
        if (transform.parent.name == "Red_Snap" || transform.parent.name == "Blue_Snap")
        {
            animator.Play("Screw_Abyss");
            source.clip = intoTheAbyss;
            source.pitch = Random.Range(0.7f, 1.3f);
            source.Play();
        }

        else
        {
            animator.SetBool("Snapped", true);
            animator.SetBool("Dragged", false);
            source.clip = startSnappin[Random.Range(0, startSnappin.Length)];
            source.Play();
            //sr.sortingOrder = 11;
        }
    }

    protected override void OnDroppedNoSnap()
    {
        animator.SetBool("Dragged", false);
    }

    public void OnScrewed()
    {
        if (!animator.GetBool("Screwed"))
        {
            animator.SetBool("Screwed", true);
            grabbable = false;
        }

        else
        {
            animator.SetBool("Screwed", false);
            //setting grabbable to true is handled by the exit of Screw_Unrotate animation so the player cannot move the screw before while it is still fastened
        }
            
    }

}