using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaUpgrade : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.instance.pickupUI.SetActive(true);
            PlayerMovement.instance.manaUpgrade = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.instance.pickupUI.SetActive(false);
            PlayerMovement.instance.manaUpgrade = null;
        }
    }
}
