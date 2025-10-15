using UnityEngine;

public class Bomber_Idle : Bomber_Informer
{
    private float timer;
    public Bomber_Idle(Bomber_Context context, Bomber_StateMachine.BomberState stateKey) : base(context, stateKey)
    {
        _context = context;
    }
    //This state also serves as the "stunned" state.
    public override void EnterState()
    {
        if (_context.CountAttackCD)
        {
            timer = _context.AttackCD;
        }
        if (_context.IsStunned)
        {
            timer = _context.StunTime;
        }
    }
    public override void UpdateState()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            CheckForPlayer();
    }

    public override void ExitState()
    {
        _context.CountAttackCD = false;
        _context.IsStunned = false;
    }

    public override Bomber_StateMachine.BomberState GetNextState()
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
                    _context.SetState(Bomber_StateMachine.BomberState.Attacking);
                    break;
                }
                else
                    _context.PlayerCollider = null;
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
