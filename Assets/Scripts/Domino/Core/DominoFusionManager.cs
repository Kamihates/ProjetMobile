using GooglePlayGames;
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

    [SerializeField] private float _bonusFor4;
    [SerializeField] private float _bonusFor6;
    [SerializeField] private float _bonusFor8;
    [SerializeField] private float _bonusFor9;
    [SerializeField] private float _bonusBasique;

    private int _fusionCount = 0;
    public int FusionCount => _fusionCount;

    private void Start()
    {
        _gridManager = GridManager.Instance;
        if (GameManager.Instance != null )
            GameManager.Instance.OnInfiniteGameStarted += resetCounters;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnInfiniteGameStarted -= resetCounters;
    }

    private void resetCounters()
    {
        _fusionCount = 0;
    }

    public float CheckForFusion(DominoPiece piece)
    {
        List<FusionSquare> fusionSquare1 = GetAllFusionSquare(piece, 0);
        List<FusionSquare> fusionSquare2 = GetAllFusionSquare(piece, 1);

        List<Vector2Int> AllFusionIndex = new List<Vector2Int>();

        foreach (FusionSquare square in fusionSquare1)
        {
            foreach (Vector2Int index in square.Square)
            {
                if (!AllFusionIndex.Contains(index))
                    AllFusionIndex.Add(index);
            }
        }
        foreach (FusionSquare square in fusionSquare2)
        {
            foreach (Vector2Int index in square.Square)
            {
                if (!AllFusionIndex.Contains(index))
                    AllFusionIndex.Add(index);
            }
        }

        if (AllFusionIndex.Count > 0)
        {
            _fusionCount++;
            if (_fusionCount == 1)
            {
                // succes sur la premiere fusion "First ritual"
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_first_ritual, 100.0f, (bool success) =>
                {
                    if (success)
                        Debug.Log("Succès débloqué !");
                    else
                        Debug.Log("Échec du déblocage du succès.");
                });
            }

            if (_fusionCount >= 8 && !GameManager.Instance.IsInfiniteState)
            {
                // succes sur 8 fusions "Ritual Master"
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_ritual_master, 100.0f, (bool success) =>
                {
                    if (success)
                        Debug.Log("Succès débloqué !");
                    else
                        Debug.Log("Échec du déblocage du succès.");
                });
            }

            if (AllFusionIndex.Count >= 4)
            {
                ManageFusion(AllFusionIndex);
                return GetBonus(AllFusionIndex.Count);
            }
            
        }
        return 0;
    }

    private float GetBonus(int count)
    {
        switch (count)
        {
            case 4:
                return _bonusFor4;
            case 6:
                return _bonusFor6;
            case 8:
                return _bonusFor8;
            case 9:
                // succes "perfect ritual"
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_perfect_ritual, 100.0f, (bool success) =>
                {
                    if (success)
                        Debug.Log("Succès débloqué !");
                    else
                        Debug.Log("Échec du déblocage du succès.");
                });
                return _bonusFor9;
            default:
                return _bonusBasique;
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

        RegionData region = _gridManager.GetRegionAtIndex(new Vector2Int(square[0].y, square[0].x)).Region;

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

            if (_gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x)).Region.RegionID != regionID)
                return false;

            if (_gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x)).IsT1)
                return false;

        }

        return true;
    }

    private void ManageFusion(List<Vector2Int> AllIndex)
    {
        // quand on a une fusion : 
        // ETAPE 1 : on récupère le domino, et supprime sa region correspondante
        List<RegionPiece> allPieces = new List<RegionPiece>();

        _deckManager.PutT1InDeck(AllIndex);
        RegionType type = RegionType.None;

        foreach (Vector2Int index in AllIndex)
        {
            RegionPiece regionPiece = _gridManager.GetRegionAtIndex(new Vector2Int(index.y, index.x));
            type = regionPiece.Region.Type;
            allPieces.Add(regionPiece);

            // on supprime la region de la liste du domino
            //domino.Data.Regions.Remove(regionPiece.Region);

            // on supprime la region (go)
            //Destroy(regionPiece.gameObject);
            regionPiece.Region = null;
            regionPiece.gameObject.SetActive(false);

            // on les supprime de la grille
            _gridManager.GridData[index.x][index.y] = null;
        }

        // on appelle la particule
        FusionVisualEffects.Instance.PlayFusionParticule(allPieces, type);

        // ETAPE 2 : On fait tomber tt les dominos qui le peuvent de la grille de bas en haut
        _gridManager.AllDominoFall();

       
    }
}


public class FusionSquare
{
    private bool _isFusionDetected = false;
    private Vector2Int[] _square = new Vector2Int[4];

    public bool IsFusionDetected { get => _isFusionDetected; set => _isFusionDetected = value; }
    public Vector2Int[] Square {get => _square; set=> _square = value;}
       
    
}
