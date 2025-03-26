using Script.Script_Camera;
using UnityEngine;

public class FocusTanks : MonoBehaviour
{

    public Transform tankTransform;
    public CameraController cameraController; // Référence à CameraController

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
        if (tankTransform != null && cameraController != null)
        {
            cameraController.player = tankTransform; // Changer le tank ciblé dans CameraController
        }
    }
}