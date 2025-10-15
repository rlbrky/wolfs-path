using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : BossStateInformer
{
    private BossContext _context;

    public Attacking(BossContext context, BossStateMachine.BossState stateKey) : base(context, stateKey)
    {
         _context = context;
    }
    
    public override void EnterState()
    {
        _context.isAttacking = false;
    }

    public override void UpdateState()
    {
        if (!_context.isAttacking)
        {
            int random = Random.Range(0, 3);
            _context.isAttacking = true;
            switch (random) 
            {
                case 0:
                    _context.animator.SetTrigger("Combo1_Opener");
                    _context.Rb.linearVelocity = _context.Transform.forward * _context.IdleMS;
                    _context.BossEventHandler.CheckComboFollowUp();
                    break;
                case 1:
                    _context.animator.SetTrigger("Combo2");
                    _context.Rb.linearVelocity = _context.Transform.forward * _context.IdleMS;
                    break;
                case 2:
                    _context.animator.SetTrigger("Combo3");
                    _context.Rb.linearVelocity = _context.Transform.forward * _context.IdleMS;
                    break;
            }
        }

        if (_context.endAttackState)
        {
            GetNextState();
        }
    }

    public override void ExitState()
    {
        _context.Rb.linearVelocity = Vector3.zero;
        _context.endAttackState = false;
    }

    public override BossStateMachine.BossState GetNextState()
    {
        if (_context.endAttackState)
            return BossStateMachine.BossState.Idle;

        return StateKey;
    }

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
}
