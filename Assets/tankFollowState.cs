using UnityEngine;
using UnityEngine.AI;

public class tankFollowState : StateMachineBehaviour
{
    AttackController attackController;

    NavMeshAgent agent;

    public float attackingDistance = 10f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Should Unit Transition to Idle state
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            // If there is no other direct command to move
            if (animator.transform.GetComponent<TankMovement>().isCommandeToMove == false)
            {
                // Moving Unit towards target
                agent.SetDestination(attackController.targetToAttack.position);

                // Calculate the direction to the target
                Vector3 direction = attackController.targetToAttack.position - animator.transform.position;

                // Prevent the rotation on the X and Y axis (keep Y axis rotation for horizontal facing)
                direction.y = 0;

                if (direction.sqrMagnitude > 0.01f)  // Check if the direction is significant
                {
                    // Create a rotation towards the target on Y axis
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // Apply only the Y axis rotation to the animator's transform (so X and Z do not change)
                    animator.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                }

                float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                if (distanceFromTarget < attackingDistance)
                {
                    Debug.Log("pass attacking");

                    agent.SetDestination(animator.transform.position);

                    animator.SetBool("isAttacking", true);
                }
            }

        }
    }



    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
