using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; set; }

    [Header("Setup")]
    public Rigidbody rb;
    public CapsuleCollider playerCollider;
    public PlayerInputs playerInputs;
    public Animator animator;
    public Transform carryItemLoc;
    //public GameObject torchGO;
    public PauseMenu pauseMenu;

    [Header("Stats")]
    public float speed;
    public float jumpForce;
    public float dashCD;
    public float dashSpeed;
    public float slideForce;
    public float yVelMax;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool hasDoubleJump;
    private float ogSpeed;

    //Movement
    [HideInInspector] public Vector2 playerInput;
    [HideInInspector] public bool canMove = true;

    //Collider
    [HideInInspector] public Vector3 colliderStartCoords;
    [HideInInspector] public float colliderStartHeight;
    [HideInInspector] public float colliderStartRadius;

    //Slide
    public VisualEffect slideDustVFX;
    private int slideAnimHash;
    [HideInInspector] public bool isSliding;

    //Climb
    [HideInInspector] public int climbAnimHash;
    [HideInInspector] public bool isClimbing;
    [HideInInspector] public GameObject blockingObj;
    [HideInInspector] public GameObject climbableObj;
    public GameObject climbinPos;
    public ClimbCheck climbCheckObj;
    private Vector3 delta;


    //Carry
    [HideInInspector] public bool isCarryingItem;
    [HideInInspector] public Carryable itemToCarry;

    //Crouch
    [Header("Crouch")]
    [SerializeField] private float crouchedSpeed;
    [SerializeField] private float crouchColliderHeight;
    [SerializeField] private Vector3 crouchColliderLoc;
    public bool isCrouched;
    private int crouchAnimHash;
    private int crouchMovAnimHash;

    //Torch Burn
    public Burnable burnable;
    public bool isSlowWalking;

    //GroundSFX
    public string groundType;

    //Ladder
    public bool isLadderClimbing;
    public float ladderClimbSpeed;
    public float ladderVerticalJumpSpeed;
    public LadderScript activeLadder;
    [HideInInspector] public int ladderAnimHash;
    private int ladderMovementAnimHash;

    //Health Upgrades
    public GameObject pickupUI;
    public HealthUpgrade healthUpgrade;

    //ManaUpgrades
    public ManaUpgrade manaUpgrade;

    //Save Points
    public SavePoint activeSavePoint;
    public SavePoint lastSaved_SavePoint;
    //TO DO - Save Point UI.

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInputs = new PlayerInputs();
        playerInputs.GeneralInputs.Enable();
        playerInputs.GeneralInputs.Jump.started += HandleJumping;
        playerInputs.GeneralInputs.Dash.started += HandleSlide;
        playerInputs.GeneralInputs.Interact.started += HandleInteract;
        playerInputs.GeneralInputs.Crouch.started += HandleCrouch;
        playerInputs.GeneralInputs.Pause.started += HandlePausing;
        colliderStartCoords = playerCollider.center;
        colliderStartHeight = playerCollider.height;
        colliderStartRadius = playerCollider.radius;

        slideAnimHash = Animator.StringToHash("Slide");
        climbAnimHash = Animator.StringToHash("Climb");

        crouchAnimHash = Animator.StringToHash("isCrouched");
        crouchMovAnimHash = Animator.StringToHash("crouchMovement");

        ladderAnimHash = Animator.StringToHash("ladderClimb");
        ladderMovementAnimHash = Animator.StringToHash("ladderMovement");

        ogSpeed = speed;

        //torchGO.SetActive(false);
    }
    
    private void Start()
    {
        var allSavePoints = FindObjectsOfType<SavePoint>();
        foreach (var savePoint in allSavePoints)
        {
            if (savePoint.ID == DataManager.Instance.GameData.lastSavePointID)
                lastSaved_SavePoint = savePoint;
        }
    }

    void Update()
    {
        HandleMovement();
        CheckForLadderClimb();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.y < yVelMax)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, yVelMax);

        if(!isGrounded && !isClimbing)
            CheckForClimb();

        if (canMove && !isLadderClimbing)
        {
            transform.LookAt(transform.position + new Vector3(playerInput.x, 0, 0), Vector3.up);
            rb.linearVelocity = new Vector3(playerInput.x * speed, rb.linearVelocity.y, 0);
        }
        else if (isSliding)
        {
            rb.linearVelocity = transform.forward.normalized * slideForce;
        }
        else if (isClimbing)
        {
            rb.linearVelocity = Vector3.zero;
            rb.MovePosition(transform.position + delta);
            delta = Vector3.zero;
        }
        else if(isLadderClimbing)
        {
            rb.linearVelocity = new Vector3(0, playerInput.y, 0) * ladderClimbSpeed;
        }
    }

    public void OnAnimatorMove()
    {
        if (isClimbing)
            delta = new Vector3(animator.deltaPosition.x * 4.5f, animator.deltaPosition.y * 2.5f, 0);
    }

    private void CheckForLadderClimb()
    {
        if (isLadderClimbing)
        {
            animator.SetFloat(ladderMovementAnimHash, playerInput.y);
            return;
        }

        if (activeLadder != null && playerInput.y > 0)
        {
            rb.linearVelocity = Vector3.zero;
            isLadderClimbing = true;
            rb.useGravity = false;
            transform.eulerAngles = new Vector3(0, 90, 0);
            //transform.LookAt(Vector3.right, Vector3.up);
            transform.position = new Vector3(activeLadder.transform.position.x, transform.position.y, transform.position.z);
            animator.SetBool(ladderAnimHash, isLadderClimbing);
            PlayerCombat.instance.Katana.SetActive(false);
        }
    }

    private void HandleMovement()
    {
        playerInput = playerInputs.GeneralInputs.Movement.ReadValue<Vector2>();

        if (playerInput.y > 0)
            PlayerCombat.instance.upPressed = true;
        else
            PlayerCombat.instance.upPressed = false;

        if (!canMove)
            return;

        if (isCrouched)
        {
            if (playerInput.x != 0)
                animator.SetFloat(crouchMovAnimHash, 1);
            else
                animator.SetFloat(crouchMovAnimHash, 0);
        }
        else
        {
            if (playerInput.x != 0)
                animator.SetBool("isMoving", true);
            else
                animator.SetBool("isMoving", false);
        }
    }

    private void HandleJumping(InputAction.CallbackContext context)
    {
        if (!canMove || isCarryingItem || isCrouched || isSlowWalking)
            return;

        if (isGrounded)
        {
            isGrounded = false;
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            AudioChooser.instance.PlayJumpSFX();
        }
        else if(hasDoubleJump)
        {
            hasDoubleJump = false;
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            AudioChooser.instance.PlayJumpSFX();
        }
        else if (isLadderClimbing)
        {
            transform.LookAt(transform.position + new Vector3(playerInput.x, 0, 0), Vector3.up);
            isLadderClimbing = false;
            animator.SetBool(ladderAnimHash, isLadderClimbing);
            rb.AddForce(new Vector3(playerInput.x * ladderVerticalJumpSpeed, 1f * jumpForce, 0), ForceMode.Impulse);
            AudioChooser.instance.PlayJumpSFX();
            PlayerCombat.instance.Katana.SetActive(true);
        }
    }

    //private void WaitForClimbInput()
    //{
    //    playerInput = playerInputs.GeneralInputs.Movement.ReadValue<Vector2>();
    //    if(playerInput.y > 0)
    //    {
    //        //Vault();
    //    }
    //}

    private void HandleSlide(InputAction.CallbackContext context)
    {
        if (!canMove || isCarryingItem || isSlowWalking) return;

        if (isGrounded && playerInput.y < 0)
        {
            canMove = false;
            playerCollider.center = new Vector3(0, -0.8f, 0);
            playerCollider.height = 1.3f;
            animator.SetTrigger(slideAnimHash);
            isSliding = true;
            slideDustVFX.Play();
            AudioChooser.instance.PlaySlideSFX();
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (!isCarryingItem && itemToCarry != null)
        {
            isCarryingItem = true;
            animator.SetLayerWeight(1, 1);
            itemToCarry.transform.parent = carryItemLoc;
            itemToCarry.transform.position = carryItemLoc.position;
            itemToCarry.rb.useGravity = false;
            itemToCarry.main.enabled = false;
            InteractUI_SC.instance.HideInteractImage();
        }
        else if(itemToCarry != null)
        {
            isCarryingItem = false;
            animator.SetLayerWeight(1, 0);
            itemToCarry.rb.useGravity = true;
            itemToCarry.transform.parent = null;
            itemToCarry.main.enabled = true;
            itemToCarry = null;
        }

        //if (torchGO.activeInHierarchy && burnable != null)
        //{
        //    canMove = false;
        //    animator.SetLayerWeight(2, 0);
        //    animator.SetTrigger("Burn");
        //    burnable.Burn();
        //}

        if (healthUpgrade != null)
        {
            Destroy(healthUpgrade.gameObject);
            healthUpgrade = null;
            pickupUI.SetActive(false);
            PlayerHealth.instance.CollectHealthFragment();
        }

        if(manaUpgrade != null)
        {
            Destroy(manaUpgrade.gameObject); 
            manaUpgrade = null;
            pickupUI.SetActive(false);
            PlayerMana.instance.CollectManaFragment();
        }

        if(activeSavePoint != null)
        {
            activeSavePoint.InteractWithSavePoint();
        }
    }

    private void HandleCrouch(InputAction.CallbackContext context)
    {
        if (!canMove || isCarryingItem || isSlowWalking) return;

       if (!isCrouched)
        {
            isCrouched = true;
            speed = crouchedSpeed;
            animator.SetBool(crouchAnimHash, true);
            playerCollider.height = crouchColliderHeight;
            playerCollider.center = crouchColliderLoc;
        }
        else
        {
            isCrouched = false;
            speed = ogSpeed;
            animator.SetBool(crouchAnimHash, false);
            playerCollider.height = colliderStartHeight;
            playerCollider.center = colliderStartCoords;
        }
    }

    private void HandlePausing(InputAction.CallbackContext context)
    {
        pauseMenu.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    private void CheckForClimb()
    {
        if(climbableObj != blockingObj)
        {
            Vector3 moveTo = climbCheckObj.GetComponent<Collider>().ClosestPointOnBounds(climbableObj.transform.position);
            //climbinPos.transform.position = climbCheckObj.GetComponent<Collider>().ClosestPointOnBounds(climbableObj.transform.position);
            if(transform.forward.normalized.x == 1)
                transform.position = new Vector3(transform.position.x + (playerCollider.radius), moveTo.y + (playerCollider.height / 2), moveTo.z);
            else
                transform.position = new Vector3(transform.forward.x + transform.position.x + (playerCollider.radius), moveTo.y + (playerCollider.height / 2), moveTo.z);
            canMove = false;
            rb.useGravity = false;
            animator.SetTrigger(climbAnimHash);
            isClimbing = true;
        }
    }

    public void OnReturnToMainMenu()
    {
        playerInputs.GeneralInputs.Jump.started -= HandleJumping;
        playerInputs.GeneralInputs.Dash.started -= HandleSlide;
        playerInputs.GeneralInputs.Interact.started -= HandleInteract;
        playerInputs.GeneralInputs.Crouch.started -= HandleCrouch;
        playerInputs.GeneralInputs.Pause.started -= HandlePausing;
        playerInputs.GeneralInputs.Disable();
    }

    //public void Vault()
    //{
    //    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + (colliderStartHeight / 2)), transform.forward, out var firstHit, 1f, VaultLayer))
    //    {
    //        print("vaultable in front");
    //        if (Physics.Raycast(firstHit.point + (transform.forward * 1.5f) + (Vector3.up * (colliderStartHeight/2)), Vector3.down, out var secondHit, colliderStartHeight))
    //        {
    //            Debug.DrawRay(firstHit.point + (transform.forward * colliderStartRadius) + (Vector3.up * (colliderStartHeight / 2)), Vector3.down, Color.black, 9999f);
    //            print("found place to land");
    //            StartCoroutine(LerpVault(secondHit.point, 0.5f));
    //        }
    //    }

    //    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + (colliderStartHeight / 2)), transform.forward, Color.red, 9999f);
    //}

    //IEnumerator LerpVault(Vector3 targetPosition, float duration)
    //{
    //    float time = 0;
    //    Vector3 startPosition = transform.position;

    //    while (time < duration)
    //    {
    //        transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //    transform.position = targetPosition;
    //    rb.useGravity = true;
    //}
}
