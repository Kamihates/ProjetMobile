using UnityEngine;

public class DominoPiece : MonoBehaviour
{
    [SerializeField] private DominoPieceVisual visualController;
    [SerializeField] private DominoFall fallController;

    private int _pieceUniqueId;
    private int _currentRotation = 0;
    private DominoInfos _data;

    public DominoPieceVisual Visual => visualController;
    public DominoFall FallController => fallController;
    public int PieceUniqueId { get => _pieceUniqueId; }
    public int Rotation { get => _currentRotation;
        set { 
            _currentRotation = value;
            if (value == 3)
                transform.GetChild(1).GetComponent<RegionPiece>().UpdateLayer(transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder - 1);
            else if (value == 1)
                transform.GetChild(1).GetComponent<RegionPiece>().UpdateLayer(transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder + 1);

        }
    }
    public DominoInfos Data => _data;

    public void Init(int _UniqueId, int rotation, DominoInfos dominoRegions)
    {
        _pieceUniqueId = _UniqueId;
        _data = dominoRegions;
        _currentRotation = rotation;

        visualController.Init(this);
        visualController.UpdateVisual();
        visualController.Rotate();

        fallController.enabled = true;
    }






}
