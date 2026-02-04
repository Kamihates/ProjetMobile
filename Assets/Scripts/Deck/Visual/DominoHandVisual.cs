using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DominoHandVisual : MonoBehaviour
{
    [SerializeField, Required] private DeckManager deckManager;
    [SerializeField, Required] private Transform handVisualParent;
    [SerializeField, Range(0,5)] private float spaceWithinDomino = 2f;
    [Header("Prefab"), SerializeField] private GameObject dominoPrefab;

    private List<DominoPiece> dominoInHandVisual = new();
    public List<DominoPiece> DominoInHandVisual => dominoInHandVisual;

    public void SpawnDominoHandVisual(int index)
    {
        GameObject dominoGO = Instantiate(dominoPrefab, handVisualParent);

        dominoGO.transform.localPosition = new Vector2(index * spaceWithinDomino, 0f);

        DominoPiece dominoPiece = dominoGO.GetComponent<DominoPiece>();
        dominoPiece.Init(index, 0, deckManager.DominoInHand[0]);

        dominoInHandVisual.Add(dominoPiece);

    }
}