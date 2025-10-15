using Cinemachine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class Melee_StateMachine : StateManager<Melee_StateMachine.EnemyState>, IEnemy
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chasing,
        Attacking
    }

    private Animator _animator;
    private Rigidbody _rb;
    private EnemyMelee_Context _context;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Melee_AnimEvents animEvents;
    [Header("General")]
    [SerializeField] private EnemyAttackRegister _attackRegisterer;
    [SerializeField] private GameObject _executionZone;
    [SerializeField] private GameObject _canExecute;
    [SerializeField] private CapsuleCollider _leCollider;
    [SerializeField] private VisualEffect _bloodVFX;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    [Header("Offsets")]
    [SerializeField] private Vector3 _executionOffset; 
    [SerializeField] private Vector3 _offset; //Offset for air combo.

    [Header("Stats")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _attackCD;
    [SerializeField] private float _speed;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _damage;
    [SerializeField] private float _stunTime;

    //Execution
    public bool _isGettingExecuted;
    [Header("Setup")]
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private int _checkFrameSetup = 20;


    public bool TEST;
    private Vector3 startPosition;
    private int fallHash;

    public bool isGrounded
    {
        get => _context.IsGrounded;        
        set => _context.IsGrounded = value;
    }

    public Animator _Animator
    {
        get => _context._Animator;
    }

    public Vector3 executionOffset 
    {
        get => _executionOffset;
        set => _executionOffset = value;
    }

    public GameObject canExecute
    {
        get => _canExecute;
        set => _canExecute = value;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        _context = new EnemyMelee_Context(this, _animator, _rb, _attackRegisterer, _executionZone, _canExecute, _leCollider, _bloodVFX, _impulseSource
            , _maxHealth, _attackCD, _speed, _detectionRadius, _damage, _whatIsPlayer, _checkFrameSetup, _stunTime,  patrolPoints, _offset
            );

        _context.ExecutionZone.SetActive(false);

        _context.AttackRegisterer.gameObject.SetActive(false);

        animEvents.context = _context;
        fallHash = Animator.StringToHash("isFalling");
        startPosition = transform.position;

        InitializeStates();
    }

    private void Update() //Overrides StateManager's update function.
    {
        _context.IsGrounded = isGrounded;
        if (PlayerMovement.instance != null && PlayerMovement.instance.rb.linearVelocity.y < 0)
        {
            _context.GotThrown = false;
        }

        if (_context.GotThrown)
        {
            _context._Rigidbody.linearVelocity = Vector3.zero;
            transform.position = new Vector3(PlayerMovement.instance.transform.position.x + _offset.x, PlayerMovement.instance.transform.position.y, transform.position.z);
        }

        if (!_context.GotAirHit && !_context.IsGrounded)
            _animator.SetBool(fallHash, true);
        else if (_context.IsGrounded)
            _animator.SetBool(fallHash, false);

        //Stun enemy
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Stunned") || _context.GotThrown || _isGettingExecuted) 
            return;

        //FOR TESTING WILL BE DELETED.
        if (TEST)
            return;

        EnemyState nextStateKey = CurrentState.GetNextState();
        if (nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToState(nextStateKey);
        }
    }

    private void InitializeStates()
    {
        //animEvents._context = _context;
        //Add states into StateManager "States" Dictionary and Sets Initial State.
        States.Add(EnemyState.Idle, new Melee_IdleState(_context, EnemyState.Idle));
        States.Add(EnemyState.Patrol, new Melee_Patrol(_context, EnemyState.Patrol));
        States.Add(EnemyState.Chasing, new Melee_ChaseState(_context, EnemyState.Chasing));
        States.Add(EnemyState.Attacking, new Melee_AttackState(_context, EnemyState.Attacking));
        CurrentState = States[EnemyState.Patrol];
    }

    public void GetExecuted(int random)
    {
        PlayerCombat.instance.enemyToExecute = null;
        _rb.useGravity = false;
        //leCollider.enabled = false;
        transform.position = new Vector3(PlayerMovement.instance.transform.position.x + _executionOffset.x, transform.position.y, transform.position.z);
        _isGettingExecuted = true;
        _executionZone.SetActive(false);
        _canExecute.SetActive(false);

        switch (random)
        {
            case 0:
                _animator.SetTrigger("getExecuted1");
                break;
            case 1:
                _animator.SetTrigger("getExecuted2");
                break;
            case 2:
                _animator.SetTrigger("getExecuted3");
                break;
            case 3:
                _animator.SetTrigger("getExecuted4");
                break;
            case 4:
                _animator.SetTrigger("getExecuted5");
                break;
            case 5:
                _animator.SetTrigger("getExecuted6");
                break;
        }

        StartCoroutine(DelayDeath());
    }

    public void GetParried()
    {
        _animator.SetTrigger("getParried");
        _attackRegisterer.gameObject.SetActive(false);
        _executionZone.SetActive(true);

        StartCoroutine(PlayerCombat.instance.ParryCoroutine(0.3f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(2);
        animEvents.TriggerDeathRoutine();
    }

    public void Revive()
    {
        TEST = false;
        _context.IsStunned = false;
        transform.position = startPosition;
        _context.PlayerCollider = null;
        _context._Rigidbody.useGravity = true;
        _context.OwnCollider.enabled = true;
        CurrentState = States[EnemyState.Patrol];
        _context.EnemyHealth.HealUnit(_context.MaxHealth);
        gameObject.SetActive(true);
    }
}
