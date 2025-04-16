using UnityEngine;

namespace Script.Script_Camera
{
    public class CameraController : MonoBehaviour
    {
        public int targetTankId; // ID du tank à suivre
        private GameObject targetTank; // Référence au tank ciblé

        public float smoothSpeed = 0.125f;
        public Vector3 offset;

        public float edgeSize = 2.0f;
        public float moveSpeed = 100.0f; // Vitesse de la caméra
        private bool isFollowingTank = true;

        private void Start()
        {
            targetTank = TankSelectionCameraManager.Instance.GetTankById(targetTankId); // Initialisation du tank à suivre
        }

        private void Update()
        {
            if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

            if (isFollowingTank && targetTank != null)
            {
                Vector3 desiredPosition = targetTank.transform.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;

                transform.LookAt(targetTank.transform);
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
