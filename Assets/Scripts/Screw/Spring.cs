using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : Draggable
{
    private Animator animator;
    public AudioClip intoTheAbyss;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        source = GetComponent<AudioSource>();
    }

    protected override void OnGrabbed()
    {
        animator.Play("Spring_Pickup");

    }

    protected override void OnSnapped(Transform point)
    {
        animator.Play("Spring_Abyss");
        source.clip = intoTheAbyss;
        source.pitch = Random.Range(0.7f, 1.3f);
        source.Play();
    }

    protected override void OnDroppedNoSnap()
    {
        animator.Play("Spring_Drop");
    }
}
