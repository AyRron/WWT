using Script.Script_Camera;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FocusTanks : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int targetTankId; // ID à configurer dans l'Inspector
    public CameraController cameraController;

    private GameObject _highlightArea;
    private Transform tankTransform;

    private void Start()
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClick);
        }

        // Démarre une coroutine avec un léger délai
        StartCoroutine(FindTankAfterDelay(1f));
    }

    private IEnumerator FindTankAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject tank = TankSelectionCameraManager.Instance.GetTankById(targetTankId);
        if (tank != null)
        {
            tankTransform = tank.transform;
            _highlightArea = tankTransform.Find("HighlightArea")?.gameObject;

            if (_highlightArea != null)
                _highlightArea.SetActive(false);
        }
        else
        {
            Debug.LogError($"FocusTanks : Aucun tank trouvé avec l’ID {targetTankId}");
        }
    }

    void OnButtonClick()
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;
        if (cameraController == null || tankTransform == null) return;

        cameraController.SetTargetTank(targetTankId);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;
        if (_highlightArea != null)
            _highlightArea.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;
        if (_highlightArea != null)
            _highlightArea.SetActive(false);
    }
}
