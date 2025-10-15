using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bomber_Attack : Bomber_Informer
{
    public Bomber_Attack(Bomber_Context context, Bomber_StateMachine.BomberState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _context.IsAttacking = false;
        if (_context.PlayerCollider != null)
            AttackCalc();
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        _context._Animator.ResetTrigger(_context.AttackHash1);
    }

    public override Bomber_StateMachine.BomberState GetNextState()
    {
        return StateKey;
    }

    private void AttackCalc()
    {
        _context.BombDir = (new Vector3(_context.PlayerCollider.transform.position.x, _context.PlayerCollider.transform.position.y) - new Vector3(_context.StateMachine.transform.position.x, _context.StateMachine.transform.position.y)).normalized;
        if (_context.BombDir.x > 0) //Player is at the right side of the enemy.
            _context.StateMachine.transform.eulerAngles = new Vector3(0, 90);
        else
            _context.StateMachine.transform.eulerAngles = new Vector3(0, -90);

        _context.IsAttacking = true;
        _context.CountAttackCD = true;
        _context._Animator.SetTrigger(_context.AttackHash1);
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
