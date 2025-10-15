using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRegister : MonoBehaviour
{
    public EnemyMelee_Context context;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (context.ShouldKnock)
            {
                PlayerCombat.instance.GetHit(1, context.Damage, context.StateMachine.transform.forward);
            }
            else
                PlayerCombat.instance.GetHit(0, context.Damage, context.StateMachine.transform.forward);
            
            gameObject.SetActive(false);
        }
        if(other.tag == "ParryCol")
        {
            context.StateMachine.GetParried();
            gameObject.SetActive(false);
        }
    }
}
