using System;
using System.Collections.Generic;
using UnityEngine;
public class GridManager : MonoBehaviour
{
    [Header("Nombre de colonnes")][SerializeField] private int _column;
    [Header("Nombre de Lignes")][SerializeField] private int _row;
    [Header("Origine de la grille")][SerializeField] private Transform _gridOrigin;
    [Header("Taille d'une cellule")][SerializeField] private float _cellSize;
    [Header("taille de la profondeur d'une cellule")][SerializeField] private float _tileDepth;


    [Header("Grid Drawer")][SerializeField] private GridDrawer gridDrawer;

    [SerializeField]
    private List<List<RegionPiece>> _gridData = new();

    public Action<DominoPiece> OnDominoPlaced;
    public Action OnDominoExceed; // action quand un domino est placé en haut de la grille

   
    public int Column { get => _column; }
    public Transform Origin { get => _gridOrigin; }
    public int Row { get => _row; }
    public float CellSize { get => _cellSize; }
    public float TileDepth { get => _tileDepth; }

    
    public List<List<RegionPiece>> GridData { get => _gridData; }


    public static GridManager Instance;
    private void Awake() { Instance = this; }

    private void Start()
    {
        OnDominoPlaced += AddDominoDataInGrid;

        for (int row = 0; row < _row; row++)
        {
            _gridData.Add(new List<RegionPiece>());

            for (int col = 0; col < _column; col++)
            {
                _gridData[row].Add(null);
            }
        }
    }

    private void OnDestroy()
    {
        OnDominoPlaced -= AddDominoDataInGrid;
    }

    public bool IsRegionInGrid(Vector2 pos)
    {
        float half = _cellSize / 2f;

        Vector2 TL = new Vector2(pos.x - half, pos.y + half);
        Vector2 TR = new Vector2(pos.x + half, pos.y + half);
        Vector2 BL = new Vector2(pos.x - half, pos.y - half);
        Vector2 BR = new Vector2(pos.x + half, pos.y - half);

        return IsInGrid(new List<Vector2> { TL, TR, BL, BR });
    }

    public bool IsInGrid(List<Vector2> positions)
    {
        // limites de la grille
        float bottomMax = (_gridOrigin.position.y + _cellSize / 2) - (_row * _cellSize);
        float LeftMax = (_gridOrigin.position.x - _cellSize / 2);
        float RightMax = (_gridOrigin.position.x - _cellSize / 2) + (_column * _cellSize);

        
        foreach (Vector2 p in positions)
        {
            if (p.x > RightMax) return false;
            if (p.x < LeftMax) return false;
            if (p.y < bottomMax) return false;
        }

        return true;
    }

    private void AddDominoDataInGrid(DominoPiece domino)
    {
        foreach (Transform child in domino.transform)
        {
            if (child.TryGetComponent(out RegionPiece region))
            {
                // si la region n'est pas vide
                if (region.Region != null)
                {
                    // on calcule sa position selon la tagetPos de notre cellule
                    Vector2 RegionPosSimulation = (Vector2)domino.transform.position + (Vector2)region.transform.localPosition;

                    // on la passe en index pour verifier si les emplacements sont vides

                    Vector2Int RegionIndex = GridManager.Instance.GetPositionToGridIndex(RegionPosSimulation);

                    _gridData[RegionIndex.y][RegionIndex.x] = region;

                    if (RegionIndex.y == 0)
                    {
                        Debug.Log("Arrivé en haut de la grille");
                    }
                }
            }
        }
    }


    public Vector2Int GetPositionToGridIndex(Vector2 position)
    {
        Vector2Int index = Vector2Int.zero;

        // 1) on met notre position en local à la grille
        Vector2 localPos = _gridOrigin.InverseTransformPoint(position);

        // 2) on calcule les index
        index.x = (int)Mathf.Abs(localPos.x / _cellSize); // sur la matrice c'est [][ici]
        index.y = (int)Mathf.Abs(localPos.y / _cellSize); // sur la matrice c'est [ici][]

        return index;

    }

    public Vector2 GetCellPositionAtIndex(Vector2Int index)
    {
        // index.x correspond à la colonne
        // index.y correspond à la ligne

        return new Vector2(_gridOrigin.position.x + _cellSize * index.x, _gridOrigin.position.y - _cellSize * index.y) ;
    }

    public RegionData GetRegionAtIndex(Vector2Int index)
    {
        if (index.y >= _gridData.Count)
        {
            return null;
        }
        if (index.x >= _gridData[index.y].Count)
        {
            return null;
        }

        if (_gridData[index.y][index.x] == null)
            return null;
        else 
            return _gridData[index.y][index.x].Region;
    }

    private void OnDrawGizmos()
    {
        gridDrawer.DrawGrid(_row, _column, _cellSize, _gridOrigin);
    }


    public bool CheckIndexValidation(Vector2Int index)
    {
        return index.x < _gridData.Count && index.y < _gridData[index.x].Count;
    }
}
