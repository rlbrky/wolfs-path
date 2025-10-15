using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackRegister : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            PlayerCombat.instance.hitEffect.transform.position = transform.position;
            var enemy = other.GetComponent<IEnemyEvents>();
            gameObject.SetActive(false);
            PlayerHealth.instance.Heal(PlayerHealth.instance.m_HealthMax / 50);
             if (PlayerCombat.instance.shouldKnock)
                 enemy.GetHit(PlayerCombat.instance.transform.forward, PlayerCombat.instance.knockbackForce, 1, PlayerCombat.instance.damage);
             else
                 enemy.GetHit(PlayerCombat.instance.transform.forward, PlayerCombat.instance.knockbackForce, 0, PlayerCombat.instance.damage);
        }

        if(other.tag == "Wall")
        {
            if(other.gameObject.TryGetComponent(out HiddenWallSC hiddenwall))
                hiddenwall.DestroyWall();
        }
    }
}
