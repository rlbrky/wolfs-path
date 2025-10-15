using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float lifeTime;
    public float explosionForce;
    private bool managedToHitPlayer;

    private void Awake()
    {
        enabled = false;
    }

    private void Start()
    {
        managedToHitPlayer = false;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !managedToHitPlayer)
        {
            managedToHitPlayer = true;
            PlayerCombat.instance.GetHit(2, 10, new Vector3(-PlayerMovement.instance.transform.forward.x, Vector3.up.y));
        }

        if(other.tag == "Breakable")
        {
            other.GetComponent<BreakableSc>().ExplodeObject(explosionForce);
        }
    }
}
