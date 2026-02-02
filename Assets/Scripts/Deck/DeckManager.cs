using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CombinaisonData combiData;

    [BoxGroup("Deck"), SerializeField, Min(1)] private int initialDominoInHandSize = 3;
    [BoxGroup("Deck"), SerializeField, Min(2)] private int deckSize = 10;
    [BoxGroup("Deck"), SerializeField] private bool shuffleDeck = true;

    [BoxGroup("Deck"), SerializeField] private List<DominoInfos> deck = new();

    [BoxGroup("Debug"), SerializeField] private List<DominoInfos> discard = new();
    [BoxGroup("Debug"), SerializeField] private List<DominoInfos> dominoInHand = new();

    [SerializeField, Required] private DominoSpawner dominoSpawner;
    

    private void Start()
    {

        GridManager.Instance.OnDominoPlaced += HandleNextDomino;

        if (combiData.allDominos.Count == 0)
            combiData.GenerateAllCombinations();

        // Si le deck est vide on le genere
        if (deck.Count == 0)
        {
            Debug.Log("Deck not initialized, deck is now generated auto");
            GeneratePlayerDeck();
        }

        for (int i = 0; i < initialDominoInHandSize; i++)
            FillHandFromDeck();

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

        for (int i = 0; i < initialDominoInHandSize; i++)
        {
            if (deck.Count >= 1)
            {
                dominoInHand.Add(deck[0]);
                deck.RemoveAt(0);
            }
            
        }

        

        /*        dominoInHand.Clear(); // On retire le clear pour pas perdre les dominos deja en main si le deck est regen

                for (int i = 0; i < initialDominoInHandSize && deck.Count > 0; i++)
                {
                    dominoInHand.Add(deck[0]);
                    deck.RemoveAt(0); // On remove le domino en main de l'index 0 plutot quand le domino est plac� sur la grille
                }*/
    }

    public void SpawnNextDomino()
    {
        if (dominoInHand.Count == 0)
        {
            Debug.Log("main vide");
            return;
        }

        // on spawn le premier domino de la main

        DominoInfos currentDomino = dominoInHand[0];

        dominoSpawner.OnDominoSpawn?.Invoke(currentDomino);

        dominoInHand.RemoveAt(0);
    }

    private void RefillDeck()
    {
        if (discard.Count == 0)
            return;

        deck.AddRange(discard);
        discard.Clear();

        if (shuffleDeck)
            ShuffleDeck(deck);
    }

    private void ShuffleDeck(List<DominoInfos> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            DominoInfos dominoInfos = list[i]; // on stock temporairement le domino a la position actuelle
            list[i] = list[random]; // On met le domino aleatoire a la place de celui actuel
            list[random] = dominoInfos; // Et on met le domino initial a la place du domino al�atoire
        }
    }


    private void DiscardDomino(List<DominoInfos> dominoInfos) 
    {
        foreach(DominoInfos domino in dominoInfos)
            discard.Add(domino);

        if (dominoInHand.Count == 0)
        {
            FillHandFromDeck();
        }
    }

    private void HandleNextDomino(DominoPiece dominoPiece)
    {
        // si le domino qui à été placé n'est pas le domino actuel on ne se charge pas de le discard ou autre car il était déjà sur le terrain
        if (dominoPiece != dominoSpawner.CurrentDomino) return;

        DiscardDomino(new List<DominoInfos> {dominoPiece.Data});
        SpawnNextDomino();
    }


}
