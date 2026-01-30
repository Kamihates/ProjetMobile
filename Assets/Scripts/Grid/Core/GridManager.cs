using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Nombre de colonnes")][SerializeField] private int _column;
    [Header("Nombre de Lignes")][SerializeField] private int _row;
    [Header("Origine de la grille")][SerializeField] private Transform _gridOrigin;
    [Header("Taille d'une cellule")][SerializeField] private float _cellSize;


    [Header("Grid Drawer")][SerializeField] private GridDrawer gridDrawer;

    public int Column { get => _column; }
    public Transform Origin { get => _gridOrigin; }
    public int Row { get => _row; }
    public float CellSize { get => _cellSize; }


    public static GridManager Instance;
    private void Awake() { Instance = this; }

    private void OnDrawGizmos()
    {
        gridDrawer.DrawGrid(_row, _column, _cellSize, _gridOrigin);
    }

}
