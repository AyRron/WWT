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
    public bool isCommandeToMove;

    private void Start()
    {
        _agent.updateRotation = true;
    }

    private void OnEnable()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _hasSetDestination = false;
            isCommandeToMove = false;
        }
    }


    // Update is called once per frame
    private bool _hasSetDestination = false;

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning)
        {
            _agent.isStopped = true;  // Arrête le tank quan le jeu est en pause
            return;
        }

        _agent.isStopped = false; // Reprend le mouvement si le jeu est actif

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                isCommandeToMove = true;
                _agent.SetDestination(hit.point);
                _hasSetDestination = true;
            }
        }

        //_hasSetDestination == true → c’est un mouvement qu’on a déclenché, pas un hasard
        //_agent.pathPending == false → Unity a fini de calculer le chemin
        //_agent.remainingDistance <= _agent.stoppingDistance → Le tank est réellement arrivé

        if (_hasSetDestination && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            isCommandeToMove = false;
            _hasSetDestination = false;
        }
    }
}
