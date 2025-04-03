using UnityEngine;
using System.Collections.Generic;
using System;

public class MapArea : MonoBehaviour
{
    public enum State { Neutral, Captured }
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

    private void Awake()
    {
        mapAreaColliderListe = new List<MapAreaCollider>();
        foreach (Transform child in transform)
        {
            MapAreaCollider mapAreaCollider = child.GetComponent<MapAreaCollider>();
            if (mapAreaCollider != null) mapAreaColliderListe.Add(mapAreaCollider);
        }

        InvokeRepeating(nameof(IncreaseScore), 1f, 1f);
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

        currentAttacker = listeTankAreaInside.Count > 0 ? GetCurrentAttacker() : CurrentAttacker.None;
    }

    private CurrentAttacker GetCurrentAttacker()
    {
        foreach (Tank tank in listeTankAreaInside)
        {
            if (gameManager.tanksAllies.Contains(tank)) return CurrentAttacker.Allies;
            if (gameManager.tanksEnnemies.Contains(tank)) return CurrentAttacker.Ennemies;
        }
        return CurrentAttacker.None;
    }

    private void ProcessCaptureProgress()
    {
        if (currentAttacker == CurrentAttacker.Allies)
        {
            propgressAllies += progressSpead * Time.deltaTime;
        }
        else if (currentAttacker == CurrentAttacker.Ennemies)
        {
            propgressEnnemie += progressSpead * Time.deltaTime;
        }

        if (propgressAllies >= timeForCapture)
        {
            CaptureZone(OwnerZone.Allies);
        }
        else if (propgressEnnemie >= timeForCapture)
        {
            CaptureZone(OwnerZone.Ennemies);
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
                if (ownerZone == OwnerZone.Allies && gameManager.tanksEnnemies.Contains(tank))
                {
                    currentAttacker = CurrentAttacker.Ennemies;
                }
                else if (ownerZone == OwnerZone.Ennemies && gameManager.tanksAllies.Contains(tank))
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
            else if (ownerZone == OwnerZone.Ennemies) gameManager.scoreEnnemies += speedScore;

            Debug.Log($"Score allie: {gameManager.scoreAllies}, score ennemies: {gameManager.scoreEnnemies}");
        }
    }
}
