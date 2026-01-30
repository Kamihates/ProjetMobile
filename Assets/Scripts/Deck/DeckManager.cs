using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private DominoData dominoData;

    [BoxGroup("Deck"), SerializeField, Min(1)] private int initialDominoInHandSize = 3;
    [BoxGroup("Deck"), SerializeField, Min(2)] private int deckSize = 10;

    private List<DominoCombination> deck = new();
    private List<DominoCombination> dominoInHand = new();

    [ReadOnly, SerializeField, Foldout("Debug")]
    private DominoCombination currentDomino;

    public Action<DominoCombination> OnSpawnDomino;
    

    private void Awake()
    {
        if (dominoData.allDominos.Count == 0)
            dominoData.GenerateAllCombinations();

        GeneratePlayerDeck();
        FillHandFromDeck();
        SpawnNextDomino();
    }

    private void GeneratePlayerDeck()
    {
        deck.Clear();
        var pool = new List<DominoCombination>(dominoData.allDominos);

        for (int i = 0; i < deckSize && pool.Count > 0; i++)
        {
            int rnd = UnityEngine.Random.Range(0, pool.Count);
            deck.Add(pool[rnd]);
            pool.RemoveAt(rnd);
        }
    }

    private void FillHandFromDeck()
    {
        dominoInHand.Clear();

        for (int i = 0; i < initialDominoInHandSize && deck.Count > 0; i++)
        {
            dominoInHand.Add(deck[0]);
            deck.RemoveAt(0);
        }
    }

    public void SpawnNextDomino()
    {
        if (dominoInHand.Count == 0)
        {
            FillHandFromDeck();
        }

        currentDomino = dominoInHand[0];
        dominoInHand.RemoveAt(0);

        OnSpawnDomino?.Invoke(currentDomino);
    }

    [Button("Place Current Domino")] //Fonction temporaire pour tester le placement du domino
    public void PlaceCurrentDomino()
    {
        if (currentDomino == null) return;
        deck.Add(currentDomino);
        SpawnNextDomino();
    }
}
