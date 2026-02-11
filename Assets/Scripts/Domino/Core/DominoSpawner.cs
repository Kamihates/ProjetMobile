using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using System.Collections;

public class DominoSpawner : MonoBehaviour
{

    [Header("Spawn"), SerializeField] private DominoMovementController dominoController;

    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Spawn")]
    [Header("Activer la rotation aléatoire")]
    [SerializeField, Label("cocher pour activer la rotation aléatoire")] private bool _rotationRandom;
    [BoxGroup("Spawn"),SerializeField, Label("temps avant la descente")] private float _waitingTime = 1f;

    [Header("Base Data")]
    [SerializeField] private CombinaisonData dominoData; 
    [SerializeField] private DeckManager deckManager; 


    private DominoPiece _currentDomino;
    public DominoPiece CurrentDomino => _currentDomino;


    public Action OnDominoSpawn; 

    private void Awake()
    {
        OnDominoSpawn += SpawnNextDomino;
    }

    private void OnDestroy()
    {
        OnDominoSpawn -= SpawnNextDomino;
    }

    Coroutine _currentCoroutine = null;
    public void SpawnNextDomino()
    {

        DominoPiece dominoToSpawn = deckManager.GetNextDominoInHand();
        
        if (dominoToSpawn != null)
        {
            deckManager.DominoInHand.RemoveAt(0);
            deckManager.HandVisual.DominoInHandVisual[0].transform.SetParent(null);
            deckManager.HandVisual.DominoInHandVisual.RemoveAt(0);
            deckManager.HandVisual.UpdateDominoHandVisual();

        }


        // on r�cupere le milieu de la case du milieu de ma grille
        Vector2 spawnPos = GetSpawnPosition();
        int rotation = 0;

        if (_rotationRandom)
        {
            // _currentRotation al�atoire
            rotation = UnityEngine.Random.Range(0, 4);
        }

        

       // dominoToSpawn.transform.position = spawnPos;
        GameManager.Instance.CurrentDomino = dominoToSpawn;
        

        GeneralVisualController.Instance.FallAtoB(dominoToSpawn.transform, 0.5f, dominoToSpawn.transform.position, spawnPos);

        if (_currentCoroutine ==  null)
            _currentCoroutine = StartCoroutine(WaitBeforeFalling(dominoToSpawn));

    }

    IEnumerator WaitBeforeFalling(DominoPiece dominoToSpawn)
    {
        yield return new WaitForSeconds(_waitingTime);
        dominoToSpawn.FallController.CanFall = true;
        _currentCoroutine = null;
    }


    private Vector2 GetSpawnPosition()
    {
        if (GridManager.Instance == null) return Vector2.zero;

        // on r�cup�re la cellule du milieu 
        int cell = Mathf.CeilToInt(GridManager.Instance.Column / 2);
        //int cell = GridManager.Instance.Column - 1;

        // on recupere sa position
        Vector2 spawnPos = new Vector2(GridManager.Instance.Origin.position.x + ((cell - 1) * GridManager.Instance.CellSize), GridManager.Instance.Origin.position.y + (GridManager.Instance.CellSize * 2f));
        return spawnPos;
    }
    
}
