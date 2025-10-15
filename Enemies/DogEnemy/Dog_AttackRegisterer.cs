using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_AttackRegisterer : MonoBehaviour
{
    public Dog_Context context;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerCombat.instance.GetHit(0, context.Damage, context.StateMachine.transform.forward);
        }
        if (other.tag == "ParryCol")
        {
            //context.StateMachine.GetParried();
        }
    }
}
