using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using System;

public class MapArea : MonoBehaviour
{
    // State de la Area
    public enum State
    {
        Neutral,
        Captured
    }

    public enum CurrentAttacker
    {
        Allies,
        Ennemies
    }

    // Gérer l'évenement de capture de zone
    // TODO : Utiliser l'event pour gérer l'affichage
    public event EventHandler OnCaptured;

    // liste des colliders liées
    private List<MapAreaCollider> mapAreaColliderListe;
    // avancement de la capture de zone
    private float propgress;
    private float progressSpead = 1f;
    private float timeForCapture = 2f;

    // score
    private float scoreArea = 0f;
    private float speedScore = 1.5f;

    // State
    private State state;

    // Area attackers
    private CurrentAttacker currentAttacker;

    // GameManager
    private GameManager gameManager;


    // Awake is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.mapAreaColliderListe = new List<MapAreaCollider>();


        foreach (Transform child in transform)
        {
            MapAreaCollider mapAreaCollider = child.GetComponent<MapAreaCollider>();

            if (mapAreaCollider != null)
            {
                mapAreaColliderListe.Add(mapAreaCollider);
            }
        }
        this.state = State.Neutral;

        // Invoque la méthode IncreaseScore toute les secondes
        InvokeRepeating(nameof(IncreaseScore), 1f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        // Gérer la state
        switch (state)
        {
            // Si la zone n'est pas capturé
            case State.Neutral:
                List<Tank> listeTankAreaInside = new List<Tank>();

                foreach (MapAreaCollider mapAreaCollider in mapAreaColliderListe)
                {
                    foreach (Tank tankInsideArea in mapAreaCollider.GetPlayerList())
                    {
                        if (!listeTankAreaInside.Contains(tankInsideArea))
                        {
                            if (gameManager.GetTanksAllies().Contains(tankInsideArea))
                            {
                                // Tank allié dans la zone
                            }
                            listeTankAreaInside.Add(tankInsideArea);
                            // animation de la zone à gérer ici
                        }
                    }
                }
                this.propgress += listeTankAreaInside.Count * progressSpead * Time.deltaTime;

                //Debug.Log("Player count Inside :" + listePlayerAreaInside.Count + ", progress :" + propgress);

                if(this.propgress >= this.timeForCapture)
                {
                    state = State.Captured;
                    OnCaptured?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Zone capturé !");
                }

                break;
            case State.Captured:
                // Si la zone est capturé

                break;
        }
    }

    private void IncreaseScore()
    {
        if(this.state == State.Captured)
        {
            this.scoreArea += speedScore;
            Debug.Log("Score de la zone :" + scoreArea);
        }
    }
}
