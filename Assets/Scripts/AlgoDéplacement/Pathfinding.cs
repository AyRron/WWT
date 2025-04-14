using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    public Transform seeker;
    public Transform target;
    private GridManager grid;
    public UnitMover unitMover;
    private Vector3 lastTargetPos;


    void Start() {
        grid = GetComponent<GridManager>();
    }

    void Update() {
        if (seeker && target && grid.grid != null)
    {
        if (target.position != lastTargetPos)
        {
            FindPath(seeker.position, target.position);
            lastTargetPos = target.position;
        }
    }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Debug.DrawLine(startPos, targetPos, Color.cyan);

        var openSet = new List<Node>();
        var closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || 
                    openSet[i].fCost == currentNode.fCost && 
                    openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode) {

        List<Node> path = new List<Node>();
        Node current = endNode;


        while (current != startNode) {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();        

        Debug.Log("RetracePath OK – path length : " + path.Count);
        foreach (Node n in path) {
            Debug.Log("Path point: " + n.worldPosition);
        }

        seeker.GetComponent<UnitMover>().SetPath(path);
    }

    int GetDistance(Node a, Node b) {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        return (dstX > dstY) ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }
}
