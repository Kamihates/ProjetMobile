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

    [SerializeField] private DeckManager deckManager;
    private Coroutine moveCoroutine = null;

    private Dictionary<RegionType, GameObject> _fusionParticles = new();
    private GameObject _currentMiniT1 = null;

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
        if (GridManager.Instance != null)
        {
            GridManager.Instance.OnDominoPlaced += removeMiniT1Preview;

        }
    }
    private void OnDestroy()
    {
        if (GridManager.Instance != null)
        {
            GridManager.Instance.OnDominoPlaced -= removeMiniT1Preview;

        }
    }


    private void removeMiniT1Preview(DominoPiece piece)
    {
        if (_currentMiniT1 != null && moveCoroutine == null)
        {

            Destroy(_currentMiniT1);
            _currentMiniT1 = null;
            
        }
    }


    public void PlayFusionParticule(List<RegionPiece> allPieces, RegionType type)
    {
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

        _currentMiniT1 = miniT1;

        // 2)
        Vector2 targetPos = deckPos.position;

        moveCoroutine = StartCoroutine(T1TrailCourbe(miniT1.transform, startPos, targetPos));

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
        moveCoroutine = null;
    }
}
