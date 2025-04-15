using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public float obstacleProximityCost; // Coût de proximité aux obstacles

    public int gCost; // Coût depuis le nœud de départ
    public int hCost; // Coût estimé jusqu'au nœud d'arrivée
    public Node parent; // Référence au nœud parent pour reconstruire le chemin

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, float _obstacleProximityCost = 0f)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        obstacleProximityCost = _obstacleProximityCost;
    }
} 