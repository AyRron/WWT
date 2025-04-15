using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float agentRadius = 1f; // Rayon de l'agent (ennemi) pour la vérification des collisions
    public Vector3 agentSize = new Vector3(2f, 1f, 3f); // Taille du tank (largeur, hauteur, longueur)
    public Node[,] grid;
    public int gridSizeX, gridSizeY;
    public float safetyMargin = 0.5f; // Marge de sécurité pour éviter les collisions
    public float groundHeight = 0f; // Hauteur du sol pour les points du chemin
    public float obstacleAvoidanceFactor = 1.5f; // Facteur pour augmenter la distance de sécurité autour des obstacles

    float nodeDiameter;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Calculer la taille effective pour la vérification des collisions
        Vector3 effectiveSize = agentSize + Vector3.one * safetyMargin;
        effectiveSize *= obstacleAvoidanceFactor; // Augmenter la zone de vérification

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                worldPoint.y = groundHeight; // Définir la hauteur Y à la hauteur du sol
                
                // Vérifier si le nœud est accessible en tenant compte de la taille de l'agent
                bool walkable = !Physics.CheckBox(worldPoint, effectiveSize / 2, Quaternion.identity, unwalkableMask);
                
                // Calculer un coût de proximité aux obstacles
                float obstacleProximityCost = CalculateObstacleProximityCost(worldPoint, effectiveSize);
                
                grid[x, y] = new Node(walkable, worldPoint, x, y, obstacleProximityCost);
            }
        }
        
        Debug.Log($"Grille créée: {gridSizeX}x{gridSizeY} nœuds, taille du monde: {gridWorldSize}, taille de l'agent: {agentSize}, marge: {safetyMargin}, hauteur du sol: {groundHeight}, facteur d'évitement: {obstacleAvoidanceFactor}");
    }

    // Calcule un coût de proximité aux obstacles pour ce nœud
    float CalculateObstacleProximityCost(Vector3 position, Vector3 checkSize)
    {
        // Vérifier plusieurs points autour du nœud pour évaluer la proximité des obstacles
        int numChecks = 8; // Nombre de points à vérifier autour du nœud
        float totalCost = 0f;
        
        for (int i = 0; i < numChecks; i++)
        {
            float angle = i * (360f / numChecks);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 checkPoint = position + direction * (checkSize.magnitude / 2);
            
            // Utiliser un raycast pour détecter les obstacles
            RaycastHit hit;
            if (Physics.Raycast(position, direction, out hit, checkSize.magnitude, unwalkableMask))
            {
                // Plus l'obstacle est proche, plus le coût est élevé
                float distance = hit.distance;
                float cost = 1f - (distance / checkSize.magnitude);
                totalCost += cost;
            }
        }
        
        return totalCost / numChecks; // Retourner le coût moyen
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Vérifier si la position est dans les limites de la grille
        float percentX = (worldPosition.x - (transform.position.x - gridWorldSize.x / 2)) / gridWorldSize.x;
        float percentY = (worldPosition.z - (transform.position.z - gridWorldSize.y / 2)) / gridWorldSize.y;
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        
        // Vérifier si les indices sont valides
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
        {
            Debug.LogWarning($"Position hors limites: {worldPosition}, indices: {x}, {y}");
            return null;
        }

        return grid[x, y];
    }

    // Vérifie si un chemin est accessible pour un agent de taille donnée
    public bool IsPathWalkable(List<Vector3> path, Vector3 agentSize)
    {
        if (path == null || path.Count < 2)
            return false;
            
        // Ajouter une marge de sécurité pour éviter les collisions
        Vector3 sizeWithMargin = agentSize + Vector3.one * safetyMargin;
        sizeWithMargin *= obstacleAvoidanceFactor; // Augmenter la zone de vérification
            
        // Vérifier chaque segment du chemin
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            
            // S'assurer que les points sont à la bonne hauteur
            start.y = groundHeight;
            end.y = groundHeight;
            
            // Vérifier plusieurs points le long du segment
            int numChecks = 10; // Nombre de points à vérifier le long du segment
            int widthChecks = 5; // Nombre de points à vérifier le long de la largeur
            
            for (int j = 0; j <= numChecks; j++)
            {
                float t = j / (float)numChecks;
                Vector3 basePoint = Vector3.Lerp(start, end, t);
                basePoint.y = groundHeight;
                
                // Vérifier plusieurs points le long de la largeur du tank
                for (int w = 0; w < widthChecks; w++)
                {
                    float widthOffset = (w / (float)(widthChecks - 1) - 0.5f) * agentSize.x;
                    Vector3 checkPoint = basePoint + transform.right * widthOffset;
                    
                    // Vérifier si ce point est accessible pour l'agent
                    if (Physics.CheckBox(checkPoint, new Vector3(0.1f, sizeWithMargin.y, sizeWithMargin.z), Quaternion.identity, unwalkableMask))
                    {
                        Debug.LogWarning($"Chemin non accessible: collision détectée à {checkPoint}");
                        return false;
                    }
                }
            }
        }
        
        return true;
    }

    // Vérifie si une position est accessible pour l'agent
    public bool IsPositionWalkable(Vector3 position, Vector3 agentSize, Quaternion rotation)
    {
        // S'assurer que la position est à la bonne hauteur
        position.y = groundHeight;
        
        // Ajouter une marge de sécurité pour éviter les collisions
        Vector3 sizeWithMargin = agentSize + Vector3.one * safetyMargin;
        sizeWithMargin *= obstacleAvoidanceFactor;
        
        // Vérifier plusieurs points le long de la largeur du tank
        int widthChecks = 5;
        for (int w = 0; w < widthChecks; w++)
        {
            float widthOffset = (w / (float)(widthChecks - 1) - 0.5f) * agentSize.x;
            Vector3 checkPoint = position + transform.right * widthOffset;
            
            if (Physics.CheckBox(checkPoint, new Vector3(0.1f, sizeWithMargin.y, sizeWithMargin.z), rotation, unwalkableMask))
            {
                return false;
            }
        }
        
        return true;
    }

    public List<Vector3> path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                // Utiliser une couleur basée sur le coût de proximité aux obstacles
                Color nodeColor = Color.Lerp(Color.white, Color.red, n.obstacleProximityCost);
                Gizmos.color = n.walkable ? nodeColor : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
        
        // Visualiser le chemin
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
} 