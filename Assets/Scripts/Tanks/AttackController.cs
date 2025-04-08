using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;

    public string enemyTag;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(enemyTag) && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(enemyTag) && targetToAttack != null) 
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
