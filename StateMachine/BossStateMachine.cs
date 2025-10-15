using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossStateMachine : StateManager<BossStateMachine.BossState>
{
    public enum BossState
    {
        Idle,
        Chasing,
        Attacking
    }

    private Rigidbody _rigidbody;
    private BossContext _context;
    private Animator _animator;
    [Header("General")]
    [SerializeField] private Boss1_AnimEvents animEvents;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float maxHealth;
    [SerializeField] public float damage;
    [SerializeField] public BossAttackRegister attackRegisterer;
    [SerializeField] public BossAttackRegister kickRegister;
    [SerializeField] public Slider healthBar;

    [Header("Idle")]
    [SerializeField] private float idlingTime;
    [SerializeField] private float idlingMS;

    [Header("Detection")]
    [SerializeField] private float _detectionRadius;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private int _checkFrameSetup = 20;

    [Header("Room Control")]
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _context = new BossContext(this, _rigidbody, movementSpeed, attackRange, idlingMS, idlingTime, transform, _animator, maxHealth, _whatIsPlayer, _checkFrameSetup, _detectionRadius, wall1, wall2, healthBar);

        InitializeStates();
        attackRegisterer.context = _context;
        kickRegister.context = _context;
    }

    private void InitializeStates()
    {
        animEvents._context = _context;
        //Add states into StateManager "States" Dictionary and Sets Initial State.
        States.Add(BossState.Idle, new Idle(_context, BossState.Idle));
        States.Add(BossState.Chasing, new Chasing(_context, BossState.Chasing));
        States.Add(BossState.Attacking, new Attacking(_context, BossState.Attacking));
        CurrentState = States[BossState.Idle];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
