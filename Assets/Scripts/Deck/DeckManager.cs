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

    private List<List<RegionData>> deck = new();
    private List<List<RegionData>> discard = new();
    private List<List<RegionData>> dominoInHand = new();

    [SerializeField] private DominoSpawner dominoSpawner;
    

    private void Start()
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

        for (int i = 0; i < deckSize && dominoData.allDominos.Count > 0; i++)
        {
            int rnd = UnityEngine.Random.Range(0, dominoData.allDominos.Count);
            deck.Add(dominoData.allDominos[rnd]);
            dominoData.allDominos.RemoveAt(rnd);
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

        dominoSpawner.OnDominoSpawn?.Invoke(dominoInHand[0]);
        dominoInHand.RemoveAt(0);
    }

}
