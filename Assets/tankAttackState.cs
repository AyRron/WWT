using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class tankAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;
    Transform tankTurret;
    float turretRotationSpeed = 5f;

    public float stopAttackingDistance = 1.2f;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        attackController.SetAttackMaterial();

        Transform tankVisual = animator.transform.Find("TankVisual");
        Transform tankBody = tankVisual.Find("Body");
        tankTurret = tankBody.Find("Turret");

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        // If there is no other direct command to move
        if (attackController.targetToAttack != null && animator.transform.GetComponent<TankMovement>().isCommandeToMove == false)
        {
            LookAtTarget();
            //agent.SetDestination(animator.transform.position);
            //agent.SetDestination(attackController.targetToAttack.position);

        }
        
        float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

        if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
        {
            animator.SetBool("isAttacking", false);
        }


    }


    private void LookAtTarget()
    {
        if (tankTurret == null || attackController.targetToAttack == null)
        {
            Debug.LogWarning("Tourelle ou cible non trouvée !");
            return;
        }

        // Obtenir la direction vers la cible dans l'espace monde
        Vector3 directionToTarget = attackController.targetToAttack.position - tankTurret.position;

        // Si la direction est nulle, éviter une erreur
        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        // Convertir la direction en espace local (prendre en compte l'orientation du tank)
        Vector3 localDirection = tankTurret.parent.InverseTransformDirection(directionToTarget);

        // Calculer l'angle dans le plan XY (ou XZ selon ton modèle)
        float angleZ = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        float targetAngle = angleZ + 180f;
        float smoothAngle = Mathf.LerpAngle(tankTurret.localRotation.eulerAngles.z, targetAngle, 0.3f);
        tankTurret.localRotation = Quaternion.Euler(0, 0, smoothAngle);


    }

















    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.isStopped = false;
    }

}
