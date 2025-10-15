using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowWalk : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !PlayerMovement.instance.isSlowWalking)
        {
            PlayerMovement.instance.animator.SetLayerWeight(3, 1);
            PlayerMovement.instance.isSlowWalking = true;
        }
        else if (other.tag == "Player" && PlayerMovement.instance.isSlowWalking)
        {
            PlayerMovement.instance.animator.SetLayerWeight(3, 0);
            PlayerMovement.instance.isSlowWalking = false;
        }
    }
}
