using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Dog_Context : MonoBehaviour
{
    //General
    private Transform[] _patrolPoints;
    private Animator _animator;
    private Rigidbody _rb;
    private VisualEffect _bloodVFX;
    private CinemachineImpulseSource _impulseSource;
    private BoxCollider _ownCollider;
    private Dog_StateMachine _stateMachine;
    private Dog_AttackRegisterer _attackRegisterer;

    //Stats
    private float _maxHealth;
    private float _attackCD;
    private float _speed;
    private float _detectionRadius;
    private float _damage;

    //Detection
    private LayerMask _whatIsPlayer; //Filled by hand in constructor.
    private int _checkFrameSetup; //Constructor sets the value.

    //Health
    private UnitHealth _enemyHealth;
    private Collider _playerCollider; //Filled in Idle or patrol state.

    //General Checks
    private bool _isGrounded;
    private bool _isAttacking;
    private float _currentHealth;

    private int attackHash1;

    public Dog_Context(Dog_StateMachine stateMachine, Animator animator, Rigidbody rb, Dog_AttackRegisterer attackRegisterer
        , BoxCollider ownCollider, VisualEffect bloodVFX, CinemachineImpulseSource impulseSource,
        float maxHealth, float attackCD, float speed, float detectionRadius, float damage, LayerMask whatIsPlayer, int checkFrameSetup, Transform[] patrolPoints)
    {
        _stateMachine = stateMachine;
        _animator = animator;
        _rb = rb;
        _attackRegisterer = attackRegisterer;
        _ownCollider = ownCollider;
        _bloodVFX = bloodVFX;
        _impulseSource = impulseSource;
        _attackCD = attackCD;
        _speed = speed;
        _detectionRadius = detectionRadius;
        _damage = damage;
        _whatIsPlayer = whatIsPlayer;
        _checkFrameSetup = checkFrameSetup;

        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _enemyHealth = new UnitHealth(_currentHealth, maxHealth);
        _attackRegisterer.context = this;

        _patrolPoints = patrolPoints;

        attackHash1 = Animator.StringToHash("Attack");
    }

    public void SwitchState(Dog_StateMachine.DogState stateKey)
    {
        _stateMachine.TransitionToState(stateKey);
    }

    //----------------Read-only--------------------
    //Detection
    public LayerMask WhatIsPlayer => _whatIsPlayer;
    public int CheckFrameSetup => _checkFrameSetup;

    //General
    public Dog_StateMachine StateMachine => _stateMachine;
    public Animator _Animator => _animator;
    public Rigidbody _Rigidbody => _rb;
    public Dog_AttackRegisterer AttackRegisterer => _attackRegisterer;
    public BoxCollider DogCollider => _ownCollider;
    public VisualEffect BloodVFX => _bloodVFX;
    public CinemachineImpulseSource ImpulseSource => _impulseSource;

    //Usage
    public Vector3 GetPatrolPoint(int index) { return _patrolPoints[index].position; }
    public float AttackCD => _attackCD;
    public int AttackHash1 => attackHash1;

    //----------------Read-Write---------------------
    //Stats
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public float DetectionRadius { get { return _detectionRadius; } set { _detectionRadius = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }

    //Health
    public UnitHealth EnemyHealth { get { return _enemyHealth; } set { _enemyHealth = value; } }
    public Collider PlayerCollider { get { return _playerCollider; } set { _playerCollider = value; } }

    //Checks
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
}
