using System.Collections;
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

    private bool _canFall = false;
    public bool CanFall { get => _canFall; set => _canFall = value; }
    public bool IgnoreCurrentDomino { get; set; }

    /// <summary>
    /// Initialisation des vitesses
    /// </summary>
    public void Init(float speed, float StepTimer)
    {
        IgnoreCurrentDomino = false;
        _currentFallingSpeed = speed;
        _currentStepSpeed = StepTimer;
    }

    private void FixedUpdate()
    {
        if (DominoPlacementController.Instance == null) { return; }

        if (!CanFall) { return; }

        if (!IgnoreCurrentDomino)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentDomino == null) return;
            if (GameManager.Instance.CurrentDomino.PieceUniqueId != _piece.PieceUniqueId) return;
        }
      

        Vector2 targetPos = DominoPlacementController.Instance.GetDestination(_piece);

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
            
            // si la position finale depasse de la grille c'est game over
           
            transform.position = targetPos; // on snap au cas ou

            if (!GridManager.Instance.IsDominoInGrid(_piece, false))
            {
                GameManager.Instance.OnGameLost?.Invoke();
            }

            DominoPiece domino = _piece;
            GridManager.Instance.AddDominoDataInGrid(domino);
            IgnoreCurrentDomino  = false;
            enabled = false;
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
