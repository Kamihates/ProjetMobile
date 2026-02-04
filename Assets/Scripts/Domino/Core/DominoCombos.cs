using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DominoCombos : MonoBehaviour
{
    [SerializeField, Foldout("Debug"), ReadOnly] private int combosCount = 0;
    public int CombosCount => combosCount;

    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentDomino;

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
        CheckForAdjacentDomino(regionPiece1);
        CheckForAdjacentDomino(regionPiece2);
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

            RegionData regionData = GridManager.Instance.GetRegionAtIndex(currentIndex);
            if (regionData == null)
                continue;

            if (!combosOfAdjacentDomino.Contains(currentIndex))
                combosOfAdjacentDomino.Add(currentIndex);

            Vector2Int[] regionNeighbors  = GetRegionNeighbors(currentIndex);

            foreach (Vector2Int neighbor in regionNeighbors)
            {
                if (!GridManager.Instance.CheckIndexValidation(neighbor))
                    continue;

                RegionData neighborRegion = GridManager.Instance.GetRegionAtIndex(neighbor);

                if (neighborRegion == null)
                    continue;

                if (neighborRegion.RegionID == regionPiece.Region.RegionID)
                {
                    if (!combosOfAdjacentDomino.Contains(neighbor))
                        regionToCheck.Add(neighbor);
                }

            }

            
        }

        Debug.Log($"Combos found: {combosOfAdjacentDomino.Count}");
        Debug.Log("----------- FUSION index ------- ");
        foreach (Vector2 Int in combosOfAdjacentDomino)
        {
            Debug.Log(Int);
        }
        Debug.Log("---------------- ");
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
