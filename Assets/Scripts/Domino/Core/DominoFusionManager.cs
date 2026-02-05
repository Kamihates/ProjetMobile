using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

public class DominoFusion : MonoBehaviour
{
    private GridManager _gridManager;
    [SerializeField] private DeckManager _deckManager;

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
        bool conflitDetected = false;

        List<FusionSquare> fusionSquare1 = GetAllFusionSquare(piece, 0);
        List<FusionSquare> fusionSquare2 = GetAllFusionSquare(piece, 1);

        if (fusionSquare1 != null && fusionSquare2!= null)
        {
            // si ya des conflits 
            conflitDetected =
                fusionSquare1.SelectMany(fs => fs.Square).GroupBy(vector => vector).Any(index => index.Count() > 1)
                ||
                fusionSquare2.SelectMany(fs => fs.Square).GroupBy(vector => vector).Any(index => index.Count() > 1)
                ;
        }



        if (conflitDetected)
        {
            if (fusionSquare1.Count > 1 && fusionSquare2.Count > 1)
            {
                // recherche de squares independants
                foreach (FusionSquare fs in fusionSquare1)
                {
                    foreach(FusionSquare fs2 in fusionSquare2)
                    {
                        bool isValid = true;
                        foreach (Vector2Int index in fs.Square)
                        {
                            // FS1 ne va pas avec FS2
                            if (fs2.Square.Contains(index))
                            {
                                isValid = false;
                                break;
                            }
                        }

                        if (isValid)
                        {
                            // on renvoie les carrés fs1 et fs2 pour T1 x 2 
                            Debug.Log("fusion T1 x 2");

                        }
                        else
                        {
                            Debug.Log("Conflits inévitables");
                        }
                    }
                }


            }
            else
            {
                // conflit inévitables
                Debug.Log("Conflits inévitables");
            }
        }
        else
        {
            
            // pas de fusion
            if (fusionSquare1.Count == 0 && fusionSquare2.Count == 0)
            {
                Debug.Log("Pas de fusion");
            }
            // fusion T1
            else
            {
                Debug.Log("Fusion T1");

                if ((fusionSquare1.Count == 1 && fusionSquare2.Count == 1) || (fusionSquare1.Count == 1 && fusionSquare2.Count <= 0))
                {
                    _deckManager.PutT1InDeck(fusionSquare1[0].Square);
                }
                else
                {
                    _deckManager.PutT1InDeck(fusionSquare2[0].Square);
                }
   
            }
        }
    }

    private List<FusionSquare> GetAllFusionSquare(DominoPiece piece, int regionIndex)
    {
        if (piece.transform.GetChild(regionIndex).TryGetComponent(out RegionPiece region2))
        {
            // si la région à de la data
            if (region2.Region != null)
            {
                // on récupère les carrés de fusion
                List<FusionSquare> fusionSquares = GetFusionNeighbors(region2);

                fusionSquares.RemoveAll(square => !square.IsFusionDetected);

                return fusionSquares;
            }
        }

        return new List<FusionSquare>(); ;
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
        Vector2Int regionIndex = _gridManager.GetIndexFromPosition(regionPos);

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

        for (int i = 0; i < neighbors.Count; i++)
        {
            // on passe les index matrice en grille
            if (_gridManager.CheckIndexValidation(new Vector2Int(neighbors[i].y, (neighbors[i].x))))
                result[i] = neighbors[i];
        }

        return result;
    }
    private bool IsSquareIsSameData(Vector2Int[] square)
    {
        if (square == null) return false;

        //RegionData region = _gridManager.GetRegionAtIndex(new Vector2Int(square[0].y, square[0].x));

        RegionData region = _gridManager.GetRegionAtIndex(new Vector2Int(square[0].y, square[0].x));

        if (region == null)
        {
            return false;
        }

        int regionID = region.RegionID;

        foreach (Vector2Int index in square)
        {
            // l'id nest pas le meme, il n'y a pas de fusion dans ce carré
            if (_gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x)) == null)
                return false;

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
