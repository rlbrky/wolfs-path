using UnityEngine;

public abstract class Bomber_Informer : BaseState<Bomber_StateMachine.BomberState>
{
    protected Bomber_Context _context;

    public Bomber_Informer(Bomber_Context context, Bomber_StateMachine.BomberState stateKey) : base(stateKey)
    {
        _context = context;
    }
}
