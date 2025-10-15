using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class PlayerAnimEvents : MonoBehaviour
{
    public static PlayerAnimEvents instance { get; set; }

    [SerializeField] GameObject attackHitbox;
    public GameObject swordSlashVFX;
    public VisualEffect bloodVFX;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        attackHitbox.SetActive(false);
        swordSlashVFX.gameObject.SetActive(false);
    }

    public void ParryWindowStart()
    {
        PlayerCombat.instance.parryCollider.SetActive(true);
    }

    public void ParryWindowEnd()
    {
        PlayerCombat.instance.parryCollider.SetActive(false);
        PlayerMovement.instance.playerCollider.center = PlayerMovement.instance.colliderStartCoords;
    }

    public void PlaySwingSFX()
    {
        AudioChooser.instance.PlaySwingSFX();
        //swordSlashVFX.Play();
    }

    public void PlayHitSFX()
    {
        AudioChooser.instance.PlayRandomHitSFX();
        PlayerCombat.instance.hitEffect.Play();
        bloodVFX.Play();
    }


    public void AttackHitboxEvent()
    {
        attackHitbox.SetActive(true);
    }

    public void AttackHitboxEndEvent()
    {
        attackHitbox.SetActive(false);
        PlayerCombat.instance.shouldKnock = false;
        PlayerCombat.instance.shouldThrow = false;
    }

    public void AttackKnock()
    {
        PlayerCombat.instance.shouldKnock = true;
    }

    public void AttackAirThrow()
    {
        PlayerCombat.instance.shouldThrow = true;
    }

    public void PlayerCanMove()
    {
        PlayerCombat.instance.gotHit = false;
        PlayerCombat.instance.ResetAttackStuff();
        swordSlashVFX.gameObject.SetActive(false);
        PlayerMovement.instance.canMove = true;
    }

    public void FinishCombo()
    {
        PlayerCombat.instance.ResetTriggers();
    }

    public void AirEvents()
    {
        PlayerMovement.instance.rb.useGravity = true;
    }

    public void AirComboEnderStart()
    {
        PlayerCombat.instance.airComboEnded = true;
        PlayerCombat.instance.airComboCoroutine = StartCoroutine(PlayerCombat.instance.CheckForAirComboEnd());
        PlayerCombat.instance.knockEnemyDown = false;
    }

    public void KnockEnemyAir()
    {
        PlayerCombat.instance.knockEnemyDown = true;
    }

    public void AirStartEvent()
    {
        //PlayerMovement.instance.rb.velocity = Vector3.up * ((PlayerMovement.instance.jumpForce * 3) / 4);
        PlayerMovement.instance.rb.AddForce(Vector3.up * ((PlayerMovement.instance.jumpForce * 3) / 4), ForceMode.Impulse);
    }

    public void AirComboEnd()
    {
        if (DashScript.instance != null)
            DashScript.instance.canDash = true;
    }

    public void DashEndEvent()
    {
        //PlayerMovement.instance.rb.useGravity = true;
        //PlayerMovement.instance.canMove = true;
        //Physics.IgnoreLayerCollision(6, 7, false);
    }

    public void SlideEndEvent()
    {
        PlayerMovement.instance.playerCollider.center = PlayerMovement.instance.colliderStartCoords;
        PlayerMovement.instance.playerCollider.height = PlayerMovement.instance.colliderStartHeight;
        PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
        PlayerMovement.instance.canMove = true;
        PlayerMovement.instance.isSliding = false;
        PlayerMovement.instance.slideDustVFX.Stop();
    }

    public void ClimbEnd()
    {
        PlayerMovement.instance.rb.useGravity = true;
        PlayerMovement.instance.isClimbing = false;
        PlayerMovement.instance.canMove = true;
    }

    public void BowAttackStart()
    {
        PlayerCombat.instance.Bow_InHand.SetActive(true);
        PlayerCombat.instance.FireArrow();
    }

    public void BowAttackEnd()
    {
        PlayerCombat.instance.Bow_InHand.SetActive(false);
        PlayerCombat.instance.Back_Bow.SetActive(true);
        PlayerCombat.instance.Katana.SetActive(true);
        PlayerMovement.instance.canMove = true;
    }

    public void TorchEquip()
    {
        PlayerMovement.instance.animator.SetLayerWeight(2, 1);
    }

    public void MovementSFX()
    {
        AudioChooser.instance.PlayGroundSFX(PlayerMovement.instance.groundType);
    }

    public void LandingSFX()
    {
        AudioChooser.instance.PlayLandingSFX();
    }
}
