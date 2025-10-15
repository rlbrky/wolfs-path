using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_Chase : Dog_Informer
{
    private float timer;

    public Dog_Chase(Dog_Context context, Dog_StateMachine.DogState stateKey) : base(context, stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        timer = 0;
    }

    public override void UpdateState()
    {
        if (timer < 5)
        {
            _context._Animator.SetBool("isMoving", true);
            _context.StateMachine.transform.position = Vector3.MoveTowards(_context.StateMachine.transform.position, new Vector3(_context.PlayerCollider.transform.position.x, _context.StateMachine.transform.position.y, _context.StateMachine.transform.position.z), _context.Speed * Time.deltaTime);
            CheckForPlayer();
            timer += Time.deltaTime;
        }
        else
        {
            _context.PlayerCollider = null;
            //_context.SwitchState(Dog_StateMachine.DogState.Patrol);
        }
    }

    public override void ExitState()
    {
        timer = 0;
        _context._Animator.SetBool("isMoving", false);
    }

    public override Dog_StateMachine.DogState GetNextState()
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
                    _context.SwitchState(Dog_StateMachine.DogState.Attack);
                    break;
                }
            }
        }
    }

    #region NOT IMPLEMENTED
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
    #endregion
}
