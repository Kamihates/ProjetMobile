using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class GridManager : MonoBehaviour
{
    [SerializeField] private GridDrawer gridDrawer;

    [SerializeField]
    private List<List<RegionPiece>> _gridData = new();



    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Infos de la grille"), Label("Nombre de colonnes")][SerializeField] private int _column;
    [BoxGroup("Infos de la grille"), Label("Nombre de Lignes")][SerializeField] private int _row;
    [BoxGroup("Infos de la grille"), Label("Origine de la grille")][SerializeField] private Transform _gridOrigin;
    [BoxGroup("Infos de la grille"), Label("Taille d'une cellule")][SerializeField] private float _cellSize;
    [BoxGroup("Infos de la grille"), Label("taille de la profondeur d'une cellule")][SerializeField] private float _tileDepth;


    

    public Action<DominoPiece> OnDominoPlaced;
   

   
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
        gridDrawer.DrawGrid(_row, Column, _cellSize, _gridOrigin);
        ResetGridData();
    }

    private void ResetGridData()
    {
        _gridData.Clear();

        for (int row = 0; row < _row; row++)
        {
            _gridData.Add(new List<RegionPiece>());

            for (int col = 0; col < _column; col++)
            {
                _gridData[row].Add(null);
            }
        }
    }

    public bool IsDominoInGrid(DominoPiece domino, bool ignoreTop)
    {
        if (domino.transform.GetChild(0).gameObject.activeSelf)
        {
            if (!IsRegionInGrid(domino.transform.GetChild(0).position, ignoreTop))
                return false;
        }
        if (domino.transform.GetChild(1).gameObject.activeSelf)
        {
            if (!IsRegionInGrid(domino.transform.GetChild(1).position, ignoreTop))
                return false;
        }
        return true;
    }

    public bool IsRegionInGrid(Vector2 pos, bool ignoreTop = true)
    {
        float half = _cellSize / 2f;

        Vector2 TL = new Vector2(pos.x - half, pos.y + half);
        Vector2 TR = new Vector2(pos.x + half, pos.y + half);
        Vector2 BL = new Vector2(pos.x - half, pos.y - half);
        Vector2 BR = new Vector2(pos.x + half, pos.y - half);

        return IsInGrid(new List<Vector2> { TL, TR, BL, BR }, ignoreTop);
    }

    public bool IsInGrid(List<Vector2> positions, bool ignoreTop = true)
    {
        // limites de la grille
        float bottomMax = (_gridOrigin.position.y + _cellSize / 2) - (_row * _cellSize);
        float LeftMax = (_gridOrigin.position.x - _cellSize / 2);
        float RightMax = (_gridOrigin.position.x - _cellSize / 2) + (_column * _cellSize);
        float TopMax = (_gridOrigin.position.y + _cellSize / 2);

        float gap = 0.2f;

        foreach (Vector2 p in positions)
        {
            if (p.x > RightMax + gap) return false;
            if (p.x < LeftMax - gap) return false;
            if (p.y < bottomMax - gap) return false;
            if (!ignoreTop)
                if (p.y > TopMax + gap) return false;
        }

        return true;
    }

    public void AddDominoDataInGrid(DominoPiece domino)
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

                    Vector2Int RegionIndex = GetIndexFromPosition(RegionPosSimulation);

                    _gridData[RegionIndex.y][RegionIndex.x] = region;

                    if (RegionIndex.y == 0)
                    {
                        Debug.Log("Arrivé en haut de la grille");
                    }
                }
            }
        }

        OnDominoPlaced?.Invoke(domino);
    }

    /// <summary>
    /// renvoie l'index sur la grille (abs, ord)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector2Int GetIndexFromPosition(Vector2 position)
    {
        Vector2Int index = Vector2Int.zero;

        // 1) on met notre position en local à la grille
        Vector2 localPos = _gridOrigin.InverseTransformPoint(position);

        // 2) on calcule les index
        index.x = Mathf.Abs(Mathf.RoundToInt(localPos.x / _cellSize)); // sur la matrice c'est [][ici]
        index.y = Mathf.Abs(Mathf.RoundToInt(localPos.y / _cellSize)); // sur la matrice c'est [ici][]

        return index;

    }

    public Vector2 GetCellPositionAtIndex(Vector2Int index)
    {
        // index.x correspond à la colonne
        // index.y correspond à la ligne

        return new Vector2(_gridOrigin.position.x + _cellSize * index.x, _gridOrigin.position.y - _cellSize * index.y) ;
    }

    /// <summary>
    /// renvoie la region à l'index d'abscisse x et ordonnée y
    /// </summary>
    /// <param name="index"> index sur la grille monde</param>
    /// <returns></returns>
    public RegionPiece GetRegionAtIndex(Vector2Int index)
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
            return _gridData[index.y][index.x];
    }

    private void OnDrawGizmos()
    {
        //gridDrawer.DrawGrid(_row, _column, _cellSize, _gridOrigin);
    }

    /// <summary>
    /// Check si les index matrices sont bons 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckIndexValidation(Vector2Int index)
    {
        if (index.x < 0) return false;

        if (index.y < 0) return false;
        
        if (index.y >= _gridData.Count) return false;

        if (index.x >= _gridData[index.y].Count) return false;


        return true;
        // return index.x >= 0 && index.x < _gridData.Count && index.y < _gridData[index.x].Count && index.y >= 0;
    }

    Coroutine _currentCoroutine = null;
    public void AllDominoFall()
    {
        // 1) on recupere tt les dominos de la grille
        List<DominoPiece> dominosToFall = new List<DominoPiece>();

        for (int i = _row - 1; i >= 0; i--)
        {
            for (int j = 0; j < _column; j++)
            {
                RegionPiece region = _gridData[i][j];
                if (region != null)
                {
                    _gridData[i][j] = null;
                    DominoPiece domino = region.DominoParent;
                    if (!dominosToFall.Contains(domino))
                        dominosToFall.Add(domino);
                }
            }
        }

        

        // 2) on fait tomber 1 par 1


        if (_currentCoroutine == null)
            _currentCoroutine = StartCoroutine(WaitToFall(dominosToFall));
    }

    private IEnumerator WaitToFall(List<DominoPiece> dominosToFall)
    {
        // on parcours tt les dominos de bas gauche en haut droite et les fait tomber avec un gap de X secondes
        foreach (DominoPiece domino in dominosToFall)
        {
            DominoFall fallController = domino.FallController;
            fallController.Init(4, 0.1f);
            fallController.IgnoreCurrentDomino = true;
            fallController.enabled = true;

            yield return new WaitForSeconds(0.1f);
        }

        
        _currentCoroutine = null;
    }
}
