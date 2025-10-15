using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class Boss1_AnimEvents : MonoBehaviour, IEnemyEvents, IBossEvents
{
    [SerializeField] float timeBetweenRocks = 0.3f;
    [SerializeField] Transform boundry1;
    [SerializeField] Transform boundry2;
    [SerializeField] GameObject attackHitbox;
    [SerializeField] GameObject kick_AttackHitbox;
    [SerializeField] GameObject kick_DistortionObj;

    [Header("Prefab")]
    [SerializeField] GameObject rock;

    [Header("VFX")]
    [SerializeField] private ParticleSystem groundSmash;
    [SerializeField] private ShiftColorForDMG getHitEffect;

    public BossContext _context;
    private Vector3 spawnLoc;

    private void Awake()
    {
        attackHitbox.SetActive(false);
        kick_AttackHitbox.SetActive(false);
    }

    private void Start()
    {
        _context.BossEventHandler = this;
        _context.Healthbar.maxValue = _context.CurrentHealth;
        _context.Healthbar.value = _context.CurrentHealth;
    }

    public void TriggerRockFall()
    {
        int random = Random.Range(3, 7);
        StartCoroutine(SpawnRocks(random));
    }

    public void MovementEnd()
    {
        _context.movementComplete = true;
    }

    public void EndAttack()
    {
        _context.isAttacking = false;
        _context.endAttackState = true;
    }

    public void Open_AttackHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void Open_KickHitbox()
    {
        kick_AttackHitbox.SetActive(true);
    }

    public void Close_KickHitbox()
    {
        kick_AttackHitbox.SetActive(false);
    }

    public void Close_AttackHitbox()
    {
        attackHitbox.SetActive(false);
    }

    public void CheckPlayerStart()
    {
        _context.CheckingForPlayer = true;
    }

    public void CheckPlayerEnd()
    {
        _context.CheckingForPlayer = false;
    }

    public void FollowupEND()
    {
        _context.animator.SetBool("Combo1_Followup", false);
    }

    public void PlayGroundSmashVFX()
    {
        //groundSmash.Play();
        groundSmash.gameObject.SetActive(true);
    }

    public void CloseGroundSmashVFX()
    {
        groundSmash.gameObject.SetActive(false);
    }

    public void ShouldKnockPlayer()
    {
        _context.DamageType = 1;
    }

    public void CancelKnockEffect()
    {
        _context.DamageType = 0;
    }

    public void ActivateKickDistortion()
    {
        kick_DistortionObj.SetActive(true);
    }

    public void DeactivateKickDistortion()
    {
        kick_DistortionObj.SetActive(false);
    }

    public void CheckComboFollowUp()
    {
        StartCoroutine(CheckForPlayer());
    }

    IEnumerator SpawnRocks(int amount)
    {
        float time = 0;

        for (int i = 0; i < amount; i++)
        {
            spawnLoc.x = Random.Range(boundry1.position.x, boundry2.position.x);
            spawnLoc.y = boundry1.position.y;
            Instantiate(rock, spawnLoc, Quaternion.identity);
            while (time < timeBetweenRocks)
            {
                //Example of how to use timing.
                //transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
    }

    private IEnumerator CheckForPlayer()
    {
        while (_context.isAttacking)
        {
            if (_context.CheckingForPlayer && Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), transform.forward, out RaycastHit hit, 5f))
            {
                if(hit.collider.tag == "Player")
                {
                    _context.endAttackState = false;
                    _context.animator.SetBool("Combo1_Followup", true);
                    break;
                }
            }
            yield return null;
        }
    }

    public void GetHit(Vector3 dir, float knockbackForce, int damageType, float damageAmount)
    {
        PlayerCombat.instance.HitStop(PlayerCombat.instance.stopDuration);
        AudioChooser.instance.PlayRandomHitSFX();
        getHitEffect.PlayEffect();
        _context.CurrentHealth -= damageAmount;
        _context.Healthbar.value = _context.CurrentHealth;
        if(_context.CurrentHealth <= 0)
        {
            _context.animator.SetTrigger("Die");
            Destroy(_context.StateMachine);
            StartCoroutine(DeathRoutine());
        }
    }

    IEnumerator DeathRoutine()
    {
        _context.Rb.useGravity = false;
        Destroy(_context.OwnCollider);
        yield return new WaitForSeconds(3);
        _context.DestroyWalls();
        Destroy(gameObject);
    }

    public void ResetBoss()
    {
        transform.position = _context.BossSpawnLoc;
        _context.PlayerCollider = null;
        _context.CurrentHealth = _context.MaxHealth;
        _context.Healthbar.value = _context.CurrentHealth;
        _context.Healthbar.gameObject.SetActive(false);
        _context.DeactivateWalls();
    }
}
