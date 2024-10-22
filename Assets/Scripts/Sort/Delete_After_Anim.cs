using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delete_After_Anim : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.transform.parent.name == "Red_Snap")
        {
            if (animator.GetComponent<Screw>())
                animator.transform.parent.parent.GetComponent<SortBox>().redSide[0]++;
            else if (animator.GetComponent<Spring>())
                animator.transform.parent.parent.GetComponent<SortBox>().redSide[1]++;
        }
        else if (animator.transform.parent.name == "Blue_Snap")
        {
            if (animator.GetComponent<Screw>())
                animator.transform.parent.parent.GetComponent<SortBox>().blueSide[0]++;
            else if (animator.GetComponent<Spring>())
                animator.transform.parent.parent.GetComponent<SortBox>().blueSide[1]++;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
