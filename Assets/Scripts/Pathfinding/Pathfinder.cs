using UnityEngine;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour
{
    public Grid grid; // Référence au script Grid
    public Transform seeker, target; // Seeker: objet qui cherche le chemin, Target: objet à atteindre
    public float updateRate = 0.5f; // Fréquence de mise à jour du chemin
    public bool pathFound = false; // Indique si un chemin a été trouvé
    public float agentRadius = 1f; // Rayon de l'agent pour la vérification des collisions
    public Vector3 agentSize = new Vector3(2f, 1f, 3f); // Taille du tank (largeur, hauteur, longueur)
    public float obstacleAvoidanceWeight = 2.0f; // Poids du coût de proximité aux obstacles dans l'algorithme A*

    void Awake()
    {
        if (grid == null)
            grid = GetComponent<Grid>();
            
        // Synchroniser la taille de l'agent avec celle de la grille
        if (grid != null)
        {
            agentRadius = grid.agentRadius;
            agentSize = grid.agentSize;
        }
    }

    void Update()
    {
        if (target == null)
        {
            //Debug.LogWarning("Pathfinder: Target est null");
            pathFound = false;
            return;
        }
        
        if (seeker == null)
        {
            //Debug.LogWarning("Pathfinder: Seeker est null");
            pathFound = false;
            return;
        }
        
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (grid == null)
        {
            //Debug.LogError("Pathfinder: Grid est null");
            pathFound = false;
            return;
        }
        
        // Calculer les positions des extrémités du tank
        Vector3 tankRight = transform.right * (agentSize.x / 2);
        Vector3 leftStartPos = startPos - tankRight;
        Vector3 rightStartPos = startPos + tankRight;
        Vector3 leftTargetPos = targetPos - tankRight;
        Vector3 rightTargetPos = targetPos + tankRight;
        
        // Obtenir les nœuds pour les positions de départ et d'arrivée
        Node leftStartNode = grid.NodeFromWorldPoint(leftStartPos);
        Node rightStartNode = grid.NodeFromWorldPoint(rightStartPos);
        Node leftTargetNode = grid.NodeFromWorldPoint(leftTargetPos);
        Node rightTargetNode = grid.NodeFromWorldPoint(rightTargetPos);
        
        // Vérifier si tous les nœuds sont valides
        if (leftStartNode == null || rightStartNode == null || leftTargetNode == null || rightTargetNode == null)
        {
            //Debug.LogError("Pathfinder: Impossible de trouver les nœuds de départ ou d'arrivée");
            pathFound = false;
            return;
        }
        
        // Vérifier si les nœuds sont accessibles
        if (!leftStartNode.walkable || !rightStartNode.walkable)
        {
            //Debug.LogWarning("Pathfinder: Les nœuds de départ ne sont pas accessibles");
            pathFound = false;
            return;
        }
        
        if (!leftTargetNode.walkable || !rightTargetNode.walkable)
        {
            //Debug.LogWarning("Pathfinder: Les nœuds d'arrivée ne sont pas accessibles");
            pathFound = false;
            return;
        }

        // Utiliser le nœud central pour le calcul du chemin
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        
        // Réinitialiser les coûts
        for (int x = 0; x < grid.gridSizeX; x++)
        {
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                grid.grid[x, y].gCost = 0;
                grid.grid[x, y].hCost = 0;
                grid.grid[x, y].parent = null;
            }
        }

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                
                // Vérifier si le chemin est accessible pour les deux extrémités du tank
                if (IsPathValidForTankWidth(grid.path, leftStartPos, rightStartPos))
                {
                    pathFound = true;
                    //Debug.Log($"Chemin trouvé avec {grid.path.Count} points");
                }
                else
                {
                    pathFound = false;
                    //Debug.LogWarning("Chemin trouvé mais non accessible pour la largeur du tank");
                    grid.path = new List<Vector3>();
                }
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Calculer le coût de déplacement en tenant compte de la proximité aux obstacles
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                
                // Ajouter un coût supplémentaire basé sur la proximité aux obstacles
                float obstacleCost = (currentNode.obstacleProximityCost + neighbour.obstacleProximityCost) * obstacleAvoidanceWeight;
                newMovementCostToNeighbour += Mathf.RoundToInt(obstacleCost * 10);
                
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        
        // Si on arrive ici, aucun chemin n'a été trouvé
        //Debug.LogWarning("Pathfinder: Aucun chemin trouvé vers la cible");
        pathFound = false;
        grid.path = new List<Vector3>();
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    // Vérifie si le chemin est valide pour la largeur du tank
    bool IsPathValidForTankWidth(List<Vector3> path, Vector3 leftStartPos, Vector3 rightStartPos)
    {
        if (path == null || path.Count < 2)
            return false;
            
        // Calculer les positions des extrémités du tank
        Vector3 tankRight = transform.right * (agentSize.x / 2);
        
        // Vérifier chaque segment du chemin
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            
            // Calculer les positions des extrémités pour ce segment
            Vector3 leftStart = start - tankRight;
            Vector3 rightStart = start + tankRight;
            Vector3 leftEnd = end - tankRight;
            Vector3 rightEnd = end + tankRight;
            
            // Vérifier si les deux extrémités sont accessibles
            if (!grid.IsPositionWalkable(leftStart, new Vector3(0.1f, agentSize.y, agentSize.z), transform.rotation) ||
                !grid.IsPositionWalkable(rightStart, new Vector3(0.1f, agentSize.y, agentSize.z), transform.rotation) ||
                !grid.IsPositionWalkable(leftEnd, new Vector3(0.1f, agentSize.y, agentSize.z), transform.rotation) ||
                !grid.IsPositionWalkable(rightEnd, new Vector3(0.1f, agentSize.y, agentSize.z), transform.rotation))
            {
                return false;
            }
        }
        
        return true;
    }
} 