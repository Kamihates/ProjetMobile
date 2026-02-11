using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Starting Deck Data", menuName = "Deck/Deck Data")]
public class DeckStartingData : ScriptableObject
{
    [SerializeField] private List<DominoInfos> dominos = new();
    public List<DominoInfos> Dominos { get => dominos; set => dominos = value; }
}
