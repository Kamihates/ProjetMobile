using UnityEngine;

public class DominoPiece : MonoBehaviour
{
    private int _currentRotation = 0;

    [SerializeField] private Sprite _horizontal;
    [SerializeField] private Sprite _vertical;

    [SerializeField] private int _pieceUniqueId;
    public int PieceUniqueId { get => _pieceUniqueId; }

    //private DominoData data; 


    public void Rotate()
    {
        Vector3 TempScale = transform.localScale;

        float temp = TempScale.x;
        TempScale.x = TempScale.y;
        TempScale.y = temp;

        transform.localScale = TempScale;


    }


    public void Init(int _UniqueId, /*DominoData data,*/ int rotation)
    {
        _pieceUniqueId = _UniqueId;
        _currentRotation = rotation;
    }


}
