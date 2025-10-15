using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : MonoBehaviour
{
    public Rigidbody rb;
    public Collider main;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerMovement.instance.itemToCarry = this;
            InteractUI_SC.instance.ShowIneractImage(transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.instance.itemToCarry = null;
            InteractUI_SC.instance.HideInteractImage();
        }
    }
}
