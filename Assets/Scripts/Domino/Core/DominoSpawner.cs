using UnityEngine;

public class DominoSpawner : MonoBehaviour
{
    [Header("Prefab"), SerializeField] private Domino dominoPrefab;
    [Header("Spawn"), SerializeField] private Transform spawnPoint;

    public Domino SpawnDomino(DominoData dominoData)
    {
        GameObject domino = Instantiate(dominoPrefab.gameObject, spawnPoint.position, Quaternion.identity);
        Domino dominoInstance = domino.GetComponent<Domino>();
        dominoInstance.Init(dominoData);
        return dominoInstance;
    }
}
