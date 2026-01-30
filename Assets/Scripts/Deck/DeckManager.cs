using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DeckManager : MonoBehaviour
{
    [Header("Deck"), SerializeField]
    private List<DominoData> deck = new();

    [Header("Queue"), SerializeField, Min(1)]
    private int initialDominoInHandSize = 3;

    private List<DominoData> dominoInHand = new();

    [ReadOnly, SerializeField, Foldout("Debug")]
    private DominoData currentDomino;

    public Action<DominoData> OnSpawnDomino;

    public DominoData CurrentDomino
    {
        get => currentDomino;
        private set => currentDomino = value;
    }

    private void Awake()
    {
        InitDominoInHand();
        SpawnNextDomino();
    }

    private void InitDominoInHand()
    {
        dominoInHand.Clear();
        for (int i = 0; i < initialDominoInHandSize; i++)
        {
            dominoInHand.Add(GetRandomDomino());
        }
    }

    private DominoData GetRandomDomino()
    {
        if (deck.Count == 0)
        {
            Debug.LogError("Deck vide");
            return null;
        }

        int domino = UnityEngine.Random.Range(0, deck.Count);
        return deck[domino];
    }

    public void SpawnNextDomino()
    {
        if (dominoInHand.Count == 0)
        {
            RefillLDominoInHand();
        }

        currentDomino = dominoInHand[0];
        dominoInHand.RemoveAt(0);

        OnSpawnDomino?.Invoke(currentDomino);
    }

    [Button("Place Current Domino")] //Fonction temporaire pour tester le placement du domino
    public void PlaceCurrentDomino()
    {
        if (currentDomino == null)
        {
            Debug.LogWarning("Aucun currentDomino à placer");
            return;
        }

        Debug.Log($"currentDomino placé : {currentDomino.name}");

        SpawnNextDomino();
    }

    public void DiscardDominoInHand()
    {
        dominoInHand.Clear();
        RefillLDominoInHand();
    }

    public void RefillLDominoInHand()
    {
        for (int i = 0; i < initialDominoInHandSize; i++)
        {
            dominoInHand.Add(GetRandomDomino());
        }
    }
}
