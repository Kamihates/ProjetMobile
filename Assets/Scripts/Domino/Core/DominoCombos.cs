using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DominoCombos : MonoBehaviour
{
    [SerializeField] private float damagePerCombo = 5;
    public float DamagePerCombo => damagePerCombo;
    [SerializeField, Label("un t1 basique multiplie par combien ? (basique = 4)")] private float T1multipicator = 2;
    public float T1Multipicator => T1multipicator;
    [SerializeField, Label("on ajoute combien au multiplicateur selon la force du t1 ? (4/6/8/9)")] private float gapDmgT1 = 1.5f;

    [SerializeField, Foldout("Debug"), ReadOnly] private int combosCount = 0;
    [SerializeField, Foldout("Debug"), ReadOnly] private int t1Count = 0;

    [SerializeField] private DominoFusion dominoFusion;
    public int CombosCount => combosCount;

    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentDomino;
    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentR1;
    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentR2;

    public Action<float> OnComboDamage;
    public Action<List<RegionPiece>> OnComboChain;
    public Action<float, float> OnComboFinished;

    private void Start()
    {
        GridManager.Instance.OnDominoPlaced += CheckForReaction;
    }

    private void OnDestroy()
    {
        if (GridManager.Instance != null) 
            GridManager.Instance.OnDominoPlaced -= CheckForReaction;
    }

    public void CheckForReaction(DominoPiece piece)
    {
        // d'abord on check les combos.
        // si ya une ou des fusions, on calcule leurs bonus pour les ajouter aux degats

        float comboDamages = CheckForCombos(piece);
        float fusionBonusDamage = dominoFusion.CheckForFusion(piece);

        float totalDamage = comboDamages + fusionBonusDamage;

        OnComboDamage?.Invoke(totalDamage);
    }

    public float CheckForCombos(DominoPiece piece)
    {
        t1Count = 0;
        combosCount = 0;

        int combosOfAdjacentR1 = 0;
        int combosOfAdjacentR2 = 0;

        RegionPiece regionPiece1 = piece.transform.GetChild(0).GetComponent<RegionPiece>();
        RegionPiece regionPiece2 = piece.transform.GetChild(1).GetComponent<RegionPiece>();

        if (regionPiece1.gameObject.activeSelf && regionPiece1.Region != null)
            combosOfAdjacentR1 = CheckForAdjacentDomino(regionPiece1);

        if (regionPiece2.gameObject.activeSelf
            && regionPiece2.Region != null
            && (!regionPiece1.gameObject.activeSelf
                || regionPiece1.Region.RegionID != regionPiece2.Region.RegionID))
        {
            combosOfAdjacentR2 = CheckForAdjacentDomino(regionPiece2);
        }

        if (combosOfAdjacentR1 == 1) combosOfAdjacentR1 = 0;
        if (combosOfAdjacentR2 == 1) combosOfAdjacentR2 = 0;

        combosCount = combosOfAdjacentR1 + combosOfAdjacentR2;

        if (combosCount < 2)
        {
            OnComboFinished?.Invoke(0, 0);
            return 0;
        }

        float comboDamage = combosCount * damagePerCombo;
        float multiplicator = 1f;

        if(t1Count > 0)
        {
            if (t1Count/2 > 0)
            {
                multiplicator = (t1Count / 2) * T1Multipicator;
                Debug.Log(multiplicator);
            }
        }

        float totalDamage = comboDamage * multiplicator;
            
        OnComboChain?.Invoke(GetAllComboRegions());
        OnComboFinished?.Invoke(totalDamage, multiplicator);

        return totalDamage;
    }


    private int CheckForAdjacentDomino(RegionPiece regionPiece)
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
                    t1Count++;
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


        return (combosOfAdjacentDomino.Count);
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

    private List<RegionPiece> GetAllComboRegions()
    {
        List<RegionPiece> regions = new();

        // On parcourt tous les index des régions qui font partie du combos et on récupère les RegionPiece correspondantes pour les envoyer à l'affichage du combo
        foreach (Vector2Int index in combosOfAdjacentDomino)
        {
            RegionPiece region = GridManager.Instance.GetRegionAtIndex(index);
            if (region != null && !regions.Contains(region))
                regions.Add(region); // On vérifie que la région n'est pas null et qu'elle n'est pas déjà dans la liste avant de l'ajouter
        }

        return regions; // On return la liste de toutes les régions qui font partie du combo
    }
}
