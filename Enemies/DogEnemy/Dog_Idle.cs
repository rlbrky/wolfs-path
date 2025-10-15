using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_Idle : Dog_Informer
{
    private float timer;

    public Dog_Idle(Dog_Context context, Dog_StateMachine.DogState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    //This state also serves as the "stunned" state.
    public override void EnterState()
    {
        timer = _context.AttackCD;
    }
    public override void UpdateState()
    {
        if (timer < 0)
        {
            _context.SwitchState(Dog_StateMachine.DogState.Chase);
        }
        else
            timer -= Time.deltaTime;
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
