using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall")
        {
            PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            PlayerMovement.instance.climbableObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            PlayerMovement.instance.climbableObj = null;
        }
    }
}
