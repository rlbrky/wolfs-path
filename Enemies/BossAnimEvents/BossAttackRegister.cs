using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackRegister : MonoBehaviour
{
    public BossContext context;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerCombat.instance.GetHit(context.DamageType, context.StateMachine.damage, context.StateMachine.transform.forward);
        }
    }
}
