using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DominoSpawner : MonoBehaviour
{
    [Header("Prefab"), SerializeField] private GameObject dominoPrefab;
    [Header("Spawn"), SerializeField] private Transform spawnPoint;

    [Header("Base Data")]
    [SerializeField] private DominoData dominoData; 


    [SerializeField] private int _currentDominoId;
    public int CurrentDominoId => _currentDominoId;

    private Action<List<RegionData>> OnDominoSpawn; 

    private void Start()
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

    public void SpawnDomino(List<RegionData> dominoCombination)
    {

        // rotation aléatoire
        int rotation = UnityEngine.Random.Range(0, 4);


        // création 

        GameObject dominoGO = Instantiate(dominoPrefab.gameObject, spawnPoint.position, Quaternion.identity);

        DominoPiece dominoInstance = dominoGO.GetComponent<DominoPiece>();

        dominoInstance.Init(_currentPieceID++, rotation, dominoCombination);  

        _currentDominoId = dominoInstance.PieceUniqueId;
    }
    
}
