using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockClimb : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground")
        {
            PlayerMovement.instance.blockingObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            PlayerMovement.instance.blockingObj = null;
        }
    }
}
