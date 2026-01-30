using UnityEditor;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    [Header("Nombre de colonnes")] [SerializeField] private int _column;
    [Header("Nombre de Lignes")] [SerializeField] private int _row;
    [Header("Origine de la grille")] [SerializeField] private Transform _gridOrigin;
    [Header("Taille d'une cellule")] [SerializeField] private float _cellSize;




    public int Column { get => _column; }
    public Transform Origin { get => _gridOrigin; }
    public int Row { get => _row; }
    public float CellSize { get => _cellSize; }





    public static GridDrawer Instance;
    private void Awake() { Instance = this; }

    void DrawGrid()
    {
        // Pour chaque ligne 
        for (int row =  0; row < _row; row++)
        {
            // Pour chaque Colonne
            for (int column = 0; column < _column; column++)
            {
                // position
                float slotWidth = _cellSize;

                float posX = _gridOrigin.position.x + (slotWidth * column);
                float posY = _gridOrigin.position.y - (slotWidth * row);

                // Visuel (debug pour le moment)
                Gizmos.color = Color.blue;
                // haut
                Gizmos.DrawLine(new Vector2(posX - slotWidth / 2, posY + slotWidth / 2), new Vector2(posX + slotWidth / 2, posY + slotWidth / 2));
                // bas
                Gizmos.DrawLine(new Vector2(posX - slotWidth / 2, posY - slotWidth / 2), new Vector2(posX + slotWidth / 2, posY - slotWidth / 2));
                // gauche
                Gizmos.DrawLine(new Vector2(posX - slotWidth / 2, posY + slotWidth / 2), new Vector2(posX - slotWidth / 2, posY - slotWidth / 2));
                // droite
                Gizmos.DrawLine(new Vector2(posX + slotWidth / 2, posY - slotWidth / 2), new Vector2(posX + slotWidth / 2, posY + slotWidth / 2));

                
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawGrid();
    }
}
