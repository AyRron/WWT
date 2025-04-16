using UnityEngine;

namespace Script.Script_Camera
{
    public class CameraController : MonoBehaviour
    {
        public int targetTankId; // ID du tank à suivre
        private GameObject targetTank; // Référence au tank ciblé

        public float smoothSpeed = 0.125f;
        public Vector3 offset;

        public float edgeSize = 100.0f; // Taille des bords pour mouvement libre
        public float moveSpeed = 100.0f; // Vitesse de la caméra
        private bool isFollowingTank = true;

        private void Start()
        {
            targetTank = TankSelectionCameraManager.Instance.GetTankById(targetTankId); // Initialisation du tank à suivre
        }

        private void Update()
{
    if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

    // Sortir du mode suivi si souris sur le bord
    if (Input.mousePosition.x >= Screen.width - edgeSize ||
        Input.mousePosition.x <= edgeSize ||
        Input.mousePosition.y >= Screen.height - edgeSize ||
        Input.mousePosition.y <= edgeSize)
    {
        isFollowingTank = false;
    }

    // Revenir au suivi si espace est pressé
    if (Input.GetKeyDown(KeyCode.Space))
    {
        isFollowingTank = true;
    }

    if (isFollowingTank && targetTank != null)
    {
        // Suivi du tank
        Vector3 desiredPosition = targetTank.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(targetTank.transform);
    }
    else
    {
        // Mouvement libre avec accélération selon la position de la souris
        Vector3 moveDirection = Vector3.zero;

        float speedModifier = 10f; // Facteur d'accélération basé sur la position de la souris

        if (Input.mousePosition.x >= Screen.width - edgeSize)
        {
            speedModifier = Mathf.Lerp(1f, 2f, (Input.mousePosition.x - (Screen.width - edgeSize)) / edgeSize);
            moveDirection += Vector3.right;
        }

        if (Input.mousePosition.x <= edgeSize)
        {
            speedModifier = Mathf.Lerp(1f, 2f, (edgeSize - Input.mousePosition.x) / edgeSize);
            moveDirection += Vector3.left;
        }

        if (Input.mousePosition.y >= Screen.height - edgeSize)
        {
            speedModifier = Mathf.Lerp(1f, 2f, (Input.mousePosition.y - (Screen.height - edgeSize)) / edgeSize);
            moveDirection += Vector3.forward;
        }

        if (Input.mousePosition.y <= edgeSize)
        {
            speedModifier = Mathf.Lerp(1f, 2f, (edgeSize - Input.mousePosition.y) / edgeSize);
            moveDirection += Vector3.back;
        }

        transform.Translate(moveDirection * moveSpeed * speedModifier * Time.deltaTime, Space.World);
    }
}


        public void SetTargetTank(int newTargetTankId)
        {
            targetTankId = newTargetTankId;
            targetTank = TankSelectionCameraManager.Instance.GetTankById(targetTankId);
            isFollowingTank = true;
        }

        public void FocusOn(Transform tankTransform)
        {
            Tank tankScript = tankTransform.GetComponent<Tank>();
            if (tankScript != null)
            {
                SetTargetTank(tankScript.id);
            }
        }
    }
}
