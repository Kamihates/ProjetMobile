using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Domino Data", menuName = "Domino/Domino Data")]
public class DominoData : ScriptableObject
{
    [BoxGroup("Sprites"), ShowAssetPreview]
    [SerializeField] private Sprite[] dominoSprites;

    [BoxGroup("All dominos combinations")]
    [ReadOnly] public List<DominoCombination> allDominos = new();

    [Button("Generate All Combinations")]
    public void GenerateAllCombinations()
    {
        allDominos.Clear();

        for (int left = 0; left < dominoSprites.Length; left++)
        {
            for (int right = left; right < dominoSprites.Length; right++)
            {
                allDominos.Add(new DominoCombination(left, right));
            }
        }

        Debug.Log($"Generated {allDominos.Count} domino combinations");
    }

    public Sprite GetSprite(int index) => dominoSprites[index];
}
