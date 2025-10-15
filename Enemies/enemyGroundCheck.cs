using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyGroundCheck : MonoBehaviour
{
    IEnemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<IEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground")
        {
            enemy.isGrounded = true;
            if (enemy._Animator.GetCurrentAnimatorStateInfo(0).IsTag("AirHit"))
                enemy._Animator.SetTrigger("downed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
            enemy.isGrounded=false;
    }
}
