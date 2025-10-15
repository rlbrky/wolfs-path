using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static Melee_StateMachine;

public class Bomber_StateMachine : StateManager<Bomber_StateMachine.BomberState>, IEnemy
{
    public enum BomberState
    {
        Idle,
        Attacking
    }

    private Animator _animator;
    private Rigidbody _rb;
    private Bomber_Context _context;
    private Bomber_Events _events;

    [Header("General")]
    [SerializeField] private CapsuleCollider _ownCollider;
    [SerializeField] private VisualEffect _bloodVFX;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] public Collider ground;

    [Header("Offsets")]
    [SerializeField] private Vector3 _offset; //Offset for air combo.

    [Header("Stats")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _attackCD;
    [SerializeField] private float _speed;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _damage;
    [SerializeField] private float _stunTime;
    [SerializeField] private float _bombSpeed;

    [Header("Setup")]
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private int _checkFrameSetup = 20;

    public bool TEST;
    private int fallHash;
    private Vector3 startPos;

    public bool isGrounded
    {
        get => _context.IsGrounded;
        set => _context.IsGrounded = value;
    }

    public Animator _Animator
    {
        get => _context._Animator;
    }
    public Vector3 executionOffset { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public GameObject canExecute { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _events = GetComponent<Bomber_Events>();

        _context = new Bomber_Context(this, _animator, _rb,  _ownCollider, _bloodVFX, _impulseSource
            , _maxHealth, _attackCD, _speed, _detectionRadius, _damage, _whatIsPlayer, _checkFrameSetup, _stunTime, _offset, _bombSpeed
            );

        _events.context = _context;

        fallHash = Animator.StringToHash("isFalling");
        startPos = transform.position;

        InitializeStates();
    }

    private void Update() //Overrides StateManager's update function.
    {
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
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Stunned") || _context.GotThrown)
            return;

        //FOR TESTING WILL BE DELETED.
        if (TEST)
            return;

        BomberState nextStateKey = CurrentState.GetNextState();
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
        //Add states into StateManager "States" Dictionary and Sets Initial State.
        States.Add(BomberState.Idle, new Bomber_Idle(_context, BomberState.Idle));
        States.Add(BomberState.Attacking, new Bomber_Attack(_context, BomberState.Attacking));
        CurrentState = States[BomberState.Idle];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    public void Revive()
    {
        TEST = false;
        _context.IsStunned = false;
        _ownCollider.enabled = true;
        _rb.useGravity = true;
        transform.position = startPos;
        _context.PlayerCollider = null;
        CurrentState = States[BomberState.Idle];
        _context.EnemyHealth.HealUnit(_context.MaxHealth);
        gameObject.SetActive(true);
    }

    public void GetExecuted(int random)
    {
        throw new System.NotImplementedException();
    }
}
