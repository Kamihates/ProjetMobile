using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Domino Data", menuName = "Domino/Domino Combinaison Data")]
public class CombinaisonData : ScriptableObject
{
    [BoxGroup("All dominos combinations")]
    [ReadOnly] public List<DominoInfos> allDominos = new();

    [SerializeField] private RegionDatabase regionDatabase;

     

    [Button("Generate All Combinations")]
    public void GenerateAllCombinations()
    {
        allDominos.Clear();

        for (int i = 0; i < regionDatabase.AllRegionsData.Count; i++)
        {
            for(int j = 0; j < regionDatabase.AllRegionsData.Count; j++)
            {
                allDominos.Add(new DominoInfos
                {
                    Regions = new List<RegionData> 
                    { 
                        regionDatabase.AllRegionsData[i],
                        regionDatabase.AllRegionsData[j]
                    }
                    
                });
            }
        }

        Debug.Log($"Generated {allDominos.Count} domino combinations");
    }
}

