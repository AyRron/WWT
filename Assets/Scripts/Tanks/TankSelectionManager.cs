using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public class TankSelectionManager : MonoBehaviour
{
    public static TankSelectionManager Instance { get; set; }

    public List<GameObject> allTanksList = new List<GameObject>();
    public List<GameObject> tanksSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;
    public GameObject groundMarker;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else {

            Instance = this;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float maxDistance = Mathf.Infinity;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelect(hit.collider.gameObject);
                } else {
                    SelectByClicking(hit.collider.gameObject);
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

        //if (Input.GetMouseButtonDown(1) && tanksSelected.Count > 0)
        //{
        //    float maxDistance = Mathf.Infinity;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, maxDistance, ground))
        //    {
        //        groundMarker.transform.position = hit.point;

        //        groundMarker.SetActive(false);
        //        groundMarker.SetActive(true);
        //    }
        //}
    }

    private void MultiSelect(GameObject tank)
    {
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
        foreach (var tank in tanksSelected)
        {
            EnableTankMovement(tank, false);
            TriggerSelectionIndicator(tank, false);
        }

        groundMarker.SetActive(false);

        tanksSelected.Clear();
    }

    private void SelectByClicking(GameObject tank)
    {
        DeselectAll();

        tanksSelected.Add(tank);

        TriggerSelectionIndicator(tank, true);

        EnableTankMovement(tank, true);

    }

    private void EnableTankMovement(GameObject tank, bool shouldMove)
    {
        tank.GetComponent<TankMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject tank, bool isVisible)
    {
        tank.transform.GetChild(1).gameObject.SetActive(isVisible);
    }
}
