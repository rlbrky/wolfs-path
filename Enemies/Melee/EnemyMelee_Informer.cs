using UnityEngine;

public abstract class EnemyMelee_Informer : BaseState<Melee_StateMachine.EnemyState>
{
    protected EnemyMelee_Context context;

    public EnemyMelee_Informer(EnemyMelee_Context context, Melee_StateMachine.EnemyState stateKey) : base(stateKey)
    {
        this.context = context;
    }
}
