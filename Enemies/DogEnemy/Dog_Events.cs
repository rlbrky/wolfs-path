using System.Collections;
using UnityEngine;

public class Dog_Events : MonoBehaviour, IEnemyEvents
{
    public Dog_Context context;

    public void AttackComplete()
    {
        if (context.PlayerCollider)
        {
            context.SwitchState(Dog_StateMachine.DogState.Chase);
        }
    }

    public void GetHit(Vector3 dir, float knockbackForce, int damageType, float damageAmount)
    {
        context._Rigidbody.linearVelocity = Vector3.zero;
        context.CurrentHealth -= damageAmount;
        AudioChooser.instance.PlayRandomHitSFX();
        context.ImpulseSource.GenerateImpulse(new Vector3(0.5f, 0));
        context.BloodVFX.Play();

        if (context.IsGrounded)
        {
            context._Rigidbody.AddForce(dir * knockbackForce, ForceMode.Impulse);
            switch (damageType)
            {
                case 0:
                    context._Animator.SetTrigger("GetHit");
                    break;
                case 1:
                    context._Animator.SetTrigger("HeavyGetHit");
                    break;
            }
        }

        if (context.CurrentHealth <= 0)
        {
            //Play Death Animation.
        }
        context.SwitchState(Dog_StateMachine.DogState.Idle);
    }
}
