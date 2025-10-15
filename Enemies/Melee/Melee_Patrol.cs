using UnityEngine;

public class Melee_Patrol : EnemyMelee_Informer
{
    private EnemyMelee_Context _context;
    private Vector3 wp;
    private Vector3[] _waypoints;

    private float _waitTime = 1f; //in seconds
    private float _waitCounter = 0f;

    private bool _waiting = false;
    private bool routeChanged;

    private int _currentWaypointIndex;

    public Melee_Patrol(EnemyMelee_Context context, Melee_StateMachine.EnemyState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        if(_waypoints == null)
        {
            _waypoints = new Vector3[2];
            _waypoints[0] = _context.GetPatrolPoint(0);
            _waypoints[1] = _context.GetPatrolPoint(1);
        }
    }
    public override void UpdateState()
    {
        CheckForPlayer();
        //if (!routeChanged)
        //{
        //    _waitCounter = 0f;
        //    _waiting = true;

        //    _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        //    wp = _waypoints[_currentWaypointIndex];
        //    _context._Animator.SetBool("isMoving", false);
        //}

        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                _context._Animator.SetBool("isMoving", true);

                //if (Mathf.Abs(wp.x - (_context.StateMachine.transform.forward + _context.StateMachine.transform.position).x) > Mathf.Abs(wp.x - _context.StateMachine.transform.position.x))
                //    _context.StateMachine.transform.LookAt(new Vector3(wp.x, _context.StateMachine.transform.position.y));
            }
        }
        else if(_context.IsGrounded)
        {
            wp = _waypoints[_currentWaypointIndex];
            _context.StateMachine.transform.LookAt(new Vector3(wp.x, _context.StateMachine.transform.position.y));
            //if ((Vector3.Distance(_transform.position, wp.position) < 0.01f))
            if (Mathf.Abs(_context.StateMachine.transform.position.x - wp.x) < 0.1f)
            {
                //_transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;
                _context._Rigidbody.linearVelocity = Vector3.zero;
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                _context._Animator.SetBool("isMoving", false);

            }
            else
            {
                _context._Animator.SetBool("isMoving", true);
                _context._Rigidbody.linearVelocity = Vector3.forward * _context.Speed;
                //if (Mathf.Abs(_context.StateMachine.transform.position.x - wp.x) > 0.1f)
                //{
                //    //_context._Rigidbody.AddRelativeForce(Vector3.forward * _context.Speed, ForceMode.Force);
                //}
                //_context.StateMachine.transform.position = Vector3.MoveTowards(_context.StateMachine.transform.position, ,);
            }
        }
    }
    public override void ExitState()
    {
    }
    public override Melee_StateMachine.EnemyState GetNextState()
    {
        return StateKey;
    }

    private void CheckForPlayer()
    {
        if (Time.frameCount % _context.CheckFrameSetup == 0)
        {
            Collider[] colliders = Physics.OverlapSphere(_context.StateMachine.transform.position, _context.DetectionRadius, _context.WhatIsPlayer);
            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    _context.PlayerCollider = collider;
                    _context.SetState(Melee_StateMachine.EnemyState.Chasing);
                    break;
                }
            }
        }
    }

    #region NOT IMPLEMENTED
    public override void OnTriggerEnter(Collider other)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTriggerExit(Collider other)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTriggerStay(Collider other)
    {
        //throw new System.NotImplementedException();
    }
    #endregion
}
