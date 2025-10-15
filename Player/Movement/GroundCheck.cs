using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            PlayerMovement.instance.animator.SetBool("isFalling", false);
            PlayerMovement.instance.isGrounded = true;
            PlayerMovement.instance.hasDoubleJump = true;
            PlayerMovement.instance.groundType = other.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            PlayerMovement.instance.animator.SetBool("isFalling", true);
            PlayerMovement.instance.isGrounded = false;
            PlayerMovement.instance.groundType = null;
        }
    }
}
