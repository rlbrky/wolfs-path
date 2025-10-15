using UnityEngine;
using UnityEngine.UI;

public class BossContext
{
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Animator _animator;
    private CapsuleCollider _ownCollider;
    private Boss1_AnimEvents _bossEventHandler;
    private BossStateMachine _stateMachine;
    private Collider _playerCollider;
    private GameObject wall1;
    private GameObject wall2;
    private Vector3 _bossSpawnLoc;
    private Slider _healthBar;

    private int damageType;
    private float _movementSpeed;
    private float _attackRange;
    private float _idleMS;
    private float _idlingTime;
    private bool _movementComplete;
    private bool _isAttacking;
    private bool _endAttackState;
    private bool _checkingForPlayer;

    //Detection
    private LayerMask _whatIsPlayer; //Filled by hand in constructor.
    private int _checkFrameSetup; //Constructor sets the value.
    private float _detectionRadius;

    private UnitHealth _enemyHealth;
    private float _maxHealth;
    private float _currentHealth;

    public BossContext(BossStateMachine stateMachine, Rigidbody rigidbody, float movementSpeed, float attackRange, float idleMS, float idlingTime, Transform transform, Animator animator, float maxHealth,
        LayerMask whatIsPlayer, int checkFrameSetup, float detectionRadius, GameObject _wall1, GameObject _wall2, Slider healthBar)
    {
        _stateMachine = stateMachine;
        _rigidbody = rigidbody;
        _movementSpeed = movementSpeed;
        _attackRange = attackRange;
        _idleMS = idleMS;
        _idlingTime = idlingTime;
        _transform = transform;
        _animator = animator;
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;

        _whatIsPlayer = whatIsPlayer;
        _checkFrameSetup = checkFrameSetup;
        _detectionRadius = detectionRadius;
        _healthBar = healthBar;

        wall1 = _wall1;
        wall2 = _wall2;

        _enemyHealth = new UnitHealth(_currentHealth, maxHealth);

        _bossSpawnLoc = _transform.position;
    }

    public void SetState(BossStateMachine.BossState stateKey)
    {
        _stateMachine.TransitionToState(stateKey);
    }

    public void ActivateWalls()
    {
        wall1.SetActive(true);
        wall2.SetActive(true);
    }
    public void DeactivateWalls()
    {
        wall1.SetActive(false);
        wall2.SetActive(false);
    }
    public void DestroyWalls()
    {
        UnityEngine.GameObject.Destroy(wall1);
        UnityEngine.GameObject.Destroy(wall2);
    }

    //Read-only
    public Slider Healthbar => _healthBar;
    public float MaxHealth => _maxHealth;
    public Vector3 BossSpawnLoc => _bossSpawnLoc;
    public LayerMask WhatIsPlayer => _whatIsPlayer;
    public int CheckFrameSetup => _checkFrameSetup;
    public BossStateMachine StateMachine => _stateMachine;
    public Rigidbody Rb => _rigidbody;
    public CapsuleCollider OwnCollider => _ownCollider;
    public float MovementSpeed => _movementSpeed;
    public float AttackRange => _attackRange;
    public float IdleMS => _idleMS;
    public float IdlingTime => _idlingTime;
    public Transform Transform => _transform;
    public Animator animator => _animator;

    //Read-Write
    public float DetectionRadius { get { return _detectionRadius; } set { _detectionRadius = value; } }
    public Collider PlayerCollider { get { return _playerCollider; } set { _playerCollider = value; } }
    public Boss1_AnimEvents BossEventHandler { get { return _bossEventHandler; } set { _bossEventHandler = value; } }
    public bool movementComplete { get { return _movementComplete; } set { _movementComplete = value; } }
    public bool isAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public bool CheckingForPlayer { get { return _checkingForPlayer; } set { _checkingForPlayer = value; } }
    public bool endAttackState { get { return _endAttackState; } set { _endAttackState = value; } }
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public int DamageType { get { return damageType; } set { damageType = value; } }
}