using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game Config", fileName = "New Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField, Foldout("Debug")] private bool loopAfterBoss = false;
    public bool LoopAfterBoss {get => loopAfterBoss; set => loopAfterBoss =  value;}

    [SerializeField, Foldout("Debug")] private bool _noGravityMode = false;
    public bool NoGravityMode { get => _noGravityMode; set => _noGravityMode = value; }
}