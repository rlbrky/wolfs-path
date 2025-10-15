using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashScript : MonoBehaviour
{
    
    public static DashScript instance { get; private set; }
    public bool canDash = true;
    public LayerMask layerToWork;

    public float dashMaxDistance;

    // salih ekledi
    [SerializeField] CharacterTrailEffectSc _trailVfx;

    private int dashAnimHash;
    private Vector3 delta;

    private Coroutine dashRoutine;
    private float dashEndPos;

    private int playerLayer;
    private int enemyLayer;
    private int bombLayer;

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

        dashAnimHash = Animator.StringToHash("Dash");
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        bombLayer = LayerMask.NameToLayer("Bomb");
    }

    void Start()
    {
        PlayerMovement.instance.playerInputs.GeneralInputs.Dash.started += HandleDash;
    }

    //private void FixedUpdate()
    //{
    //    if (!canDash)
    //    {
    //        PlayerMovement.instance.rb.MovePosition(transform.position + delta);
    //    }
    //    delta = Vector3.zero;
    //}

    //public void OnAnimatorMove()
    //{
    //    if (!canDash)
    //    {
    //        delta = new Vector3(PlayerMovement.instance.animator.deltaPosition.x, 0) * PlayerMovement.instance.dashSpeed;
    //    }
    //}

    private void HandleDash(InputAction.CallbackContext context)
    {
        if (!PlayerMovement.instance.canMove || PlayerMovement.instance.isCarryingItem || PlayerCombat.instance.isAttacking || PlayerMovement.instance.isCrouched) return;

        if (PlayerMovement.instance.playerInput.y >= 0 && canDash && !PlayerCombat.instance.isAttacking)
        {
            dashRoutine = StartCoroutine(DashCoroutine());
            _trailVfx.StartEffect();
        }
    }

    private void CheckForWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 1.3f, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.3f, layerToWork))
        {
            PlayerMovement.instance.rb.useGravity = true;
            PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
            PlayerMovement.instance.canMove = true;
            Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            StopCoroutine(dashRoutine);
            StartCoroutine(StartDashCD());
            SecureDashBug();
            //if(hit.collider.tag == "Wall")
            //{
            //    PlayerMovement.instance.rb.useGravity = true;
            //    PlayerMovement.instance.canMove = true;
            //    Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            //    StopCoroutine(dashRoutine);
            //    StartCoroutine(StartDashCD());
            //}
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        Physics.IgnoreLayerCollision(playerLayer, bombLayer, true);
        dashEndPos = transform.position.x + (dashMaxDistance * transform.forward.x);
        PlayerMovement.instance.animator.SetTrigger(dashAnimHash);
        //Play VFX.
        PlayerMovement.instance.rb.useGravity = false;
        PlayerMovement.instance.canMove = false;
        PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
        while(Mathf.Abs(dashEndPos - transform.position.x) > 0.2f)
        {
            PlayerMovement.instance.rb.linearVelocity = new Vector3(transform.forward.x * PlayerMovement.instance.dashSpeed, PlayerMovement.instance.rb.linearVelocity.y, 0);
            CheckForWall();
            yield return null;
        }
        //PlayerMovement.instance.rb.AddForce(transform.forward * PlayerMovement.instance.dashSpeed, ForceMode.Impulse);
        PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
        PlayerMovement.instance.rb.useGravity = true;
        PlayerMovement.instance.canMove = true;
        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, bombLayer, false);
        yield return new WaitForSeconds(PlayerMovement.instance.dashCD);
        canDash = true;
    }

    private IEnumerator StartDashCD()
    {
        yield return new WaitForSeconds(PlayerMovement.instance.dashCD);
        canDash = true;
    }

    private IEnumerator SecureDashBug()
    {
        yield return new WaitForSeconds(PlayerMovement.instance.dashCD + 0.5f);
        canDash = true;
        StopAllCoroutines();
    }
}
