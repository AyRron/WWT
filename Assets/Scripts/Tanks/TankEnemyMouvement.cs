using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankEnemyMovement : MonoBehaviour
{
    // access to nav mesh Agent (floor)
    [SerializeField] private NavMeshAgent _agent = null;

    public LayerMask ground;
    public bool isCommandeToMove;

    // Liste des points où les tanks ennemis peuvent se déplacer
    public List<Transform> areaPoints; // Liste des zones définies (GameObjects ou points d'intérêt)

    private bool _hasSetDestination = false;

    private void Start()
    {
        _agent.updateRotation = true;
        // Si on ne trouve pas de points, on peut récupérer tous les objets de type "Area" ou similaires.
        if (areaPoints.Count == 0)
        {
            // Exemple de récupération des points d'une certaine couche
            GameObject[] points = GameObject.FindGameObjectsWithTag("Area");
            foreach (var point in points)
            {
                areaPoints.Add(point.transform); // Ajoute les points à la liste
            }
        }
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

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning)
        {
            _agent.isStopped = true;  // Arrête le tank quand le jeu est en pause
            return;
        }

        _agent.isStopped = false; // Reprend le mouvement si le jeu est actif

        // Mouvement aléatoire pour les tanks ennemis
        if (!_hasSetDestination && areaPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, areaPoints.Count); // Choisir un index aléatoire
            Vector3 randomDestination = areaPoints[randomIndex].position; // Prendre la position du point

            _agent.SetDestination(randomDestination);
            _hasSetDestination = true;
        }

        // Si la destination a été définie et que le tank a atteint la destination
        if (_hasSetDestination && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            isCommandeToMove = false;
            _hasSetDestination = false;
        }
    }
}
