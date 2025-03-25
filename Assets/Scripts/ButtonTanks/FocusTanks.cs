using UnityEngine;

public class FocusTanks : MonoBehaviour
{
    public Camera mainCamera;
    public Transform tankTransform;

    void Start()
    {
        // Ajouter l'écouteur d'événement de clic au bouton
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        FocusOnTank();
    }

    void FocusOnTank()
    {
        if (mainCamera != null && tankTransform != null)
        {
            mainCamera.transform.position = tankTransform.position + new Vector3(0, 50, -50);
            mainCamera.transform.LookAt(tankTransform);
        }
    }
}