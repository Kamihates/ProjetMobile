using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DominoFusion : MonoBehaviour
{
    private GridManager _gridManager;


    private void Start()
    {
        _gridManager = GridManager.Instance;
        _gridManager.OnDominoPlaced += CheckForFusion;
    }

    private void OnDestroy()
    {
        _gridManager.OnDominoPlaced -= CheckForFusion;
    }


    private void CheckForFusion(DominoPiece piece)
    {
        // Pour chaque région du domino
        for (int i = 0; i < piece.transform.childCount; i++)
        {
            if (piece.transform.GetChild(i).TryGetComponent(out RegionPiece region))
            {
                // si la région à de la data
                if (region.Region != null)
                {
                    // on récupère les carrés de fusion
                    List<FusionSquare> fusionSquares = GetFusionNeighbors(region);

                    foreach (FusionSquare square in fusionSquares)
                    {
                        if (square.IsFusionDetected)
                        {
                            Debug.Log("FUSION DETECTEE");
                        }
                    }

                }
            }
        }
    }

    /// <summary>
    /// Calcule et renvoie un tableau des 4 carrées fusion d'une region
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    private List<FusionSquare> GetFusionNeighbors(RegionPiece region)
    {
        // On transforme notre region en index pour recup les index adjacents
        Vector2 regionPos = region.transform.position;
        Vector2Int regionIndex = _gridManager.GetPositionToGridIndex(regionPos);

        Vector2Int regionMatriceIndex = new Vector2Int(regionIndex.y, regionIndex.x); // converti index grille en index matrice

        // carré TL 
        FusionSquare fusionSquare1 = new();

        Vector2Int TL1 = new Vector2Int(regionMatriceIndex.x - 1, regionMatriceIndex.y - 1);
        Vector2Int TR1 = new Vector2Int(regionMatriceIndex.x - 1, regionMatriceIndex.y);
        Vector2Int BL1 = new Vector2Int(regionMatriceIndex.x, regionMatriceIndex.y - 1);

        fusionSquare1.Square = GetFusionSquareIndex(new List<Vector2Int> { regionMatriceIndex, TL1, TR1, BL1 });
        fusionSquare1.IsFusionDetected = IsSquareIsSameData(fusionSquare1.Square);

        // carré TR
        FusionSquare fusionSquare2 = new();

        Vector2Int TL2 = new Vector2Int(regionMatriceIndex.x - 1, regionMatriceIndex.y);
        Vector2Int TR2 = new Vector2Int(regionMatriceIndex.x - 1, regionMatriceIndex.y + 1);
        Vector2Int BR2 = new Vector2Int(regionMatriceIndex.x, regionMatriceIndex.y + 1);

        fusionSquare2.Square = GetFusionSquareIndex(new List<Vector2Int> { regionMatriceIndex, TL2, TR2, BR2 });
        fusionSquare2.IsFusionDetected = IsSquareIsSameData(fusionSquare2.Square);

        // Index en matrice pas en grille

        // carré BL
        FusionSquare fusionSquare3 = new();

        Vector2Int TL3 = new Vector2Int(regionMatriceIndex.x, regionMatriceIndex.y - 1);
        Vector2Int BL3 = new Vector2Int(regionMatriceIndex.x + 1, regionMatriceIndex.y - 1);
        Vector2Int BR3 = new Vector2Int(regionMatriceIndex.x + 1, regionMatriceIndex.y);

        fusionSquare3.Square = GetFusionSquareIndex(new List<Vector2Int> { regionMatriceIndex, TL3, BL3, BR3 });
        fusionSquare3.IsFusionDetected = IsSquareIsSameData(fusionSquare3.Square);

        // carré BR
        FusionSquare fusionSquare4 = new();

        Vector2Int TR4 = new Vector2Int(regionMatriceIndex.x, regionMatriceIndex.y + 1);
        Vector2Int BL4 = new Vector2Int(regionMatriceIndex.x + 1, regionMatriceIndex.y);
        Vector2Int BR4 = new Vector2Int(regionMatriceIndex.x + 1, regionMatriceIndex.y + 1);

        fusionSquare4.Square = GetFusionSquareIndex(new List<Vector2Int> { regionMatriceIndex, TR4, BL4, BR4 });
        fusionSquare4.IsFusionDetected = IsSquareIsSameData(fusionSquare4.Square);

       return new List<FusionSquare> { fusionSquare1 , fusionSquare2, fusionSquare3, fusionSquare4 };

    }
    private Vector2Int[] GetFusionSquareIndex(List<Vector2Int> neighbors)
    {
        Vector2Int[] result = new Vector2Int[neighbors.Count];

        for (int i =0; i < neighbors.Count; i++)
        {
            // on passe les index matrice en grille
            if (_gridManager.CheckIndexValidation(neighbors[i]))
                result[i] = neighbors[i];
        }

        return result;
    }
    private bool IsSquareIsSameData(Vector2Int[] square)
    {
        RegionData region = _gridManager.GetRegionAtIndex(new Vector2Int(square[0].y, square[0].x));

        if (region == null)
        {
            Debug.LogWarning("pas de region Data a cet index");
            return false;
        }

        int regionID = region.RegionID;

        foreach (Vector2Int index in square)
        {
            


            // l'id nest pas le meme, il n'y a pas de fusion dans ce carré
            if (_gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x)) == null)
                return false;
            Debug.Log("------ square[" + index + "] = " + _gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x)).RegionID);
            if (_gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x)).RegionID != regionID)
                return false;
        }

        return true;
    }
}


public class FusionSquare
{
    private bool _isFusionDetected = false;
    private Vector2Int[] _square = new Vector2Int[4];

    public bool IsFusionDetected { get => _isFusionDetected; set => _isFusionDetected = value; }
    public Vector2Int[] Square {get => _square; set=> _square = value;}
       
    
}
