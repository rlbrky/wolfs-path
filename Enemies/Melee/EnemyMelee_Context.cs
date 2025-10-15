using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyMelee_Context
{
    //General
    private Transform[] _patrolPoints;
    private Animator _animator;
    private Rigidbody _rb;
    private EnemyAttackRegister _attackRegisterer;
    private GameObject _executionZone;
    private GameObject _canExecute;
    private CapsuleCollider _ownCollider;
    private VisualEffect _bloodVFX;
    private CinemachineImpulseSource _impulseSource;

    //Stats
    private float _maxHealth;
    private float _attackCD;
    private float _speed;
    private float _damage;

    //Detection
    private LayerMask _whatIsPlayer; //Filled by hand in constructor.
    private int _checkFrameSetup; //Constructor sets the value.
    private float _detectionRadius;

    //Health
    private UnitHealth _enemyHealth;
    private Collider _playerCollider; //Filled in Idle or patrol state.

    //General checks.
    private bool _isStunned;
    private bool _countAttackCD;
    private bool _isGrounded;
    private bool _isAttacking;
    private bool _shouldKnock;
    private bool _gotThrownToAir;
    private bool _gotAirHit;
    private float _currentHealth;
    private float _stunTime;
    private Melee_StateMachine _stateMachine;

    //Other
    private Vector3 _airOffset;
    private int attackHash1;
    private int attackHash2;
    private int attackHash3;

    public EnemyMelee_Context(Melee_StateMachine stateMachine, Animator animator, Rigidbody rb, EnemyAttackRegister attackRegisterer, GameObject executionZone
        ,GameObject canExecute, CapsuleCollider leCollider, VisualEffect bloodVFX, CinemachineImpulseSource impulseSource, 
        float maxHealth, float attackCD, float speed, float detectionRadius, float damage, LayerMask whatIsPlayer, int checkFrameSetup, float stunTime, Transform[] patrolPoints, Vector3 airOffset)
    {
        _stateMachine = stateMachine;
        _animator = animator;
        _rb = rb;
        _attackRegisterer = attackRegisterer;
        _executionZone = executionZone;
        _canExecute = canExecute;
        _ownCollider = leCollider;
        _bloodVFX = bloodVFX;
        _impulseSource = impulseSource;
        _attackCD = attackCD;
        _speed = speed;
        _detectionRadius = detectionRadius;
        _damage= damage;
        _whatIsPlayer = whatIsPlayer;
        _checkFrameSetup = checkFrameSetup;
        _stunTime = stunTime;

        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _enemyHealth = new UnitHealth(_currentHealth, maxHealth);
        _attackRegisterer.context = this;

        _patrolPoints = patrolPoints;
        _airOffset = airOffset;

        attackHash1 = Animator.StringToHash("Attack1");
        attackHash2 = Animator.StringToHash("Attack2");
        attackHash3 = Animator.StringToHash("Attack3");
    }

    public void SetState(Melee_StateMachine.EnemyState stateKey)
    {
       _stateMachine.TransitionToState(stateKey);
    }

    //----------------Read-only--------------------
    //Detection
    public LayerMask WhatIsPlayer => _whatIsPlayer;
    public int CheckFrameSetup => _checkFrameSetup;

    //General
    public Melee_StateMachine StateMachine => _stateMachine;
    public Animator _Animator => _animator;
    public Rigidbody _Rigidbody => _rb;
    public EnemyAttackRegister AttackRegisterer => _attackRegisterer;
    public GameObject ExecutionZone => _executionZone;
    public GameObject CanExecute => _canExecute;
    public CapsuleCollider OwnCollider => _ownCollider;
    public VisualEffect BloodVFX => _bloodVFX;
    public CinemachineImpulseSource ImpulseSource => _impulseSource;

    //Usage
    public Vector3 GetPatrolPoint(int index) { return _patrolPoints[index].position; }
    public float StunTime => _stunTime;
    public float AttackCD => _attackCD;
    public int AttackHash1 => attackHash1;
    public int AttackHash2 => attackHash2;
    public int AttackHash3 => attackHash3;

    //----------------Read-Write---------------------
    //Stats
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public float DetectionRadius { get { return _detectionRadius; } set { _detectionRadius = value; } }
    public float Speed { get { return _speed; }  set { _speed = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public Vector3 AirOffset { get { return _airOffset; } set { _airOffset = value; } }

    //Health
    public UnitHealth EnemyHealth { get { return _enemyHealth; } set { _enemyHealth = value; } }
    public Collider PlayerCollider { get { return _playerCollider; } set { _playerCollider = value; } }

    //Checks
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool ShouldKnock { get { return _shouldKnock; } set { _shouldKnock = value; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public bool CountAttackCD { get { return _countAttackCD; } set { _countAttackCD = value; } }
    public bool IsStunned { get { return _isStunned; } set { _isStunned = value; } }
    public bool GotThrown { get { return _gotThrownToAir; } set { _gotThrownToAir = value; } }
    public bool GotAirHit { get { return _gotAirHit; } set { _gotAirHit = value; } }
}
