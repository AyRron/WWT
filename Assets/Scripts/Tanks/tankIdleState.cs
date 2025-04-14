using UnityEngine;

public class tankIdleState : StateMachineBehaviour
{
    AttackController attackController;

    public float followingDistance = 10f;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        // Check if there is an available target
        if (attackController.targetToAttack != null) {

            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);


            if (followingDistance < distanceFromTarget)
            {
                animator.SetBool("isFollowing", true);
            }

        }

    }

}
