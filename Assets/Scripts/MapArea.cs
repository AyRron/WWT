using UnityEngine;
using System.Collections.Generic;
using System;

public class MapArea : MonoBehaviour
{
    public enum State { Neutral, OnCaptured, Captured }
    public enum CurrentAttacker { Allies, Ennemies, None }
    public enum OwnerZone { Allies, Ennemies, None }

    public event EventHandler OnCaptured;

    private List<MapAreaCollider> mapAreaColliderListe;
    private HashSet<Tank> listeTankAreaInside = new HashSet<Tank>();

    public float propgressAllies = 0f;
    public float propgressEnnemie = 0f;
    private float progressSpead = 1f;
    private float timeForCapture = 2f;

    private float speedScore = 1.5f;

    private State state = State.Neutral;
    private OwnerZone ownerZone = OwnerZone.None;
    private CurrentAttacker currentAttacker = CurrentAttacker.None;

    public GameManager gameManager;

    public GameObject linesZone; // pour les couleurs de la zone

    private void Awake()
    {
        mapAreaColliderListe = new List<MapAreaCollider>();
        foreach (Transform child in transform)
        {
            MapAreaCollider mapAreaCollider = child.GetComponent<MapAreaCollider>();
            if (mapAreaCollider != null) mapAreaColliderListe.Add(mapAreaCollider);
        }

        InvokeRepeating(nameof(IncreaseScore), 1f, 1f);

        SetZoneColor(Color.gray);
    }

    private void Update()
    {
        UpdateTanksInside();
        ProcessCaptureProgress();
        HandleStateLogic();
    }

    private void UpdateTanksInside()
    {
        listeTankAreaInside.Clear();

        foreach (MapAreaCollider mapAreaCollider in mapAreaColliderListe)
        {
            foreach (Tank tankInsideArea in mapAreaCollider.GetPlayerList())
            {
                listeTankAreaInside.Add(tankInsideArea);
                
            }
        }

        int alliesCount = 0;
        int enemiesCount = 0;

        foreach (Tank tank in listeTankAreaInside)
        {
            switch (tank.tag)
            {
                case "Player":
                    alliesCount++;
                    break;
                case "Enemy":
                    enemiesCount++;
                    break;
            }
        }

        currentAttacker = alliesCount > enemiesCount
            ? CurrentAttacker.Allies
            : enemiesCount > alliesCount
                ? CurrentAttacker.Ennemies
                : CurrentAttacker.None;

    }

    private void ProcessCaptureProgress()
    {
        bool progressing = false;
        if (currentAttacker != CurrentAttacker.None)
        {
            if (currentAttacker == CurrentAttacker.Allies)
            {
                propgressAllies += progressSpead * Time.deltaTime;
                progressing = true;
            }
            else if (currentAttacker == CurrentAttacker.Ennemies)
            {
                propgressEnnemie += progressSpead * Time.deltaTime;
                progressing = true;
            }
        }

        // Changement d'état visuel si progression mais pas encore capturé
        if (progressing && state != State.Captured && state != State.OnCaptured)
        {
            state = State.OnCaptured;
            SetZoneColor(Color.Lerp(Color.gray, Color.yellow, 0.5f));
        }


        if (propgressAllies >= timeForCapture)
        {
            CaptureZone(OwnerZone.Allies);
            SetZoneColor(Color.green);
        }
        else if (propgressEnnemie >= timeForCapture)
        {
            CaptureZone(OwnerZone.Ennemies);
            SetZoneColor(Color.red);
        }
    }

    private void CaptureZone(OwnerZone newOwner)
    {
        ownerZone = newOwner;
        propgressAllies = 0f;
        propgressEnnemie = 0f;
        state = State.Captured;
        OnCaptured?.Invoke(this, EventArgs.Empty);
    }

    private void HandleStateLogic()
    {
        if (state == State.Captured)
        {
            foreach (Tank tank in listeTankAreaInside)
            {
                if (ownerZone == OwnerZone.Allies && tank.CompareTag("Enemy"))
                {
                    currentAttacker = CurrentAttacker.Ennemies;
                }
                else if (ownerZone == OwnerZone.Ennemies && tank.CompareTag("Player"))
                {
                    currentAttacker = CurrentAttacker.Allies;
                }
            }
        }
    }

    private void IncreaseScore()
    {
        if (state == State.Captured)
        {
            if (ownerZone == OwnerZone.Allies) gameManager.scoreAllies += speedScore;
            else if (ownerZone == OwnerZone.Ennemies) gameManager.scoreEnemies += speedScore;
        }
    }

    public void SetZoneColor(Color newColor)
    {
        if (linesZone == null) return;

        foreach (Transform line in linesZone.transform)
        {
            Renderer renderer = line.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = newColor;
            }
        }
    }
}
