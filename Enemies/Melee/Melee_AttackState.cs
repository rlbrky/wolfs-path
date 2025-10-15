using UnityEngine;

public class Melee_AttackState : EnemyMelee_Informer
{
    private EnemyMelee_Context _context;

    public Melee_AttackState(EnemyMelee_Context context, Melee_StateMachine.EnemyState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _context.IsAttacking = false;

        if (_context.PlayerCollider != null && _context.IsGrounded)
        {
            _context.IsAttacking = true;
            _context.CountAttackCD = true;
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    _context._Animator.SetTrigger(_context.AttackHash1);
                    break;
                case 1:
                    _context._Animator.SetTrigger(_context.AttackHash2);
                    break;
                case 2:
                    _context._Animator.SetTrigger(_context.AttackHash3);
                    break;
            }
        }
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        
    }

    public override Melee_StateMachine.EnemyState GetNextState()
    {
        return StateKey;
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
