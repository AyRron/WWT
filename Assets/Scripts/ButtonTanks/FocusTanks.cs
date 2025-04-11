using Script.Script_Camera;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusTanks : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Transform tankTransform;
    public CameraController cameraController; // Référence à CameraController
    private GameObject _highlightArea; // La zone de surbrillance au sol

    private void Start()
    {
        // Ajouter l'écouteur d'événement de clic au bouton
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonClick);
        
        if (tankTransform == null) return;

        _highlightArea = tankTransform.transform.Find("HighlightArea")?.gameObject;

        if (_highlightArea != null)
        {
            _highlightArea.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Aucune zone de surbrillance trouvée sous le tank");
        }
    }

    void OnButtonClick()
    {
        FocusOnTank();
    }

    private void FocusOnTank()
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        if (tankTransform == null || cameraController == null) return;
        cameraController.player = tankTransform; // Changer le tank ciblé dans CameraController
        cameraController.isFollowingPlayer = true; // Activer le suivi du tank
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        if (_highlightArea != null)
        {
            _highlightArea.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        if (_highlightArea != null)
        {
            _highlightArea.SetActive(false);
        }
    }
}