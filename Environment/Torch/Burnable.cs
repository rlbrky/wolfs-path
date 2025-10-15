using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerMovement.instance.burnable = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerMovement.instance.burnable = null;
        }
    }

    public void Burn()
    {
        Destroy(gameObject);
    }
}
