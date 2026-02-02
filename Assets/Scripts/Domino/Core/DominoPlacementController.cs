using System;
using Unity.VisualScripting;
using UnityEngine;

public class DominoPlacementController : MonoBehaviour
{

    public Vector2 GetFinalDestination(DominoPiece domino, Vector2Int index)
    {
        // ETAPE 1 : On converti notre domino en index de la grille pour récupérer la colonne de notre domino
        if (index.x == -1)
        {
            index = GridManager.Instance.GetPositionToGridIndex(domino.transform.position);
            index.y = 0;
        }

        // ETAPE 2 : On verifie si la cellule est bonne
        if (GridManager.Instance.GetRegionAtIndex(index) != null)
        {
            // placement déjà occupé
            if (index.y == 0)
            {
                Debug.LogWarning("aucun emplacement trouvé");
                GridManager.Instance.OnDominoExceed?.Invoke();
                return domino.transform.position;
            }
            else
            {
                // l'emplacement est pris, on est pas à l'index 1 donc on renvoie la derniere position bonne
                index.y--;
                DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(index));
                return GridManager.Instance.GetCellPositionAtIndex(index);
            }
           
        }

        // la cellule est pas occupée, on verifie pour les regions

        Vector2 targetCellPos = GridManager.Instance.GetCellPositionAtIndex(index);

        // pour chaque region
        foreach (Transform child in domino.transform)
        {
            if (child.TryGetComponent(out RegionPiece region))
            {
                // si la region n'est pas vide
                if (region.Region != null)
                {
                    // on calcule sa position selon la tagetPos de notre cellule
                    Vector2 RegionPosSimulation = targetCellPos + (Vector2)region.transform.localPosition;

                    // on verifie qu'elle est dans la grille
                    if (!GridManager.Instance.IsRegionInGrid(RegionPosSimulation))
                    {
                        // la region dépasse en bas, on renvoie la derniere position trouvé bonne
                        index.y--;
                        DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(index));
                        return GridManager.Instance.GetCellPositionAtIndex(index);
                    }

                    // on passe la region en index pour verifier si les emplacements sont vides

                    Vector2Int RegionIndex = GridManager.Instance.GetPositionToGridIndex(RegionPosSimulation);

                    if (GridManager.Instance.GetRegionAtIndex(RegionIndex) != null)
                    {
                        if (index.y == 0)
                        {
                            Debug.LogWarning("aucun emplacement trouvé");
                            return domino.transform.position;
                        }
                        else
                        {
                            // l'emplacement est pris, on est pas à l'index 1 donc on renvoie la derniere position bonne
                            index.y--;
                            DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(index));
                            return GridManager.Instance.GetCellPositionAtIndex(index);
                        }
                    }
                }
            }

        }
        // tout est bon, on peut check la cellule du dessous

        
        index.y++;


        return GetFinalDestination(domino, index);
    }

    //public Vector2 GetFinalDestination(DominoPiece domino, Vector2Int index)
    //{
    //    // ETAPE 1 : On converti notre domino en index de la grille pour récupérer la colonne de notre domino
    //    if (index.x == -1 )
    //    {
    //        index = GridManager.Instance.GetPositionToGridIndex(domino.transform.position);
    //        index.y = GridManager.Instance.Row - 1;
    //    }

    //    // ETAPE 2 : On verifie si la cellule la plus basse est bonne
    //    if (GridManager.Instance.GetRegionAtIndex(index) != null )
    //    {
    //        // placement déjà occupé
    //        index.y--;

    //        if (index.y < 0 )
    //        {
    //            Debug.LogWarning("aucun emplacement trouvé");
    //            return domino.transform.position;
    //        }

    //        return GetFinalDestination(domino, index);
    //    }
    //    else
    //    {
    //        Vector2 targetCellPos = GridManager.Instance.GetCellPositionAtIndex(index);

    //        foreach (Transform child in domino.transform)
    //        {
    //            if (child.TryGetComponent(out RegionPiece region))
    //            {
    //                // si la region n'est pas vide
    //                if (region.Region != null)
    //                {
    //                    // on calcule sa position selon la tagetPos de notre cellule
    //                    Vector2 RegionPosSimulation = targetCellPos + (Vector2)region.transform.localPosition;

    //                    // on verifie qu'elle est dans la grille
    //                    if (!GridManager.Instance.IsRegionInGrid(RegionPosSimulation))
    //                    {
    //                        // la region dépasse en bas, faut remonter
    //                        index.y--;
    //                        return GetFinalDestination(domino, index);
    //                    }

    //                    // on la passe en index pour verifier si les emplacements sont vides

    //                    Vector2Int RegionIndex = GridManager.Instance.GetPositionToGridIndex(RegionPosSimulation);

    //                    if (GridManager.Instance.GetRegionAtIndex(RegionIndex) != null)
    //                    {
    //                        // l'emplacement est pris... il faut remonter
    //                        index.y--;
    //                        return GetFinalDestination(domino, index);
    //                    }
    //                }
    //            }
    //        }
    //        // placement libre

    //        DrawPrevisualisation(domino, GridManager.Instance.GetCellPositionAtIndex(index));


    //        return GridManager.Instance.GetCellPositionAtIndex(index);
    //    }
    //}



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
