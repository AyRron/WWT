using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;





    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack != null) 
        {
            Debug.Log("fuit");
            targetToAttack = null;
        }
    }

    public void SetIdleMaterial()
    {
        // Animation arret
        
    }

    public void SetFollowMaterial()
    {
        // Animation suivre
        
    }

    public void SetAttackMaterial()
    {
        // Animation attack
    }

  

}
