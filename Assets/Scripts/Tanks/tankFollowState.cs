using UnityEngine;
using UnityEngine.AI;

public class tankFollowState : StateMachineBehaviour
{
    AttackController attackController;

    NavMeshAgent agent;

    public float attackingDistance = 10f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            bool hasCommandeToMove = false;

            // Vérifie s'il y a un TankMovement (joueur)
            TankMovement playerMovement = animator.transform.GetComponent<TankMovement>();
            if (playerMovement != null)
            {
                hasCommandeToMove = playerMovement.isCommandeToMove;
            }

            // Sinon, vérifie s'il y a un TankEnemyMovement (IA)
            else
            {
                TankEnemyMovement enemyMovement = animator.transform.GetComponent<TankEnemyMovement>();
                if (enemyMovement != null)
                {
                    hasCommandeToMove = enemyMovement.isCommandeToMove;
                }
            }

            if (!hasCommandeToMove)
            {
                agent.SetDestination(attackController.targetToAttack.position);

                float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                if (distanceFromTarget < attackingDistance)
                {
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
