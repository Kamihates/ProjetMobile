using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionVisualEffects : MonoBehaviour
{
    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion eau")] private GameObject ParticleWater;
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion terre")] private GameObject ParticleRock;
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion feu")] private GameObject ParticleFire;
    [BoxGroup("Particules Fusion"), SerializeField, Label("fusion vent")] private GameObject ParticleWind;

    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("T1 trainée"), SerializeField, Label("prefab mini t1")] private GameObject visuPrefabT1;
    [BoxGroup("T1 trainée"), SerializeField, Label("position de la fin du deck")] private Transform deckPos;
    [BoxGroup("T1 trainée"), SerializeField, Label("curve")] private AnimationCurve curve;

    private Dictionary<RegionType, GameObject> _fusionParticles = new();

    public static FusionVisualEffects Instance;
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
        AnimationT1ToDeck(targertPos);

        if (_fusionParticles[type] != null)
        {
            // on instantie la particule
            GameObject particleGO = Instantiate(_fusionParticles[type], targertPos, Quaternion.identity);

            foreach (Transform particle in particleGO.transform)
            {
                if (particle.TryGetComponent(out ParticleSystem systeme))
                {
                    systeme.Play();
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

    public void AnimationT1ToDeck(Vector2 startPos)
    {
        // 1) on instantie notre t1 en petit à la position donnée
        GameObject miniT1 = Instantiate(visuPrefabT1, startPos, Quaternion.Euler(new Vector3(0, 0, 45f)));

        // 2)
        Vector2 targetPos = deckPos.position;

        StartCoroutine(T1TrailCourbe(miniT1.transform, startPos, targetPos));

    }

    
    float duration = 1f;

    private IEnumerator T1TrailCourbe(Transform miniT1, Vector2 startPos, Vector2 target)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            Vector2 linearPos = Vector2.Lerp(startPos, target, t);


            // calcul du vecteur
            Vector2 direction = (target - startPos).normalized;

            // calcule de la normale (perpendiculaire) pour faire notre trajectoire
            Vector2 normal = new Vector2(-direction.y, direction.x);

            // calcule de la deviation selon la curve
            float curveValue = curve.Evaluate(t);

            Vector2 finalPos = linearPos + normal * (curveValue * 2);

            miniT1.position = finalPos;

            yield return null;
        }
    }
}
