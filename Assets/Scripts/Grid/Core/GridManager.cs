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

    public int Column { get => _column; }
    public Transform Origin { get => _gridOrigin; }
    public int Row { get => _row; }
    public float CellSize { get => _cellSize; }
    public float TileDepth { get => _tileDepth; }


    public static GridManager Instance;
    private void Awake() { Instance = this; }

    public bool IsInGrid(List<Vector2> positions)
    {
        foreach (Vector2 position in positions) 
        {
            // limites de la grille
            float bottomMax = (_gridOrigin.position.y + _cellSize / 2) - (_row * _cellSize);
            float LeftMax = (_gridOrigin.position.x - _cellSize / 2);
            float RightMax = (_gridOrigin.position.x - _cellSize / 2) + (_column * _cellSize);

            if (position.x > RightMax) return false;
            if (position.x < LeftMax) return false;
            if (position.y < bottomMax) return false;
        }
        
        return true;
    }

    private void OnDrawGizmos()
    {
        gridDrawer.DrawGrid(_row, _column, _cellSize, _gridOrigin);
    }

}
