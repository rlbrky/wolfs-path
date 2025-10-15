using UnityEngine;

public class Melee_IdleState : EnemyMelee_Informer
{
    private float timer;
    private EnemyMelee_Context _context;

    public Melee_IdleState(EnemyMelee_Context context, Melee_StateMachine.EnemyState stateKey) : base(context, stateKey)
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
        if (timer < 0)
        {
            _context.SetState(Melee_StateMachine.EnemyState.Chasing);
        }
        else
            timer -= Time.deltaTime;
    }

    public override void ExitState()
    {
        _context.CountAttackCD = false;
        _context.IsStunned = false;
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
