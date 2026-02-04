using UnityEditor;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Transform _gridParent;

    private bool _isPlaying = false;

    private void Start()
    {
        _isPlaying = true;

        foreach (GameObject child in _gridParent.transform)
            Destroy(child);
    }

    public void DrawGrid(int Row, int Column, float CellSize, Transform GridOrigin)
    {
        // Pour chaque ligne 
        for (int row =  0; row < Row; row++)
        {
            // Pour chaque Colonne
            for (int column = 0; column < Column; column++)
            {
                // position
                float slotWidth = CellSize;

                float posX = GridOrigin.position.x + (slotWidth * column);
                float posY = GridOrigin.position.y - (slotWidth * row);

                GameObject cell = Instantiate(_cellPrefab, _gridParent);

                GeneralVisualController.Instance.FitSpriteInCell(cell.GetComponent<SpriteRenderer>());

                cell.transform.position = new Vector2(posX, posY);


                


                //// -----------  Visuel (debug pour le moment)
                //Gizmos.color = Color.blue;
                //// haut
                //Gizmos.DrawLine(new Vector2(posX - slotWidth / 2, posY + slotWidth / 2), new Vector2(posX + slotWidth / 2, posY + slotWidth / 2));
                //// bas
                //Gizmos.DrawLine(new Vector2(posX - slotWidth / 2, posY - slotWidth / 2), new Vector2(posX + slotWidth / 2, posY - slotWidth / 2));
                //// gauche
                //Gizmos.DrawLine(new Vector2(posX - slotWidth / 2, posY + slotWidth / 2), new Vector2(posX - slotWidth / 2, posY - slotWidth / 2));
                //// droite
                //Gizmos.DrawLine(new Vector2(posX + slotWidth / 2, posY - slotWidth / 2), new Vector2(posX + slotWidth / 2, posY + slotWidth / 2));
                //// ------------
                
            }
        }
    }

}
