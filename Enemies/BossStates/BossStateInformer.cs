using UnityEngine;

public abstract class BossStateInformer : BaseState<BossStateMachine.BossState>
{
    protected BossContext Context;

    public BossStateInformer(BossContext context, BossStateMachine.BossState stateKey) : base(stateKey)
    {
        Context = context;
    }
}
