using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerMovement.instance.pickupUI.SetActive(true);
            PlayerMovement.instance.healthUpgrade = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.instance.pickupUI.SetActive(false);
            PlayerMovement.instance.healthUpgrade = null;
        }
    }
}
