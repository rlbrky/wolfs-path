using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTrigger : MonoBehaviour
{
    private bool isTorchActive;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !isTorchActive)
        {
            isTorchActive = true;
            PlayerMovement.instance.animator.SetBool("isMoving", false);
            PlayerMovement.instance.canMove = false;
            PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
            //PlayerMovement.instance.torchGO.SetActive(true);
            PlayerMovement.instance.animator.SetTrigger("torchEquip");
        }
        else if(other.tag == "Player" && isTorchActive)
        {
            PlayerMovement.instance.animator.SetLayerWeight(2, 0);
            isTorchActive =false;
            PlayerMovement.instance.canMove = false;
            PlayerMovement.instance.animator.SetBool("isMoving", false);
            //PlayerMovement.instance.torchGO.SetActive(false);
            PlayerMovement.instance.rb.linearVelocity = Vector3.zero;
            PlayerMovement.instance.animator.SetTrigger("torchUnequip");
        }
    }
}
