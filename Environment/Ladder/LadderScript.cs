using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.instance.activeLadder = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement.instance.activeLadder = null;
        PlayerMovement.instance.rb.useGravity = true;
        PlayerMovement.instance.isLadderClimbing = false;
        PlayerMovement.instance.animator.SetBool(PlayerMovement.instance.ladderAnimHash, PlayerMovement.instance.isLadderClimbing);
        PlayerCombat.instance.Katana.SetActive(true);
    }
}
