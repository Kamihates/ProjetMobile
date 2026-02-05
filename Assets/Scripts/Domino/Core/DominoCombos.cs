using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DominoCombos : MonoBehaviour
{
    [SerializeField, Foldout("Temp")] private int damagePerCombo = 5;

    [SerializeField, Foldout("Debug"), ReadOnly] private int combosCount = 0;
    public int CombosCount => combosCount;

    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentDomino;

    public Action<int> OnComboDamage;

    private void SetCombosOfAdjacentDomino(List<Vector2Int> combos)
    {
        combosOfAdjacentDomino = combos;
        combosCount = combosOfAdjacentDomino.Count;
    }

    private void Start()
    {
        GridManager.Instance.OnDominoPlaced += CheckForCombos;
    }

    private void OnDestroy()
    {
        GridManager.Instance.OnDominoPlaced -= CheckForCombos;
    }

    public void CheckForCombos(DominoPiece piece)
    {
        RegionPiece regionPiece1 = piece.transform.GetChild(0).GetComponent<RegionPiece>();
        RegionPiece regionPiece2 = piece.transform.GetChild(1).GetComponent<RegionPiece>();

        if (regionPiece1.gameObject.activeSelf)
            CheckForAdjacentDomino(regionPiece1);

        if(regionPiece1.Region == null) return;

        if (!regionPiece2.gameObject.activeSelf) return;
        if (regionPiece2.Region == null) return;

        if(regionPiece1.Region.RegionID != regionPiece2.Region.RegionID ) 
            CheckForAdjacentDomino(regionPiece2);

        combosCount = combosOfAdjacentDomino.Count;

        if (combosCount < 2)
            return;

        int totalDamage = combosCount * damagePerCombo;
        OnComboDamage?.Invoke(totalDamage);
    }


    private void CheckForAdjacentDomino(RegionPiece regionPiece)
    {
        combosOfAdjacentDomino.Clear();

        // Donne l'index par rapport a la grille
        Vector2Int regionIndex = GridManager.Instance.GetIndexFromPosition(regionPiece.transform.position);


        List<Vector2Int> regionToCheck = new List<Vector2Int> { regionIndex };


        while(regionToCheck.Count> 0)
        {
            Vector2Int currentIndex = regionToCheck[0];
            regionToCheck.RemoveAt(0);

            RegionData regionData = GridManager.Instance.GetRegionAtIndex(currentIndex)?.Region;
            if (regionData == null)
                continue;

            if (!combosOfAdjacentDomino.Contains(currentIndex))
                combosOfAdjacentDomino.Add(currentIndex);

            Vector2Int[] regionNeighbors  = GetRegionNeighbors(currentIndex);

            foreach (Vector2Int neighbor in regionNeighbors)
            {
                if (!GridManager.Instance.CheckIndexValidation(neighbor))
                    continue;

                RegionData neighborRegion = GridManager.Instance.GetRegionAtIndex(neighbor)?.Region;

                if (neighborRegion == null)
                    continue;

                if (neighborRegion.Type == regionPiece.Region.Type)
                {
                    if (!combosOfAdjacentDomino.Contains(neighbor))
                        regionToCheck.Add(neighbor);
                }
            }
        }
    }

    private Vector2Int[] GetRegionNeighbors(Vector2Int regionIndex)
    {
        Vector2Int[] neighbors = new Vector2Int[]
        {
            new Vector2Int(0, 1) + regionIndex,   // Up
            new Vector2Int(1, 0) + regionIndex,   // Right
            new Vector2Int(0, -1) + regionIndex,  // Down
            new Vector2Int(-1, 0) + regionIndex  // Left
        };

        return neighbors;
    }
}
