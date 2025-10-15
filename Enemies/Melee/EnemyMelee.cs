using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyMelee : MonoBehaviour
{
    [Header("General")]
    [SerializeField] public Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] public GameObject attackRegisterer;
    [SerializeField] public GameObject executionZone;
    [SerializeField] public GameObject canExecute;
    [SerializeField] public CapsuleCollider leCollider;
    [SerializeField] public VisualEffect bloodVFX;
    [SerializeField] public CinemachineImpulseSource impulseSource;

    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] public float damage;
    [SerializeField] private float attackCD;
    [SerializeField] private float speed;
    [SerializeField] private float detectionRadius;

    [Header("Detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private int checkFrameSetup;

    private UnitHealth meleeHealth;
    private Collider player;

    private bool isGettingExecuted;
    public Vector3 executionOffset;
    
    public Vector3 offset; //Air combo location for enemy
    public bool shouldKnock; //Should knock player
    public bool isGrounded;
    public bool gotThrownToAir;

    public bool TEST;
    
    private float attackCD_Counter;
    private float currentHealth;
    private int attackHash; //Animator hash

    private void Awake()
    {
        currentHealth = maxHealth;
        meleeHealth = new UnitHealth(currentHealth, maxHealth);
        attackHash = Animator.StringToHash("Attack");

        executionZone.SetActive(false);

        attackRegisterer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovement.instance.rb.linearVelocity.y < 0)
        {
            gotThrownToAir = false;
        }

        if (gotThrownToAir)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = new Vector3(PlayerMovement.instance.transform.position.x + offset.x, PlayerMovement.instance.transform.position.y, transform.position.z);
        }

        //Stun enemy
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") || animator.GetCurrentAnimatorStateInfo(0).IsName("Stunned")
            || gotThrownToAir || isGettingExecuted) return;

        //FOR TESTING WILL BE DELETED.
        if (TEST)
            return;

        if(attackCD_Counter < 0)
        {
            CheckForPlayer();
            ChasePlayer();
        }
        else
            attackCD_Counter -= Time.deltaTime;
    }

    private void CheckForPlayer()
    {
        if (Time.frameCount % checkFrameSetup == 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, whatIsPlayer);
            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    player = collider;
                    break;
                }
            }
        }
    }

    private void ChasePlayer()
    {
        if(player != null)
        {
            attackRegisterer.SetActive(true);
            animator.SetTrigger(attackHash);
            attackCD_Counter = attackCD;
        }
    }

    public void GetHit(Vector3 dir, float knockbackForce, int damageType)
    {
        rb.linearVelocity = Vector3.zero;

        AudioChooser.instance.PlayRandomHitSFX();
        impulseSource.GenerateImpulse(new Vector3(0.5f, 0));
        bloodVFX.Play();

        if (PlayerCombat.instance.shouldThrow)
        {
            animator.SetTrigger("getThrown");
            gotThrownToAir = true;
            offset.x = PlayerMovement.instance.transform.forward.x * 2.5f;

            rb.useGravity = false;
            StartCoroutine(ResetGravUsage());

            return;
        }

        if (isGrounded)
        {
            rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            switch (damageType)
            {
                case 0:
                    animator.SetTrigger("GetHit");
                    break;
                case 1:
                    animator.SetTrigger("HeavyGetHit");
                    break;
            }
        }
        else
        {
            offset.x = PlayerMovement.instance.transform.forward.x * 2.5f;
            animator.SetTrigger("getHit_Air");
            if(PlayerCombat.instance.knockEnemyDown)
                rb.AddForce(Vector3.down * PlayerCombat.instance.airFinisherForce, ForceMode.Impulse);
        }
        
        attackCD_Counter = attackCD;
    }

    public void GetExecuted(int random)
    {
        PlayerCombat.instance.enemyToExecute = null;
        rb.useGravity = false;
        //leCollider.enabled = false;
        transform.position = new Vector3(PlayerMovement.instance.transform.position.x + executionOffset.x, transform.position.y, transform.position.z);
        isGettingExecuted = true;
        executionZone.SetActive(false);
        canExecute.SetActive(false);

        switch (random)
        {
            case 0:
                animator.SetTrigger("getExecuted1");
                break;
            case 1:
                animator.SetTrigger("getExecuted2");
                break;
            case 2:
                animator.SetTrigger("getExecuted3");
                break;
            case 3:
                animator.SetTrigger("getExecuted4");
                break;
            case 4:
                animator.SetTrigger("getExecuted5");
                break;
            case 5:
                animator.SetTrigger("getExecuted6");
                break;
        }
    }

    public void GetParried()
    {
        animator.SetTrigger("getParried");
        attackRegisterer.SetActive(false);
        executionZone.SetActive(true);

        StartCoroutine(PlayerCombat.instance.ParryCoroutine(0.3f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void AttacksShouldKnock()
    {
        shouldKnock = true;
    }

    public void CancelAttacksShouldKnock()
    {
        shouldKnock = false;
    }

    IEnumerator ResetGravUsage()
    {
        yield return new WaitForSeconds(0.2f);
        rb.useGravity = true;
    }
}
