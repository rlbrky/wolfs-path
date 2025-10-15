using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class Bomber_Context
{
    //General
    private Animator _animator;
    private Rigidbody _rb;
    private CapsuleCollider _ownCollider;
    private VisualEffect _bloodVFX;
    private CinemachineImpulseSource _impulseSource;

    //Stats
    private float _maxHealth;
    private float _attackCD;
    private float _speed;
    private float _detectionRadius;
    private float _damage;
    private float _bombSpeed;

    //Detection
    private LayerMask _whatIsPlayer; //Filled by hand in constructor.
    private int _checkFrameSetup; //Constructor sets the value.

    //Health
    private UnitHealth _enemyHealth;
    private Collider _playerCollider; //Filled in Idle or patrol state.

    //General checks.
    private bool _isStunned;
    private bool _countAttackCD;
    private bool _isGrounded;
    private bool _isAttacking;
    private bool _gotThrownToAir;
    private bool _gotAirHit;
    private float _currentHealth;
    private float _stunTime;
    private Bomber_StateMachine _stateMachine;

    //Other
    private Vector3 _bombDir;
    private Vector3 _airOffset;
    private int attackHash1;

    public Bomber_Context(Bomber_StateMachine stateMachine, Animator animator, Rigidbody rb
        , CapsuleCollider ownCollider, VisualEffect bloodVFX, CinemachineImpulseSource impulseSource,
        float maxHealth, float attackCD, float speed, float detectionRadius, float damage, LayerMask whatIsPlayer, int checkFrameSetup, float stunTime, Vector3 airOffset, float bombSpeed)
    {
        _stateMachine = stateMachine;
        _animator = animator;
        _rb = rb;
        _ownCollider = ownCollider;
        _bloodVFX = bloodVFX;
        _impulseSource = impulseSource;
        _attackCD = attackCD;
        _speed = speed;
        _detectionRadius = detectionRadius;
        _damage = damage;
        _whatIsPlayer = whatIsPlayer;
        _checkFrameSetup = checkFrameSetup;
        _stunTime = stunTime;
        _bombSpeed = bombSpeed;

        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _enemyHealth = new UnitHealth(_currentHealth, maxHealth);

        _airOffset = airOffset;

        attackHash1 = Animator.StringToHash("Attack");
    }

    public void SetState(Bomber_StateMachine.BomberState stateKey)
    {
        _stateMachine.TransitionToState(stateKey);
    }



    //----------------Read-only--------------------
    //Detection
    public LayerMask WhatIsPlayer => _whatIsPlayer;
    public int CheckFrameSetup => _checkFrameSetup;

    //General
    public Bomber_StateMachine StateMachine => _stateMachine;
    public Animator _Animator => _animator;
    public Rigidbody _Rigidbody => _rb;
    public CapsuleCollider OwnCollider => _ownCollider;
    public VisualEffect BloodVFX => _bloodVFX;
    public CinemachineImpulseSource ImpulseSource => _impulseSource;

    //Usage
    public float BombSpeed => _bombSpeed;
    public float StunTime => _stunTime;
    public float AttackCD => _attackCD;
    public int AttackHash1 => attackHash1;

    //----------------Read-Write---------------------
    //Stats
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public float DetectionRadius { get { return _detectionRadius; } set { _detectionRadius = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public Vector3 AirOffset { get { return _airOffset; } set { _airOffset = value; } }
    public Vector3 BombDir { get { return _bombDir; } set { _bombDir = value; } }

    //Health
    public UnitHealth EnemyHealth { get { return _enemyHealth; } set { _enemyHealth = value; } }
    public Collider PlayerCollider { get { return _playerCollider; } set { _playerCollider = value; } }

    //Checks
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public bool CountAttackCD { get { return _countAttackCD; } set { _countAttackCD = value; } }
    public bool IsStunned { get { return _isStunned; } set { _isStunned = value; } }
    public bool GotThrown { get { return _gotThrownToAir; } set { _gotThrownToAir = value; } }
    public bool GotAirHit { get { return _gotAirHit; } set { _gotAirHit = value; } }
}
