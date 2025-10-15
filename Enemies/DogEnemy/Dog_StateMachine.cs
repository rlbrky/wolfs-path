using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Dog_StateMachine : StateManager<Dog_StateMachine.DogState>
{
    public enum DogState
    {
        Idle,
        Chase,
        Patrol,
        Attack
    }

    private Animator _animator;
    private Rigidbody _rb;
    private Dog_Context _context;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Dog_Events dogEvents;
    [Header("General")]
    [SerializeField] private Dog_AttackRegisterer _attackRegisterer;
    [SerializeField] private BoxCollider _dogCollider;
    [SerializeField] private VisualEffect _bloodVFX;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    [Header("Stats")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _attackCD;
    [SerializeField] private float _speed;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _damage;

    [Header("Setup")]
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private int _checkFrameSetup = 20;

    public bool TEST;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        _context = new Dog_Context(this, _animator, _rb, _attackRegisterer, _dogCollider, _bloodVFX, 
            _impulseSource, _maxHealth, _attackCD, _speed, _detectionRadius, _damage, _whatIsPlayer, _checkFrameSetup, patrolPoints);

        dogEvents.context = _context;

        InitializeStates();
    }

    private void Update()
    {
        //FOR TESTING WILL BE DELETED.
        if (TEST)
            return;

        DogState nextStateKey = CurrentState.GetNextState();
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
        States.Add(DogState.Idle, new Dog_Idle(_context, DogState.Idle));
        States.Add(DogState.Patrol, new Dog_Patrol(_context, DogState.Patrol));
        States.Add(DogState.Chase, new Dog_Chase(_context, DogState.Chase));
        States.Add(DogState.Attack, new Dog_Attack(_context, DogState.Attack));
        CurrentState = States[DogState.Patrol];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
