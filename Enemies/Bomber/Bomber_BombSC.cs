using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber_BombSC : MonoBehaviour
{
    public Bomber_StateMachine stateMachine;
    public ParticleSystem explosionEffect;
    public Rigidbody rb;
    public Vector3 direction;
    public SphereCollider bombCollider;
    public float speed;
    public float damage;
    public float timer;

    private ParticleSystem prefab;

    private void Awake()
    {
        prefab = Instantiate(explosionEffect);
    }

    private void Update()
    {
        if (timer < 3)
            timer += Time.deltaTime;
        else
            gameObject.SetActive(false);

        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != stateMachine.gameObject)
        {
            if (other == PlayerMovement.instance.playerCollider)
            {
                prefab.transform.position = transform.position;
                prefab.Play();
                PlayerCombat.instance.GetHit(0, damage, new Vector3((transform.position - PlayerMovement.instance.transform.position).normalized.x, PlayerMovement.instance.transform.position.y));
            }

            if (other.tag == "Enemy")
            {
                prefab.transform.position = transform.position;
                prefab.Play();
                var enemy = other.GetComponent<IEnemyEvents>();
                enemy.GetHit(transform.forward, PlayerCombat.instance.knockbackForce, 0, damage);
            }
            if (other.gameObject != stateMachine.ground.gameObject)
            {
                prefab.transform.position = transform.position;
                prefab.Play();
                gameObject.SetActive(false);
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Debug.Log(collision);
    //    //if (collision.gameObject != stateMachine.gameObject)
    //    //{
    //    //    if (collision.collider.tag == "Player")
    //    //    {
    //    //        var contact = collision.contacts[0];
    //    //        prefab.transform.position = contact.point;
    //    //        prefab.Play();
    //    //        PlayerCombat.instance.GetHit(0, damage, new Vector3((transform.position - PlayerMovement.instance.transform.position).normalized.x, PlayerMovement.instance.transform.position.y));
    //    //    }

    //    //    if (collision.collider.tag == "Enemy")
    //    //    {
    //    //        prefab.transform.position = collision.transform.position;
    //    //        prefab.Play();
    //    //        var enemy = collision.collider.GetComponent<IEnemyEvents>();
    //    //        enemy.GetHit(transform.forward, PlayerCombat.instance.knockbackForce, 0);
    //    //    }

    //    //    if (collision.collider != stateMachine.ground)
    //    //    {
    //    //        prefab.transform.position = collision.transform.position;
    //    //        prefab.Play();
    //    //        gameObject.SetActive(false);
    //    //    }
    //    //}
    //}
}