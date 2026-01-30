using UnityEngine;

public class DominoSpawner : MonoBehaviour
{
    [Header("Prefab"), SerializeField] private GameObject dominoPrefab;
    [Header("Spawn"), SerializeField] private Transform spawnPoint;

    [Header("Base Data")]
    [SerializeField] private DominoData dominoData; 

    public Domino SpawnDomino(DominoCombination dominoCombination)
    {
        GameObject dominoGO = Instantiate(dominoPrefab.gameObject, spawnPoint.position, Quaternion.identity);
        Domino dominoInstance = dominoGO.GetComponent<Domino>();
        dominoInstance.Init(dominoCombination, dominoData); 
        return dominoInstance;
    }
}
