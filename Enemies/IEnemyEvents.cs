using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyEvents
{
    public void GetHit(Vector3 dir, float knockbackForce, int damageType, float damageAmount);
}
