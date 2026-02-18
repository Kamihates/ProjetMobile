using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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


    private List<GameObject> _MiniT1Queue = new();

    [SerializeField] private DeckManager deckManager;

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

    private void Start()
    {
        deckManager.OnT1InHand += removeMiniT1Preview;
    }
    private void OnDestroy()
    {
        deckManager.OnT1InHand -= removeMiniT1Preview;
    }


    private void removeMiniT1Preview()
    {
        if (_MiniT1Queue.Count > 0)
        {
            Destroy(_MiniT1Queue[0]);
            _MiniT1Queue.RemoveAt(0);
        }


        for (int i = 0; i <  _MiniT1Queue.Count; i++)
        {
            Vector2 targetPos = deckPos.position;
            targetPos.x += i * 0.4f;

            GeneralVisualController.Instance.FallAtoB(_MiniT1Queue[i].transform, 0.2f, _MiniT1Queue[i].transform.position, targetPos);
        }
    }


    public void PlayFusionParticule(List<RegionPiece> allPieces, RegionType type)
    {


        PlayFusionSound(type);


        Vector2 targertPos = GeneralVisualController.Instance.GetCenterPosition(allPieces);
        AnimationT1ToDeck(targertPos, type);

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

    private void PlayFusionSound(RegionType type)
    {
        switch (type)
        {
            case RegionType.Wind:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.DataAudio.WindFusion);
                break;
            case RegionType.Fire:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.DataAudio.FireFusion);
                break;
            case RegionType.Water:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.DataAudio.WaterFusion);
                break;
            case RegionType.Rock:
                AudioManager.Instance.PlaySFX(AudioManager.Instance.DataAudio.EarthFusion);
                break;
            default:
                break;
        }
    }

    private IEnumerator WaitForDestroyParticle(GameObject particle)
    {
        yield return new WaitForSeconds(3f);
        Destroy(particle);
    }

    public void AnimationT1ToDeck(Vector2 startPos, RegionType type)
    {
        // 1) on instantie notre t1 en petit à la position donnée
        GameObject miniT1 = Instantiate(visuPrefabT1, startPos, visuPrefabT1.transform.rotation);
        
        RegionData data = deckManager.GetT1Data(type);

        if (miniT1.TryGetComponent(out MiniT1VisualController controller) && data != null)
        {
            controller.Init(data);
        }

        _MiniT1Queue.Add(miniT1);

        // 2)
        Vector2 targetPos = deckPos.position;
        targetPos.x +=(  _MiniT1Queue.Count -1 )* 0.4f;

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
