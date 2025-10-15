using UnityEngine;

public abstract class Dog_Informer : BaseState<Dog_StateMachine.DogState>
{
    protected Dog_Context _context;

    public Dog_Informer(Dog_Context context, Dog_StateMachine.DogState stateKey) : base(stateKey)
    {
        _context = context;
    }
}
