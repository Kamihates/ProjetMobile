using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion eau")] private GameObject ParticleWater;
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion terre")] private GameObject ParticleRock;
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion feu")] private GameObject ParticleFire;
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion vent")] private GameObject ParticleWind;

    private Dictionary<RegionType, GameObject> _fusionParticles = new();

    public static EventManager Instance;
    private void Awake() 
    { 
        Instance = this;

        _fusionParticles[RegionType.Rock] = ParticleRock;
        _fusionParticles[RegionType.Water] = ParticleWater;
        _fusionParticles[RegionType.Wind] = ParticleWind;
        _fusionParticles[RegionType.Fire] = ParticleFire;
    }


    public void PlayFusionParticule(List<RegionPiece> allPieces, RegionType type)
    {
        Vector2 targertPos = GeneralVisualController.Instance.GetCenterPosition(allPieces);

        Debug.Log("partivule a jouer ? " + _fusionParticles[type]);

        if (_fusionParticles[type] != null)
        {
            // on instantie la particule
            GameObject particleGO = Instantiate(_fusionParticles[type], targertPos, Quaternion.identity);

            foreach (Transform particle in particleGO.transform)
            {
                if (particle.TryGetComponent(out ParticleSystem systeme))
                {
                    systeme.Play();
                    Debug.Log("on joue la particule");
                }
            }

            StartCoroutine(WaitForDestroyParticle(particleGO));
        }
        
    }

    private IEnumerator WaitForDestroyParticle(GameObject particle)
    {
        yield return new WaitForSeconds(3f);
        Destroy(particle);
    }
}
