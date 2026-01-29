using UnityEngine;

public class DominoSpawner : MonoBehaviour
{
    [SerializeField] private int _currentDominoId;
    public int CurrentDominoId => _currentDominoId;
}
