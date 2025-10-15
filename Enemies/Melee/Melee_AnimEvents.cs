using System.Collections;
using UnityEngine;

public class Melee_AnimEvents : MonoBehaviour, IEnemyEvents
{
    public EnemyMelee_Context context;

    [SerializeField] GameObject attackHitbox;

    private void Start()
    {
        context._Rigidbody.useGravity = true;
        context.OwnCollider.enabled = true;
        attackHitbox.gameObject.SetActive(false);
    }

    public void ShouldKnock()
    {
        context.ShouldKnock = true;
    }

    public void CancelShouldKnock()
    {
        context.ShouldKnock = false;
    }

    public void ActivateHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void CloseHitbox()
    {
        attackHitbox.SetActive(false);
    }

    public void AttackComplete()
    {
        context.SetState(Melee_StateMachine.EnemyState.Idle);
    }

    public void GetHit(Vector3 dir, float knockbackForce, int damageType, float damageAmount)
    {
        PlayerCombat.instance.HitStop(PlayerCombat.instance.stopDuration);
        context._Rigidbody.linearVelocity = Vector3.zero;
        context.EnemyHealth.DamageUnit(damageAmount);

        AudioChooser.instance.PlayRandomHitSFX();
        context.ImpulseSource.GenerateImpulse(new Vector3(0.5f, 0));
        context.BloodVFX.Play();

        if (PlayerCombat.instance.shouldThrow)
        {
            context.GotAirHit = true;
            context._Animator.SetTrigger("getThrown");
            context.GotThrown = true;
            context.AirOffset = new Vector3(PlayerMovement.instance.transform.forward.x * 2.5f, 0);

            context._Rigidbody.useGravity = false;
            StartCoroutine(ResetGravUsage());

            return;
        }

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
        else
        {
            context.GotAirHit = true;
            context.AirOffset = new Vector3(PlayerMovement.instance.transform.forward.x * 2.5f, 0);
            context._Animator.SetTrigger("getHit_Air");
            if (PlayerCombat.instance.knockEnemyDown)
                context._Rigidbody.AddForce(Vector3.down * PlayerCombat.instance.airFinisherForce, ForceMode.Impulse);
        }

        context.IsAttacking = false;
        context.IsStunned = true;
        if (context.EnemyHealth.Health <= 0)
        {
            context._Animator.SetTrigger("Die");
            context.StateMachine.TEST = true;
            StartCoroutine(DeathRoutine());
        }
        context.SetState(Melee_StateMachine.EnemyState.Idle);
    }

    public void TriggerDeathRoutine()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator ResetGravUsage()
    {
        yield return new WaitForSeconds(0.2f);
        context._Rigidbody.useGravity = true;
    }

    IEnumerator DeathRoutine()
    {
        PlayerCombat.instance.killedEnemies.Add(context.StateMachine);
        context._Rigidbody.useGravity = false;
        context.OwnCollider.enabled = false;
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
}
