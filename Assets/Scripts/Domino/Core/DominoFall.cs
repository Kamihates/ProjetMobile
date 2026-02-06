using UnityEngine;

public class DominoFall : MonoBehaviour
{

    [SerializeField] DominoPiece _piece;

    // case par case
    private float _fallingStepChrono = 0; // chrono entre chaque step
    private bool _canDoOneStep = true;

    // vitesse de fall actuelle
    private float _currentFallingSpeed;
    private float _currentStepSpeed;

    public bool CanFall { get; set; }

    /// <summary>
    /// Initialisation des vitesses
    /// </summary>
    public void Init(float speed, float StepTimer)
    {
        CanFall = false;
        _currentFallingSpeed = speed;
        _currentStepSpeed = StepTimer;
    }

    private void FixedUpdate()
    {
        if (DominoPlacementController.Instance == null) { return; }

        if (!CanFall)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentDomino == null) return;
            if (GameManager.Instance.CurrentDomino.PieceUniqueId != _piece.PieceUniqueId) return;
        }
        


        Vector2Int currentIndex = GridManager.Instance.GetIndexFromPosition(transform.position);

        Vector2 targetPos = DominoPlacementController.Instance.GetFinalDestination(_piece, currentIndex);

        if (TEST_GD.Instance != null)
        {
            if (TEST_GD.Instance.FallPerCase) // déplacements case par case
            {
                FallPerCase();
            }
            else // déplacements fluides
            {
                Fall();
            }
        }

        // si on arrive a destination
        if ((Vector2.Distance(targetPos, transform.position) < 0.01f) || transform.position.y < targetPos.y) // evite un depassement
        {
            transform.position = targetPos; // on snap au cas ou
            DominoPiece domino = _piece;
            GridManager.Instance.AddDominoDataInGrid(domino);
            CanFall  = false;
            this.enabled = false;
        }
    }

    private void FallPerCase()
    {
        // on fait tomber le domino a vitesse constante
        Vector2 newPos = transform.position;
        _fallingStepChrono += Time.deltaTime;

        if (_canDoOneStep)
        {
            newPos.y -= GridManager.Instance.CellSize;
            _canDoOneStep = false;
        }

        if (_fallingStepChrono >= _currentStepSpeed)
        {
            _canDoOneStep = true;
            _fallingStepChrono = 0;
        }

        transform.position = newPos;
    }

    private void Fall()
    {
        Vector2 newPos = transform.position;
        newPos.y -= _currentFallingSpeed * Time.deltaTime;
        transform.position = newPos;
    }
}
