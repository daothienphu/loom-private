using UnityEngine;

public class Character_Controller : MonoBehaviour
{
    //components
    private CharacterController _cc;
    private Animator _anim;

    //movement
    [SerializeField]
    private Vector2 _currentMovement;
    [SerializeField]
    private Vector2 _currentRunMovement;
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _runMultiplier = 3.0f;
    [SerializeField]
    private bool _isGrounded = false;
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private Transform _groundCheckPoint;
    private bool _isRunKeyPressed = false;
    private bool _isMovementKeyPressed = false;
    private bool _isLookingLeft = false;

    [Space]

    //jump
    [SerializeField]
    private float _jumpingHeight = 1.0f;
    [SerializeField]
    private float _jumpingTime = 0.7f;
    //private float _jumpingDistance;
    private float _initialJumpVelocity;
    private bool _isJumpKeyPressed = false;
    private bool _isJumping = false;

    [Space]

    //gravity
    [SerializeField]
    private float _gravity;

    [Space]

    //inputs
    private float _horizontalInput;
    private float _verticalInput;

    [Space]

    //animation
    private int _isWalkingHash;
    private int _isJumpingHash;
    private bool _isJumpingAnimActive;
    private bool _isWalkingAnimActive;

    #region Life cycle methods

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        
        GetAnimatorHashes();
        SetupJumpVariables();
    }

    void Update()
    {
        GetAllInputs();
        HandleAnimation();
        HandleRotation();

        
        HandleMovement();
        GroundCheck();
        HandleGravity();
        HandleJump();
        
    }
    #endregion

    #region Variables Setup
    void GetAnimatorHashes()
    {
        _isWalkingHash = Animator.StringToHash("isWalking");
        //_isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
    }

    void SetupJumpVariables()
    {
        float _timeToApex = _jumpingTime / 2;
        _gravity = (-2 * _jumpingHeight) / Mathf.Pow(_timeToApex, 2);
        _initialJumpVelocity = (2 * _jumpingHeight) / _timeToApex;
    }
    #endregion

    #region Movement Handlers
    void HandleGravity()
    {
        if (!_isGrounded)
        {
            _currentMovement.y += _gravity * Time.deltaTime;
            _currentRunMovement.y += _gravity * Time.deltaTime;
        }
        else
        {
            _currentMovement.y = 0f;
            _currentRunMovement.y = 0f;
        }
    } 
    
    void HandleMovement()
    {
        if (_isRunKeyPressed) {
            _cc.Move(_currentRunMovement * Time.deltaTime);
        }
        else {
            _cc.Move(_currentMovement * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (!_isJumping && _isGrounded && _isJumpKeyPressed)
        {
            _isJumping = true;
            _currentMovement.y = _initialJumpVelocity;
            _currentRunMovement.y = _initialJumpVelocity;
        }
        else if (!_isJumpKeyPressed && _isJumping && _isGrounded)
        {
            _isJumping = false;   
        }
        
    }
    
    void HandleRotation()
    {
        if ((!_isLookingLeft && _horizontalInput < 0) || (_isLookingLeft && _horizontalInput > 0))
        {
            transform.localScale = new Vector3(-1f * transform.localScale.x, 1f, 1f);
            _isLookingLeft = !_isLookingLeft;
        }
    }
    #endregion

    #region Animation Handlers
    void HandleAnimation()
    {
        _isWalkingAnimActive = _anim.GetBool(_isWalkingHash);
        _isJumpingAnimActive = _anim.GetBool(_isJumpingHash);
        Debug.Log(_isJumpingAnimActive);

        if (!_isWalkingAnimActive && _isMovementKeyPressed)
        {
            _anim.SetBool(_isWalkingHash, true);
        }
        else if (_isWalkingAnimActive && !_isMovementKeyPressed)
        {
            _anim.SetBool(_isWalkingHash, false);
        }

        if (!_isJumpingAnimActive && _isJumping)
        {
            _anim.SetBool(_isJumpingHash, true);
            _anim.SetBool(_isWalkingHash, false);
        }
        else if (_isJumpingAnimActive && !_isJumping)
        {
            _anim.SetBool(_isJumpingHash, false);
        }
    }
    #endregion

    #region Helper funtions
    void GroundCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheckPoint.transform.position, 0.1f, _groundLayer);
        _isGrounded = colliders.Length > 0;
    }

    void GetAllInputs()
    {
        //will upgrade to the new input system if im feeling like it.
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        //_verticalInput = Input.GetAxis("Vertical");
        _isRunKeyPressed = Input.GetKey(KeyCode.LeftShift);
        _isJumpKeyPressed = Input.GetKey(KeyCode.Space);

        _currentMovement.x = _horizontalInput * _movementSpeed;
        _currentRunMovement.x = _horizontalInput * _movementSpeed * _runMultiplier;
        _isMovementKeyPressed = _currentMovement.x != 0 || _currentRunMovement.x != 0;
    }
    #endregion
}
