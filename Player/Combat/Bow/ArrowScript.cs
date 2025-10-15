using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    public float lifeTime;

    private void Start()
    {
        transform.forward = PlayerMovement.instance.transform.forward;
        //transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void Update()
    {
        if(lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            PlayerCombat.instance.prevArrow = null;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            PlayerCombat.instance.hitEffect.transform.position = transform.position;
            PlayerCombat.instance.hitEffect.Play();
            var enemy = other.GetComponent<IEnemyEvents>();
            enemy.GetHit(PlayerCombat.instance.transform.forward, PlayerCombat.instance.knockbackForce, 0, PlayerCombat.instance.damage);
        }
        if(other.tag != "Player")
        {
            PlayerCombat.instance.prevArrow = null;
            Destroy(gameObject);
        }
    }
}
