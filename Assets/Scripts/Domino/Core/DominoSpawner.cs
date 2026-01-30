using UnityEngine;

public class DominoSpawner : MonoBehaviour
{
    [Header("Prefab"), SerializeField] private GameObject dominoPrefab;
    [Header("Spawn"), SerializeField] private Transform spawnPoint;

    [Header("Base Data")]
    [SerializeField] private DominoData dominoData; 


    [SerializeField] private int _currentDominoId;
    public int CurrentDominoId => _currentDominoId;


    public Domino SpawnDomino(DominoCombination dominoCombination)
    {
        GameObject dominoGO = Instantiate(dominoPrefab.gameObject, spawnPoint.position, Quaternion.identity);
        Domino dominoInstance = dominoGO.GetComponent<Domino>();
        dominoInstance.Init(dominoCombination, dominoData); 
        return dominoInstance;
    }
    
}
