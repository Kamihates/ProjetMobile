using System.Collections;
using UnityEngine;

public class DominoFall : MonoBehaviour
{

    [SerializeField] DominoPiece _piece;
    [SerializeField] private GameConfig _gameConfig;

    private Vector2Int _lastIndex;
    public Vector2Int LastIndex { get => _lastIndex; set => _lastIndex = value; }

    private float _baseFallingSpeed; 
    private float _baseStepSpeed;

    // case par case
    private float _fallingStepChrono = 0; // chrono entre chaque step
    private bool _canDoOneStep = true;

    // vitesse de fall actuelle
    private float _currentFallingSpeed;
    private float _currentStepSpeed;

    private bool _canFall = false;
    public bool CanFall { get => _canFall; set => _canFall = value; }
    public bool IgnoreCurrentDomino { get; set; }

    private bool _isAccelerating = false;
    public bool IsAccelerating { get=> _isAccelerating; set {
            _isAccelerating = value;
            ApplySpeed();
        } }

    private bool _isTapToFall = false;
    public bool IsTapToFall { get => _isTapToFall; set => _isTapToFall = value; }

    private void Start()
    {
        _lastIndex = new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Initialisation des vitesses
    /// </summary>
    public void Init(float speed, float StepTimer)
    {
        IgnoreCurrentDomino = false;

        _baseFallingSpeed = speed;
        _baseStepSpeed = StepTimer;

        ApplySpeed();

    }

    private void ApplySpeed()
    {
        if (_isAccelerating)
        {
            _currentFallingSpeed = _baseFallingSpeed *= 2f;
            _currentStepSpeed = _baseStepSpeed /= 2f;

        }
        else
        {
            _currentFallingSpeed = _baseFallingSpeed;
            _currentStepSpeed = _baseStepSpeed;
        }

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


        if (TEST_GD.Instance != null && (!GameManager.Instance.NoGravityMode || _isTapToFall))
        {
            //if (_gameConfig.FallPerCase) // déplacements case par case
            //{
            //    FallPerCase();
            //}
            //else // déplacements fluides
            //{
                Fall();
            //}
        }


        float distanceGap = 0.08f;

        // si on arrive a destination
        if ((Vector2.Distance(targetPos, transform.position) < distanceGap) || transform.position.y < targetPos.y) // evite un depassement
        {
            
            // si la position finale depasse de la grille c'est game over
           
            transform.position = targetPos; // on snap au cas ou
           

            if (!GridManager.Instance.IsDominoInGrid(_piece, false))
            {
                Debug.Log("perdu");
                GameManager.Instance.GameLost();
                return;
            }

            int orderLayerR1 = GridManager.Instance.GetIndexFromPosition(transform.GetChild(0).position).y;
            int orderLayerR2 = GridManager.Instance.GetIndexFromPosition(transform.GetChild(1).position).y;

            transform.GetChild(0).GetComponent<RegionPiece>().UpdateLayer(orderLayerR1);
            transform.GetChild(1).GetComponent<RegionPiece>().UpdateLayer(orderLayerR2);

            DominoPiece domino = _piece;
            GridManager.Instance.AddDominoDataInGrid(domino);
            IgnoreCurrentDomino = false;
            enabled = false;

            //// on recupere l'index
            //_lastIndex = GridManager.Instance.GetIndexFromPosition(transform.GetChild(0).position);
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
