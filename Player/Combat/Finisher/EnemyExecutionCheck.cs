using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExecutionCheck : MonoBehaviour
{
    public IEnemy parentSC;

    private void Awake()
    {
        parentSC = GetComponentInParent<IEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerCombat.instance.enemyToExecute = parentSC;
            parentSC.canExecute.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerCombat.instance.enemyToExecute = null;
            parentSC.canExecute.SetActive(false);
        }
    }
}
