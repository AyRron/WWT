using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour {
    public float speed = 5f;
    private List<Node> path;

    public void SetPath(List<Node> newPath) {
        Debug.Log("SetPath reçu – longueur : " + newPath.Count);
        StopAllCoroutines();
        path = newPath;
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath() {
        foreach (Node node in path) {
            Vector3 targetPos = node.worldPosition;

            // Tourner vers la cible
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.forward = direction;  // Cela oriente le PNJ vers la cible

            // Se déplacer vers la cible
            while ((transform.position - targetPos).sqrMagnitude > 0.04f) {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    void OnDrawGizmos() {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < path.Count - 1; i++) {
            Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
            Gizmos.DrawSphere(path[i].worldPosition, 0.2f);
        }

        // Dernier point
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(path[path.Count - 1].worldPosition, 0.3f);
    }
}
