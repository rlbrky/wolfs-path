using UnityEngine;

public class Melee_ChaseState : EnemyMelee_Informer
{
    private EnemyMelee_Context _context;
    private float timer;

    public Melee_ChaseState(EnemyMelee_Context context, Melee_StateMachine.EnemyState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        timer = 0;
    }

    public override void UpdateState()
    {
        if (timer < 5 && _context.PlayerCollider != null && _context.IsGrounded)
        {
            _context._Animator.SetBool("isMoving", true);
            _context.StateMachine.transform.LookAt(new Vector3(_context.PlayerCollider.transform.position.x, _context.StateMachine.transform.position.y));
            _context.StateMachine.transform.position = Vector3.MoveTowards(_context.StateMachine.transform.position, new Vector3(_context.PlayerCollider.transform.position.x, _context.StateMachine.transform.position.y, _context.StateMachine.transform.position.z), _context.Speed * Time.deltaTime);
            CheckForPlayer();
            timer += Time.deltaTime;
        }
        else
        {
            _context._Animator.SetBool("isMoving", false);
            _context.PlayerCollider = null;
            _context.SetState(Melee_StateMachine.EnemyState.Patrol);
        }
    }

    public override void ExitState()
    {
        timer = 0;
        _context._Animator.SetBool("isMoving", false);
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
                    _context.SetState(Melee_StateMachine.EnemyState.Attacking);
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
