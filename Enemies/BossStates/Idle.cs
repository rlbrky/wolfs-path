using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Idle : BossStateInformer
{
    private bool runningF;
    private float timer;
    private BossContext _context;

    public Idle(BossContext context, BossStateMachine.BossState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        timer = _context.IdlingTime;
        _context.movementComplete = true;
    }
    
    public override void UpdateState()
    {
        if (_context.PlayerCollider == null)
        {
            CheckForPlayer();
        }
        else
        {
            if (_context.movementComplete && _context.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                if (PlayerMovement.instance.transform.position.x - _context.Transform.position.x > 0) //Is the player on the left side or the right side?
                    _context.Transform.rotation = Quaternion.Euler(0, 90, 0);
                else
                    _context.Transform.rotation = Quaternion.Euler(0, -90, 0);

                _context.movementComplete = false;
                int random = Random.Range(0, 4);
                switch (random)
                {
                    case 0:
                        _context.animator.SetTrigger("shouldRunB");
                        runningF = false;
                        break;
                    case 1:
                        _context.animator.SetTrigger("shouldRunF");
                        runningF = true;
                        break;
                    default:
                        _context.SetState(BossStateMachine.BossState.Chasing);
                        GetNextState();
                        break;
                }
            }
            else if (_context.animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement"))
            {
                if (runningF)
                    _context.Rb.linearVelocity = _context.Transform.forward * _context.IdleMS;
                else
                    _context.Rb.linearVelocity = -_context.Transform.forward * _context.IdleMS;
            }
        }
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override BossStateMachine.BossState GetNextState()
    {
        return StateKey;
    }

    private void CheckForPlayer()
    {
        if (Time.frameCount % _context.CheckFrameSetup == 0)
        {
            Collider[] colliders = Physics.OverlapSphere(_context.StateMachine.transform.position, _context.DetectionRadius, _context.WhatIsPlayer);
            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    _context.PlayerCollider = collider;
                    _context.ActivateWalls();
                    _context.Healthbar.gameObject.SetActive(true);
                    _context.SetState(BossStateMachine.BossState.Chasing);
                    break;
                }
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTriggerExit(Collider other)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTriggerStay(Collider other)
    {
        //throw new System.NotImplementedException();
    }
}
