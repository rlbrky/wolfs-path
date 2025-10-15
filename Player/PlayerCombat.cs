using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerCombat : MonoBehaviour, ISaveManager
{
    public static PlayerCombat instance { get; set; }

    private int comboCount = 0;
    //private bool hasBow;

    [Header("General")]
    public GameObject ArrowPrefab;
    public GameObject Back_Bow;
    public GameObject Bow_InHand;
    public GameObject Katana;
    public Transform arrowSpawnLoc;

    [Header("Time Stop")]
    public float stopDuration;
    public bool waiting;

    [Header("Stats")]
    public float knockbackForce;
    public float comboWindow;
    public float arrowSpeed;
    public float damage;

    [Header("VFX")]
    public ParticleSystem hitEffect;

    [Header("Parry")]
    public GameObject parryCollider;
    public Vector3 executionCamOffset;

    private Vector3 cameraStartPos;
    public CinemachineTransposer cinemachineTransposer;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool shouldKnock;
    [HideInInspector] public bool gotHit;
    public bool shouldThrow;

    private int attackHash1;
    private int attackHash2;
    private int attackHash3;
    private int attackHash4;

    private int airStarterHash;
    private int airAttackHash1;
    private int airAttackHash2;
    private int airAttackHash3;

    private int bowAttackHash;
    private int bowAirAttackHash;

    private Coroutine currentCoroutine;
    private bool comboWindowOpen;
    private Vector3 delta;
    //Testing stuff
    private float exectuionTimer;
    private bool isExecuting;

    //Air combo
    [Header("Air Combo")]
    public bool airComboEnded;
    public bool knockEnemyDown;
    public float airFinisherForce;
    public Coroutine airComboCoroutine;

    //Arrow to Teleport
    [HideInInspector] public ArrowScript prevArrow;


    public List<IEnemy> killedEnemies = new List<IEnemy>();
    [HideInInspector] public bool upPressed;
    [HideInInspector] public IEnemy enemyToExecute;
    
    //Bug fix
    private Coroutine attackBugFixCoroutine;

    public void LoadData(GameData data)
    {
        //hasBow = data.hasBow;
    }

    public void SaveData(GameData data)
    {
        //data.hasBow = hasBow;
    }


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

        attackHash1 = Animator.StringToHash("Attack1");
        attackHash2 = Animator.StringToHash("Attack2");
        attackHash3 = Animator.StringToHash("Attack3");
        attackHash4 = Animator.StringToHash("Attack4");

        airStarterHash = Animator.StringToHash("ThrowToAirAttack");
        airAttackHash1 = Animator.StringToHash("AirAttack1");
        airAttackHash2 = Animator.StringToHash("AirAttack2");
        airAttackHash3 = Animator.StringToHash("AirAttack3");

        bowAirAttackHash = Animator.StringToHash("BowAir");
        bowAttackHash = Animator.StringToHash("BowAttack");

        //if(hasBow)
        //    Back_Bow.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraStartPos = cinemachineTransposer.m_FollowOffset;
        parryCollider.SetActive(false);

        Bow_InHand.SetActive(false);

        PlayerMovement.instance.playerInputs.GeneralInputs.Attack.started += HandleAttacking;
        PlayerMovement.instance.playerInputs.GeneralInputs.Attack.started += HandleExecution;
        PlayerMovement.instance.playerInputs.GeneralInputs.Parry.started += HandleParry;
        PlayerMovement.instance.playerInputs.GeneralInputs.Special.started += HandleSpecial;
        PlayerMovement.instance.playerInputs.GeneralInputs.BowAttack.started += HandleBowAttack;
    }

    // Update is called once per frame
    void Update()
    {
        if(isExecuting)
        {
            exectuionTimer -= Time.deltaTime;

            if(exectuionTimer < 0 )
            {
                isExecuting = false;

                PlayerMovement.instance.rb.useGravity = true;
                PlayerMovement.instance.playerCollider.enabled = true;
            }
        }

        //if (PlayerMovement.instance.animator.IsInTransition(0) || isAttacking || isExecuting || airComboEnded)
        //    return;
    }

    private void FixedUpdate()
    {
        if (gotHit) return;
        
        if (isAttacking || isExecuting)
                PlayerMovement.instance.rb.MovePosition(transform.position + delta);
        delta = Vector3.zero;
    }

    public void OnAnimatorMove()
    {
        if (PlayerMovement.instance.isGrounded && !shouldThrow)
        {
            if (isAttacking)
            {
                delta = new Vector3(PlayerMovement.instance.animator.deltaPosition.x, PlayerMovement.instance.animator.deltaPosition.y, 0) * 1.2f;
            }

            if (comboCount == 3 && isAttacking)
                delta = new Vector3(PlayerMovement.instance.animator.deltaPosition.x, PlayerMovement.instance.animator.deltaPosition.y, 0) * 2;

            if (isExecuting)
            {
                delta = new Vector3(PlayerMovement.instance.animator.deltaPosition.x, PlayerMovement.instance.animator.deltaPosition.y, 0);
            }
        }
    }

    private void HandleAttacking(InputAction.CallbackContext context)
    {
        if (PlayerMovement.instance.animator.IsInTransition(0) || airComboEnded || shouldThrow || PlayerMovement.instance.isCarryingItem || PlayerMovement.instance.isCrouched || PlayerMovement.instance.isSlowWalking || PlayerMovement.instance.isLadderClimbing)
            return;

        PlayerAnimEvents.instance.swordSlashVFX.gameObject.SetActive(true);

        if (attackBugFixCoroutine != null)
        {
            StopCoroutine(attackBugFixCoroutine);
            attackBugFixCoroutine = StartCoroutine(ResetStuff());
        }
        else
            attackBugFixCoroutine = StartCoroutine(ResetStuff());
        
        if (!upPressed)
        {
            if (PlayerMovement.instance.isGrounded && !isAttacking)
            {
                StopMovementNAttack();
                PlayerMovement.instance.animator.SetTrigger(attackHash1);
                comboCount = 1;
            }
            else if (PlayerMovement.instance.isGrounded && comboWindowOpen)
            {
                StopMovementNAttack();
                switch (comboCount)
                {
                    case 1:
                        PlayerMovement.instance.animator.SetTrigger(attackHash2);
                        comboCount = 2;
                        break;
                    case 2:
                        PlayerMovement.instance.animator.SetTrigger(attackHash3);
                        comboCount = 3;
                        break;
                    case 3:
                        PlayerMovement.instance.animator.SetTrigger(attackHash4);
                        break;
                }
            }

            if (!PlayerMovement.instance.isGrounded && !isAttacking)
            {
                StopMovementNAttack();
                PlayerMovement.instance.rb.useGravity = false;
                PlayerMovement.instance.animator.SetTrigger(airAttackHash1);
                comboCount = 1;
            }
            else if (!PlayerMovement.instance.isGrounded && comboWindowOpen)
            {
                StopMovementNAttack();
                PlayerMovement.instance.rb.useGravity = false;
                switch (comboCount)
                {
                    case 1:
                        PlayerMovement.instance.animator.SetTrigger(airAttackHash2);
                        comboCount = 2;
                        break;
                    case 2:
                        PlayerMovement.instance.animator.SetTrigger(airAttackHash3);
                        if (DashScript.instance != null)
                            DashScript.instance.canDash = false;
                        break;
                }
            }
        }
        else if(upPressed && comboCount == 2)
        {
            ResetAttackStuff();
            ResetTriggers();
            if (PlayerMovement.instance.isGrounded)
            {
                StopMovementNAttack();
                //shouldThrow = true;
                PlayerMovement.instance.rb.useGravity = false;
                PlayerMovement.instance.animator.SetTrigger(airStarterHash);
            }
        }
    }

    private void HandleExecution(InputAction.CallbackContext context)
    {
        if (PlayerMovement.instance.isCrouched) return;

        if(enemyToExecute != null)
        {
            hitEffect.transform.position = transform.position + new Vector3(2 * transform.forward.x, 0, 0);
            isExecuting = true;
            exectuionTimer = 2.5f;
            PlayerMovement.instance.rb.useGravity = false;
            PlayerMovement.instance.playerCollider.enabled = false;
            PlayerMovement.instance.animator.SetBool("isMoving", false);
            PlayerMovement.instance.canMove = false;
            PlayerMovement.instance.rb.linearVelocity = Vector3.zero;

            int random = Random.Range(0, 6);
            switch (random)
            {
                case 0:
                    PlayerMovement.instance.animator.SetTrigger("Execute1");
                    enemyToExecute.executionOffset = new Vector3(2 * transform.forward.x, 0 ,0);
                    break;
                case 1:
                    PlayerMovement.instance.animator.SetTrigger("Execute2");
                    enemyToExecute.executionOffset = new Vector3(2 * transform.forward.x, 0, 0);
                    break;
                case 2:
                    PlayerMovement.instance.animator.SetTrigger("Execute3");
                    enemyToExecute.executionOffset = new Vector3(1 * transform.forward.x, 0, 0);
                    break;
                case 3:
                    PlayerMovement.instance.animator.SetTrigger("Execute4");
                    enemyToExecute.executionOffset = new Vector3(2 * transform.forward.x, 0, 0);
                    break;
                case 4:
                    PlayerMovement.instance.animator.SetTrigger("Execute5");
                    enemyToExecute.executionOffset = new Vector3(2 * transform.forward.x, 0, 0);
                    break;
                case 5:
                    PlayerMovement.instance.animator.SetTrigger("Execute6");
                    enemyToExecute.executionOffset = new Vector3(2 * transform.forward.x, 0, 0);
                    break;
            }
            StartCoroutine(ExecutionCoroutine(0.7f));
            enemyToExecute.GetExecuted(random);

            ResetAttackStuff();
            ResetTriggers();
        }    
    }

    private void HandleParry(InputAction.CallbackContext context)
    {
        if (!PlayerMovement.instance.isGrounded || isAttacking || PlayerMovement.instance.isCarryingItem || PlayerMovement.instance.isCrouched || PlayerMovement.instance.isSlowWalking || PlayerMovement.instance.isLadderClimbing) return;

        PlayerMovement.instance.animator.SetTrigger("Parry");
        PlayerMovement.instance.playerCollider.center = new Vector3(PlayerMovement.instance.playerCollider.center.x, PlayerMovement.instance.playerCollider.center.y, transform.forward.normalized.x * -0.5f);
    }

    private void HandleSpecial(InputAction.CallbackContext context)
    {
        if (!PlayerMovement.instance.isGrounded || isAttacking || PlayerMovement.instance.isCarryingItem || PlayerMovement.instance.isCrouched || PlayerMovement.instance.isSlowWalking || PlayerMovement.instance.isLadderClimbing) return;

        PlayerMovement.instance.animator.SetTrigger("Special");
    }

    private void HandleBowAttack(InputAction.CallbackContext context)
    {
        if (isAttacking || PlayerMovement.instance.isCarryingItem || PlayerMovement.instance.isCrouched || PlayerMovement.instance.isSlowWalking || PlayerMovement.instance.isLadderClimbing) return;

        if(prevArrow == null)
        {
            PlayerMovement.instance.canMove = false;
            Back_Bow.SetActive(false);
            Katana.SetActive(false);
            PlayerMovement.instance.animator.SetTrigger(bowAttackHash);
        }
        else
        {
            transform.position = prevArrow.transform.position;
            Destroy(prevArrow.gameObject);
            prevArrow = null;
        }
    }

    public void GetHit(int damageType, float incDamage, Vector3 dir)
    {
        HitStop(stopDuration);
        gotHit = true;
        isAttacking = false;
        PlayerHealth.instance.DamagePlayer(incDamage);
        PlayerMovement.instance.canMove = false;
        PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
        ResetTriggers();
        switch (damageType)
        {
            case 0:
                PlayerMovement.instance.animator.SetTrigger("GetHit");
                break;
            case 1:
                PlayerMovement.instance.animator.SetTrigger("HeavyGetHit");
                PlayerMovement.instance.rb.AddForce(dir * 20f, ForceMode.Impulse);
                break;
            case 2:
                PlayerMovement.instance.animator.SetTrigger("HeavyGetHit");
                PlayerMovement.instance.rb.AddForce(dir * 8f, ForceMode.Impulse);
                Bow_InHand.SetActive(false);
                Back_Bow.SetActive(true);
                Katana.SetActive(true);
                break;
        }
    }

    void StopMovementNAttack()
    {
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ResetAttack());
        isAttacking = true;
        PlayerMovement.instance.animator.SetBool("isMoving", false);
        PlayerMovement.instance.canMove = false;
        PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
    }
    
    public void ResetAttackStuff()
    {
        comboWindowOpen = false;
        comboCount = 0;
        isAttacking = false;
        shouldKnock = false;
        //PlayerMovement.instance.canMove = true;
    }

    public void ResetTriggers()
    {
        PlayerMovement.instance.animator.ResetTrigger(attackHash1);
        PlayerMovement.instance.animator.ResetTrigger(attackHash2);
        PlayerMovement.instance.animator.ResetTrigger(attackHash3);
        PlayerMovement.instance.animator.ResetTrigger(attackHash4);
    }

    public void FireArrow()
    {
        if(prevArrow == null)
        {
            var arrow = Instantiate(ArrowPrefab, arrowSpawnLoc.position, Quaternion.identity);
            prevArrow = arrow.GetComponent<ArrowScript>();
            prevArrow.direction = transform.forward;
            prevArrow.speed = arrowSpeed;
        }
    }

    public void ResetAll()
    {
        PlayerMovement.instance.animator.ResetTrigger(attackHash1);
        PlayerMovement.instance.animator.ResetTrigger(attackHash2);
        PlayerMovement.instance.animator.ResetTrigger(attackHash3);
        PlayerMovement.instance.animator.ResetTrigger(attackHash4);

        PlayerMovement.instance.animator.ResetTrigger(airStarterHash);
        PlayerMovement.instance.animator.ResetTrigger(airAttackHash1);
        PlayerMovement.instance.animator.ResetTrigger(airAttackHash2);
        PlayerMovement.instance.animator.ResetTrigger(airAttackHash3);

        comboWindowOpen = false;
        comboCount = 0;
        isAttacking = false;
        shouldKnock = false;
        PlayerMovement.instance.canMove = true;
    }

    public void ReviveKilledEnemies()
    {
        if (killedEnemies.Count != 0) 
        { 
            foreach (IEnemy enemy in killedEnemies)
            {
                enemy.Revive();
            }
            killedEnemies.Clear();
        }
    }

    public void HitStop(float stopDuration)
    {
        if (waiting)
            return;

        Time.timeScale = 0;
        StartCoroutine(Wait(stopDuration));
    }

    private IEnumerator ResetAttack()
    {
        comboWindowOpen = true;
        yield return new WaitForSeconds(comboWindow);
        ResetAttackStuff();
    }

    public IEnumerator ParryCoroutine(float duration)
    {
        Time.timeScale = 0.4f;
        float time = 0;

        AudioChooser.instance.PlayParrySFX();

        while (time < duration)
        {
            //Example of how to use timing.
            //transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
    }

    IEnumerator ExecutionCoroutine(float duration)
    {
        Time.timeScale = 0.2f;
        float time = 0;

        //groupTarget.position = transform.position + (transform.position - enemyToExecute.transform.position) / 2;
        //parryCam.transform.position = groupTarget.position;
        //parryCam.gameObject.SetActive(true);

        while (time < duration)
        {
            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, executionCamOffset, time / duration);
            //Example of how to use timing.
            //transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1f;

        time = 0;
        while(time < duration)
        {
            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, cameraStartPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        //parryCam.gameObject.SetActive(false);
    }

    public IEnumerator CheckForAirComboEnd()
    {
        while(true)
        {
            if(airComboEnded && PlayerMovement.instance.isGrounded && PlayerMovement.instance.animator.GetCurrentAnimatorStateInfo(0).IsTag("GroundMovement"))
            {
                airComboEnded = false;
                PlayerMovement.instance.canMove = true;
                break;
            }
            yield return null;
        }
        
        //PlayerMovement.instance.canMove = false;

        if (!airComboEnded)
        {
            StopCoroutine(airComboCoroutine);
            airComboCoroutine = null;
        }

        yield return null;
    }

    private IEnumerator ResetStuff()
    {
        yield return new WaitForSeconds(4f);
        ResetAll();
    }

    private IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        waiting = false;
    }

    private void OnDestroy()
    {
        PlayerMovement.instance.playerInputs.GeneralInputs.Attack.started -= HandleAttacking;
        PlayerMovement.instance.playerInputs.GeneralInputs.Attack.started -= HandleExecution;
        PlayerMovement.instance.playerInputs.GeneralInputs.Parry.started -= HandleParry;
        PlayerMovement.instance.playerInputs.GeneralInputs.Special.started -= HandleSpecial;
        PlayerMovement.instance.playerInputs.GeneralInputs.BowAttack.started -= HandleBowAttack;
    }
}
