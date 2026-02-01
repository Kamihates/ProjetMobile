using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CombinaisonData combiData;

    [BoxGroup("Deck"), SerializeField, Min(1)] private int initialDominoInHandSize = 3;
    [BoxGroup("Deck"), SerializeField, Min(2)] private int deckSize = 10;

    private List<DominoInfos> deck = new();
    private List<DominoInfos> discard = new();
    private List<DominoInfos> dominoInHand = new();

    [SerializeField] private DominoSpawner dominoSpawner;
    

    private void Start()
    {
        if (combiData.allDominos.Count == 0)
            combiData.GenerateAllCombinations();

        GeneratePlayerDeck();
        FillHandFromDeck();
        SpawnNextDomino();
    }


    private void GeneratePlayerDeck()
    {
        Debug.Log("AllDominos count: " + combiData.allDominos.Count);
        for (int i = 0; i < combiData.allDominos.Count; i++)
            Debug.Log(i + " => " + combiData.allDominos[i].Regions[0].Type + " | " + combiData.allDominos[i].Regions[1].Type);


        deck.Clear();
        for (int i = 0; i < deckSize && combiData.allDominos.Count > 0; i++)
        {
            int rnd = UnityEngine.Random.Range(0, combiData.allDominos.Count);
            deck.Add(combiData.allDominos[rnd]);
            combiData.allDominos.RemoveAt(rnd);

            
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

        if (dominoInHand.Count == 0)
        {
            Debug.Log("Deck vide");
            return;
        }

        dominoSpawner.OnDominoSpawn?.Invoke(dominoInHand[0]);
        dominoInHand.RemoveAt(0);
    }

}
