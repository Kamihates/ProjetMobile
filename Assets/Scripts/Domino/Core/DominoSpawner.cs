using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DominoSpawner : MonoBehaviour
{

    [Header("Spawn"), SerializeField] private DominoMovementController dominoController;

    [Header("Base Data")]
    [SerializeField] private CombinaisonData dominoData; 


    [SerializeField] private int _currentDominoId;
    public int CurrentDominoId => _currentDominoId;


    public Action<DominoPiece> OnDominoSpawn; 

    private void Awake()
    {
        OnDominoSpawn += SpawnDomino;
    }

    private void OnDestroy()
    {
        OnDominoSpawn -= SpawnDomino;
    }


    // temp !! 

    int _currentPieceID = 0;

    // temps !!

    public void SpawnDomino(DominoPiece domino)
    {

        // on récupere le milieu de la case du milieu de ma grille
        Vector2 spawnPos = GetSpawnPosition();
        int rotation = 0;


        if (TEST_GD.Instance !=  null)
        {
            if (TEST_GD.Instance.RotationRandom)
            {
                // _currentRotation aléatoire
                rotation = UnityEngine.Random.Range(0, 4);
            }
            
        }
        

        //// création 

        //GameObject dominoGO = Instantiate(dominoPrefab.gameObject, spawnPos, Quaternion.identity);

        //DominoPiece dominoInstance = dominoGO.GetComponent<DominoPiece>();

        //dominoInstance.Init(_currentPieceID++, rotation, dominoCombination);  

        //_currentDominoId = dominoInstance.PieceUniqueId;
        
        domino.transform.position = spawnPos;

        dominoController.CurrentDomino = domino;
        dominoController.CanFall = true;
    }

    private Vector2 GetSpawnPosition()
    {
        if (GridManager.Instance == null) return Vector2.zero;

        // on récupère la cellule du milieu 
        int cell = Mathf.CeilToInt(GridManager.Instance.Column / 2);

        // on recupere sa position
        Vector2 spawnPos = new Vector2(GridManager.Instance.Origin.position.x + ((cell - 1) * GridManager.Instance.CellSize), GridManager.Instance.Origin.position.y);
        return spawnPos;
    }
    
}
