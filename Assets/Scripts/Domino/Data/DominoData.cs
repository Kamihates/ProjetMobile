using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Domino Data", menuName = "Domino/Domino Data")]
public class DominoData : ScriptableObject
{
    [BoxGroup("All dominos combinations")]
    [ReadOnly] public List<List<RegionData>> allDominos = new();

    [SerializeField] private RegionDatabase regionDatabase;

    private List<RegionData> dominoRegion = new List<RegionData>();

    [Button("Generate All Combinations")]
    public void GenerateAllCombinations()
    {
        allDominos.Clear();

        for (int i = 0; i < regionDatabase.AllRegionsData.Count; i++)
        {
            for(int j = 0; j < regionDatabase.AllRegionsData.Count; j++)
            {
                dominoRegion.Clear();

                dominoRegion.Add(regionDatabase.AllRegionsData[i]);
                dominoRegion.Add(regionDatabase.AllRegionsData[j]);

                allDominos.Add(dominoRegion);
            }
        }

        Debug.Log($"Generated {allDominos.Count} domino combinations");
    }
}
