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

    public void SpawnDominoHandVisual(int uid, int index)
    {
        GameObject dominoGO = Instantiate(dominoPrefab, handVisualParent);

        dominoGO.transform.localPosition = new Vector2(index * spaceWithinDomino, 0f);

        DominoPiece dominoPiece = dominoGO.GetComponent<DominoPiece>();
        dominoPiece.Init(uid, 0, deckManager.DominoInHand[index]);

        dominoInHandVisual.Add(dominoPiece);

    }

    public void UpdateDominoHandVisual()
    {
        for (int i = 0; i < dominoInHandVisual.Count; i++)
        {
            dominoInHandVisual[i].transform.localPosition = new Vector2(i * spaceWithinDomino * dominoInHandVisual.Count, 0f);
        }
    }
}