using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DominoCombos : MonoBehaviour
{
    [SerializeField] private float damagePerCombo = 5;
    [SerializeField, Label("un t1 basique multiplie par combien ? (basique = 4)")] private float multipleT1 = 5;
    [SerializeField, Label("on ajoute combien au multiplicateur selon la force du t1 ? (4/6/8/9)")] private float gapDmgT1 = 1.5f;

    [SerializeField, Foldout("Debug"), ReadOnly] private int combosCount = 0;
    public int CombosCount => combosCount;

    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentDomino;

    public Action<float> OnComboDamage;

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
        if (GridManager.Instance != null) 
            GridManager.Instance.OnDominoPlaced -= CheckForCombos;
    }

    private int t1Multiplicateur = 1;
    public void CheckForCombos(DominoPiece piece)
    {
        t1Multiplicateur = 1;

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

        if (combosCount < 3)
            return;

        // degats totaux = (nb de regions adj * degat basique t0) * t1Multiplicateur
        float totalDamage = (combosCount * damagePerCombo) * t1Multiplicateur;
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
            {
                combosOfAdjacentDomino.Add(currentIndex);
                if (GridManager.Instance.GetRegionAtIndex(currentIndex).DominoParent.Data.IsDominoFusion)
                {
                    //float t1dmg = (multipleT1 * gapDmgT1 * );
                    //t1Multiplicateur += 
                }
            }

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
                    {
                        regionToCheck.Add(neighbor);

                    }
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
