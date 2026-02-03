using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CombinaisonData combiData;

    [BoxGroup("Deck"), SerializeField, Min(1)] private int initialDominoInHandSize = 3;
    [BoxGroup("Deck"), SerializeField, Min(2)] private int deckSize = 10;
    [BoxGroup("Deck"), SerializeField] private bool shuffleDeck = true;

    [BoxGroup("Deck"), SerializeField] private DeckStartingData startingDeckData;
    [BoxGroup("Deck"), SerializeField] private DominoHandVisual dominoHandVisual;

    [BoxGroup("Debug"), SerializeField, ReadOnly] private List<DominoInfos> deck = new();
    [BoxGroup("Debug"), SerializeField, ReadOnly] private List<DominoInfos> discard = new();
    [BoxGroup("Debug"), SerializeField, ReadOnly] private List<DominoInfos> dominoInHand = new();
    public List<DominoInfos> DominoInHand => dominoInHand;

    [SerializeField, Required] private DominoSpawner dominoSpawner;
    

    private void Start()
    {

        deck = new List<DominoInfos>(startingDeckData.Dominos);

        GridManager.Instance.OnDominoPlaced += HandleNextDomino;

        if (combiData.allDominos.Count == 0)
            combiData.GenerateAllCombinations();

        // Si le deck est vide on le genere
        if (deck.Count == 0)
        {
            Debug.Log("Deck not initialized, deck is now generated auto");
            GeneratePlayerDeck();
        }

        SpawnNextDomino();
    }

    private void OnDestroy()
    {
        GridManager.Instance.OnDominoPlaced -= HandleNextDomino;
    }


    private void GeneratePlayerDeck()
    {
        deck.Clear();
        discard.Clear();
        dominoInHand.Clear();

        // Copie temporaire de tout les dominos disponibles dans le deck pour ne pas modifier les combinaisons globales
        List<DominoInfos> dominosInDeck = new List<DominoInfos>(combiData.allDominos); 

        Debug.Log("AllDominos count: " + combiData.allDominos.Count);

        for(int i=0; i < deckSize && dominosInDeck.Count > 0; i++)
        {
            int randomDominosFromDeck = Random.Range(0, dominosInDeck.Count);
            deck.Add(dominosInDeck[randomDominosFromDeck]);
            dominosInDeck.RemoveAt(randomDominosFromDeck);
        }

        if (shuffleDeck)
            ShuffleDeck(deck);

        for (int i = 0; i < combiData.allDominos.Count; i++)
            Debug.Log(i + " => " + combiData.allDominos[i].Regions[0].Type + " | " + combiData.allDominos[i].Regions[1].Type);


        /*        for (int i = 0; i < deckSize && combiData.allDominos.Count > 0; i++)
                {
                    int randomDomino = UnityEngine.Random.Range(0, combiData.allDominos.Count);
                    deck.Add(combiData.allDominos[randomDomino]);
                    combiData.allDominos.RemoveAt(randomDomino);
                }*/
    }

    private void FillHandFromDeck()
    {
        if (deck.Count == 0)
            RefillDeck();

        if (deck.Count == 0)
            return;

        dominoInHand.Add(deck[0]);
        deck.RemoveAt(0);        

        dominoHandVisual.SpawnDominoHandVisual(dominoInHand.Count - 1);


    }

    public void SpawnNextDomino()
    {
        while (dominoInHand.Count < initialDominoInHandSize)
        {
            FillHandFromDeck();
        }

        if (dominoInHand.Count == 0)
        {
            Debug.Log("main vide");
            return;
        }
        DominoInfos DominoToSpawn = dominoInHand[0];

        //dominoSpawner.OnDominoSpawn?.Invoke(currentDomino);
        //dominoInHand.RemoveAt(0);

    }

    private void RefillDeck()
    {
        // Si le deck a encore des cartes, rien à faire
        if (deck.Count > 0)
            return;

        // Si discard contient des cartes, on les remet dans le deck
        if (discard.Count > 0)
        {
            deck.AddRange(discard);
            discard.Clear();

            if (shuffleDeck)
                ShuffleDeck(deck);
        }
        // Si discard est vide, on rebuild le deck depuis le SO de départ
        else
        {
            deck = new List<DominoInfos>(startingDeckData.Dominos);

            if (shuffleDeck)
                ShuffleDeck(deck);
        }
    }

    private void ShuffleDeck(List<DominoInfos> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            DominoInfos dominoInfos = list[i]; // on stock temporairement le domino a la position actuelle
            list[i] = list[random]; // On met le domino al�atoire � la place de celui actuel
            list[random] = dominoInfos; // Et on met le domino initial � la place du domino al�atoire
        }
    }


    private void DiscardDomino(List<DominoInfos> dominoInfos) 
    {
        foreach(DominoInfos domino in dominoInfos)
            discard.Add(domino);

        if (dominoInHand.Count == 0)
        {

        }
    }

    private void HandleNextDomino(DominoPiece dominoPiece)
    {
        DiscardDomino(new List<DominoInfos> {dominoPiece.Data});
        SpawnNextDomino();
    }


}
