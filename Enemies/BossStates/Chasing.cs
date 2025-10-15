using UnityEngine;

public class Chasing : BossStateInformer
{
    private bool changeState;
    private BossContext _context;

    public Chasing(BossContext context, BossStateMachine.BossState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        changeState = false;
    }

    public override void UpdateState()
    {
        if(Mathf.Abs(PlayerMovement.instance.transform.position.x - _context.Transform.position.x) > _context.AttackRange)
        {
            if (PlayerMovement.instance.transform.position.x - _context.Transform.position.x > 0) //Is the player on the left side or the right side?
                _context.Transform.rotation = Quaternion.Euler(0, 90, 0);
            else
                _context.Transform.rotation = Quaternion.Euler(0, -90, 0);

            _context.animator.SetBool("isMoving", true);
            _context.Rb.linearVelocity = _context.Transform.forward * _context.MovementSpeed;
        }
        else
        {
            changeState = true;
            GetNextState();
        }
    }

    public override void ExitState()
    {
        _context.animator.SetBool("isMoving", false);
    }

    public override BossStateMachine.BossState GetNextState()
    {
        if (changeState)
            return BossStateMachine.BossState.Attacking;

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
