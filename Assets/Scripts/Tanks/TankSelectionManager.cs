using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class TankSelectionManager : MonoBehaviour
{
    public static TankSelectionManager Instance { get; set; }

    public List<GameObject> allTanksList = new List<GameObject>();
    public List<GameObject> tanksSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;
    public GameObject groundMarker;
    private Camera mainCamera;

    public bool attackCursorVisible;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else {
            Instance = this;
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("La caméra principale n'a pas été trouvée dans la scène. Assurez-vous qu'une caméra est marquée comme 'MainCamera'.");
                enabled = false;
                return;
            }
        }
    }
    void Update()
    {
        if (mainCamera == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            float maxDistance = Mathf.Infinity;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, clickable))
            {
                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        MultiSelect(hit.collider.gameObject);
                    } 
                    else 
                    {
                        SelectByClicking(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    DeselectAll();
                }
            }
        }

        // Attack target
        if (tanksSelected.Count > 0 && AtleastOneOffensiveTank(tanksSelected))
        {
            float maxDistance = Mathf.Infinity;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, attackable))
            {
                Debug.Log("enemy");

                attackCursorVisible = true;

                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = hit.transform;

                    foreach (GameObject tank in tanksSelected)
                    {
                        if (tank.GetComponent<AttackController>())
                        {
                            tank.GetComponent<AttackController>().targetToAttack = target;
                        }

                    }
                }
            } else
            {
                attackCursorVisible = false;
            }
        }
    }

    private bool AtleastOneOffensiveTank(List<GameObject> tanksSelected)
    {
        foreach (GameObject tank in tanksSelected)
        {
            if (tank.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;

    }

    private void MultiSelect(GameObject tank)
    {
        if (tank == null) return;

        if (tanksSelected.Contains(tank) == false)
        {
            tanksSelected.Add(tank);
            TriggerSelectionIndicator(tank, true);
            EnableTankMovement(tank, true);
        }
        else
        {
            EnableTankMovement(tank, false);
            TriggerSelectionIndicator(tank, false);
            tanksSelected.Remove(tank);
        }
    }

    private void DeselectAll()
    {
        if (tanksSelected == null) return;

        foreach (var tank in tanksSelected)
        {
            if (tank != null)
            {
                EnableTankMovement(tank, false);
                TriggerSelectionIndicator(tank, false);
            }
        }

        if (groundMarker != null)
        {
            groundMarker.SetActive(false);
        }

        tanksSelected.Clear();
    }

    private void SelectByClicking(GameObject tank)
    {
        if (tank == null) return;

        DeselectAll();

        tanksSelected.Add(tank);
        TriggerSelectionIndicator(tank, true);
        EnableTankMovement(tank, true);
    }

    private void EnableTankMovement(GameObject tank, bool shouldMove)
    {
        if (tank == null) return;

        var movement = tank.GetComponent<TankMovement>();
        if (movement != null)
        {
            movement.enabled = shouldMove;
        }
    }

    private void TriggerSelectionIndicator(GameObject tank, bool isVisible)
    {
        if (tank == null) return;

        var selectionIndicator = tank.transform.GetChild(1)?.gameObject;
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(isVisible);
        }
    }
}
