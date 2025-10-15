using UnityEngine;

public class Dog_Attack : Dog_Informer
{
    public Dog_Attack(Dog_Context context, Dog_StateMachine.DogState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _context.IsAttacking = false;

        if (_context.PlayerCollider != null && _context.IsGrounded)
        {
            _context.IsAttacking = true;
            _context.AttackRegisterer.gameObject.SetActive(true);
            int random = Random.Range(0, 1);
            switch (random)
            {
                case 0:
                    _context._Animator.SetTrigger(_context.AttackHash1);
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

    public override Dog_StateMachine.DogState GetNextState()
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
