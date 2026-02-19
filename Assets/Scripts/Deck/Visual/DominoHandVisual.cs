using NaughtyAttributes;
using System;
using System.Collections;
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

        
        //dominoGO.transform.localPosition = new Vector2(index * spaceWithinDomino, 0f);
        dominoGO.transform.localPosition = new Vector2(index * - spaceWithinDomino, 0f);

        DominoPiece dominoPiece = dominoGO.GetComponent<DominoPiece>();
        dominoPiece.Init(uid, 0, deckManager.DominoInHand[index]);

        dominoInHandVisual.Add(dominoPiece);
    }

    private Coroutine _currentCoroutine = null;
    public void UpdateDominoHandVisual()
    {
        if ( _currentCoroutine == null )
        {
            _currentCoroutine = StartCoroutine(UpdateQueueVisual());
        }
    }


    IEnumerator UpdateQueueVisual()
    {
        for (int i = 0; i < dominoInHandVisual.Count; i++)
        {
            Vector2 startingPos = dominoInHandVisual[i].transform.position;
            Vector2 EndPos = new Vector2((startingPos.x + spaceWithinDomino), handVisualParent.position.y);

            GeneralVisualController.Instance.FallAtoB(dominoInHandVisual[i].transform, 1f, startingPos, EndPos);
            // dominoInHandVisual[i].transform.localPosition = new Vector2(handVisualParent.position.x - (i * spaceWithinDomino), 0);

            yield return new WaitForSeconds(0.2f);
        }
        _currentCoroutine = null;
    }
}