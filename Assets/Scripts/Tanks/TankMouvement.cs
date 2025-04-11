using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TankMovement : MonoBehaviour
{
    // access to nav mesh Agent (floor)
    [SerializeField] private NavMeshAgent _agent = null;

    public LayerMask ground;
    public bool isCommandToMove;


    private void Start()
    {
        _agent.updateRotation = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning)
        {
            _agent.isStopped = true;  // Arrête le tank quan le jeu est en pause
            return;
        }

        _agent.isStopped = false; // Reprend le mouvement si le jeu est actif

        if (Input.GetMouseButtonDown(1))
        {
                var maxDistance = Mathf.Infinity;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxDistance, ground)){
                    Debug.Log("click");
                    isCommandToMove = true;
                    _agent.SetDestination(hit.point);
                }  
        }

        // V�rification si l'agent a atteint sa destination
        if (_agent.hasPath == false || _agent.remainingDistance == _agent.stoppingDistance)
        {
            Debug.Log("agent a atteint sa destination");
            isCommandToMove=false;
        }
    }
}
