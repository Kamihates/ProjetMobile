using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Domino Data", menuName = "Domino/Domino Combinaison Data")]
public class CombinaisonData : ScriptableObject
{
    [BoxGroup("All dominos combinations")]
    [ReadOnly] public List<DominoInfos> allDominos = new();

    private List<RegionData> regionDatabase = new();

    private void OnEnable()
    {
        regionDatabase = new List<RegionData>(Resources.LoadAll<RegionData>("ScriptableObjects"));
    }

    [Button("Generate All Combinations")]
    public void GenerateAllCombinations()
    {
        allDominos.Clear();

        for (int i = 0; i < regionDatabase.Count; i++)
        {
            for(int j = 0; j < regionDatabase.Count; j++)
            {
                allDominos.Add(new DominoInfos
                {
                    Regions = new List<RegionData> 
                    { 
                        regionDatabase[i],
                        regionDatabase[j]
                    }
                    
                });
            }
        }

        Debug.Log($"Generated {allDominos.Count} domino combinations");
    }
}

