using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game Config", fileName = "New Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField, Foldout("Settings")] private bool loopAfterBoss = false;
    public bool LoopAfterBoss {get => loopAfterBoss; set => loopAfterBoss =  value;}

    [SerializeField, Foldout("Settings")] private bool _noGravityMode = false;
    public bool NoGravityMode { get => _noGravityMode; set => _noGravityMode = value; }

    [SerializeField, Foldout("Settings")] private bool fallPerCase;
    public bool FallPerCase { get => fallPerCase; set => fallPerCase = value; }
}