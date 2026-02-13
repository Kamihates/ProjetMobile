using System;
using UnityEngine;

public class DominoPlacementController : MonoBehaviour
{
    public static DominoPlacementController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public Vector2 GetDestination(DominoPiece domino)
    {
        Transform t0 = domino.transform.GetChild(0);
        Transform t1 = domino.transform.GetChild(1);

        // 1) Déterminer le pivot actif
        Transform pivot = t0.gameObject.activeSelf ? t0 : t1;
        bool hasTwoRegions = t0.gameObject.activeSelf && t1.gameObject.activeSelf;

        // 2) Index du pivot
        Vector2Int pivotIndex = GridManager.Instance.GetIndexFromPosition(pivot.position);

        while (true)
        {
            // Si on dépasse la grille
            if (pivotIndex.y >= GridManager.Instance.Row)
            {
                pivotIndex.y--;
                DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(pivotIndex));
                return GridManager.Instance.GetCellPositionAtIndex(pivotIndex);
            }

            // Si la case pivot est occupée
            if (GridManager.Instance.GetRegionAtIndex(pivotIndex) != null || GridManager.Instance.DisableCells.ContainsKey(pivotIndex))
            {
                pivotIndex.y--;
                DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(pivotIndex));
                return GridManager.Instance.GetCellPositionAtIndex(pivotIndex);
            }


            // 3) Vérifier la région 2 si elle existe
            if (hasTwoRegions)
            {
                Vector2 pivotWorldPos = GridManager.Instance.GetCellPositionAtIndex(pivotIndex);
                Vector2 region2WorldPos = pivotWorldPos + (Vector2)t1.localPosition;
                Vector2Int region2Index = GridManager.Instance.GetIndexFromPosition(region2WorldPos);

                // Si la région 2 dépasse la grille
                if (region2Index.y >= GridManager.Instance.Row)
                {
                    pivotIndex.y--;
                    DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(pivotIndex));
                    return GridManager.Instance.GetCellPositionAtIndex(pivotIndex);
                }

                // Si la région 2 est occupée
                if (GridManager.Instance.GetRegionAtIndex(region2Index) != null || GridManager.Instance.DisableCells.ContainsKey(region2Index))
                {
                    pivotIndex.y--;
                    DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(pivotIndex));
                    return GridManager.Instance.GetCellPositionAtIndex(pivotIndex);
                }
            }

            // Sinon on descend
            pivotIndex.y++;
        }
    }


    private void DrawPrevisualisation(DominoPiece domino, Vector2 targetPos)
    {
        foreach (Transform child in domino.transform)
        {
            if (child.TryGetComponent(out RegionPiece region))
            {
                // si la region n'est pas vide
                if (region.Region != null)
                {
                    Vector2 RegionPosSimulation = targetPos + (Vector2)region.transform.localPosition;

                    Vector2 TL = new Vector2(RegionPosSimulation.x - GridManager.Instance.CellSize / 2, RegionPosSimulation.y + GridManager.Instance.CellSize / 2);
                    Vector2 TR = new Vector2(RegionPosSimulation.x + GridManager.Instance.CellSize / 2, RegionPosSimulation.y + GridManager.Instance.CellSize / 2);
                    Vector2 BL = new Vector2(RegionPosSimulation.x - GridManager.Instance.CellSize / 2, RegionPosSimulation.y - GridManager.Instance.CellSize / 2);
                    Vector2 BR = new Vector2(RegionPosSimulation.x + GridManager.Instance.CellSize / 2, RegionPosSimulation.y - GridManager.Instance.CellSize / 2);

                    Debug.DrawLine(TL, TR, Color.red);
                    Debug.DrawLine(TR, BR, Color.red);
                    Debug.DrawLine(BR, BL, Color.red);
                    Debug.DrawLine(BL, TL, Color.red);
                }
            }
        }
    }
}
