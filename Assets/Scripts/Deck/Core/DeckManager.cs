using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CombinaisonData combiData;

    [BoxGroup("Deck"), SerializeField, Min(1)] private int initialDominoInHandSize = 3;
    [BoxGroup("Deck"), SerializeField, Min(2)] private int deckSize = 10;
    [BoxGroup("Deck"), SerializeField] private bool shuffleDeck = true;

    [BoxGroup("Deck"), SerializeField] private DeckStartingData startingDeckData;
    [BoxGroup("Deck"), SerializeField] private DominoHandVisual dominoHandVisual;
    public DominoHandVisual HandVisual => dominoHandVisual;

    [BoxGroup("Debug"), SerializeField, ReadOnly] private List<DominoInfos> deck = new();
    [BoxGroup("Debug"), SerializeField, ReadOnly] private List<DominoInfos> discard = new();
    [BoxGroup("Debug"), SerializeField, ReadOnly] private List<DominoInfos> dominoInHand = new();
    public List<DominoInfos> DominoInHand => dominoInHand;

    private List<RegionData> regionDataT1;

    [SerializeField, Required] private DominoSpawner dominoSpawner;

    public Action OnT1InHand;

    private void OnEnable()
    {
        regionDataT1 = new List<RegionData>(Resources.LoadAll<RegionData>("ScriptableObjects/T1"));
    }


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

        dominoSpawner.SpawnNextDomino();

    }

    private void OnDestroy()
    {
        if (GridManager.Instance != null)
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
            int randomDominosFromDeck = UnityEngine.Random.Range(0, dominosInDeck.Count);
            deck.Add(dominosInDeck[randomDominosFromDeck]);
            dominosInDeck.RemoveAt(randomDominosFromDeck);
        }

        if (shuffleDeck)
            ShuffleDeck(deck);

        //for (int i = 0; i < combiData.allDominos.Count; i++)
        //    Debug.Log(i + " => " + combiData.allDominos[i].Regions[0].Type + " | " + combiData.allDominos[i].Regions[1].Type);


        /*        for (int i = 0; i < deckSize && combiData.allDominos.Count > 0; i++)
                {
                    int randomDomino = UnityEngine.Random.Range(0, combiData.allDominos.Count);
                    deck.Add(combiData.allDominos[randomDomino]);
                    combiData.allDominos.RemoveAt(randomDomino);
                }*/
    }


    private int dominoUID = 0;
    private void FillHandFromDeck()
    {
        if (deck.Count == 0)
            RefillDeck();

        if (deck.Count == 0)
            return;

        dominoInHand.Add(deck[0]);

        if (deck[0].IsDominoFusion)
            OnT1InHand.Invoke();


        deck.RemoveAt(0);

        dominoUID++;

        

        int newIndex = dominoUID;
        dominoHandVisual.SpawnDominoHandVisual(newIndex, dominoInHand.Count - 1);


    }

    public RegionData GetT1Data(RegionType type)
    {
        foreach (RegionData t1 in regionDataT1)
        {
            if (t1.Type == type)
            {
                return t1;
            }
        }
        return null;
    }

    public void PutT1InDeck(List<Vector2Int> fusionIndex)
    {
        // on recupere la region 

        RegionData region = GridManager.Instance.GetRegionAtIndex(new Vector2Int(fusionIndex[0].y, fusionIndex[0].x)).Region;
        RegionType T1Type = region.Type;

        foreach (RegionData t1 in regionDataT1)
        {
            if(t1.Type == T1Type)
            {
                DominoInfos dominoInfos = new DominoInfos()
                {
                    Regions = new List<RegionData>() { t1, t1 },
                    IsDominoFusion = true,
                    
                };

                dominoInfos.SetPower(fusionIndex.Count);


                if (deck.Count <= 0)
                {
                    deck.Insert(0, dominoInfos);
                    return;
                }

                for (int i = 0; i < deck.Count; i++)
                {
                    if (!deck[i].IsDominoFusion)
                    {
                        deck.Insert(i, dominoInfos);
                        Debug.Log("on ajoute le t1 dans le deck");
                        return;
                    }

                }
            }
        }


    }

    public DominoPiece GetNextDominoInHand()
    {
        while (dominoInHand.Count < initialDominoInHandSize)
        {
            FillHandFromDeck();
        }

        if (dominoInHand.Count == 0)
        {
            Debug.Log("main vide");
            return null;
        }

        if (dominoHandVisual.DominoInHandVisual.Count <= 0)
        {
            Debug.LogWarning("visuel vide...");
            return null;
        }    
        return dominoHandVisual.DominoInHandVisual[0];

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
            int random = UnityEngine.Random.Range(i, list.Count);
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
        Debug.Log("current domino = " + GameManager.Instance.CurrentDomino);
        // si le domino qui à été placé n'est pas le domino actuel on ne se charge pas de le discard ou autre car il était déjà sur le terrain
        if (GameManager.Instance.CurrentDomino == null)
        {
            return;
        }

        if (dominoPiece.PieceUniqueId != GameManager.Instance.CurrentDomino.PieceUniqueId) return;

        GameManager.Instance.CurrentDomino = null;

        if(!dominoPiece.Data.IsDominoFusion) 
            DiscardDomino(new List<DominoInfos> {dominoPiece.Data});

        dominoSpawner.SpawnNextDomino();

    }


}
