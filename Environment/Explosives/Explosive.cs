using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public Vector3 offset;
    public ParticleSystem explosionEffect;
    public ExplosionScript explosionSC;
    public SphereCollider explosionCollider;
    private ParticleSystem prefab;

    private void Start()
    {
        prefab = Instantiate(explosionEffect);
        explosionSC = prefab.GetComponent<ExplosionScript>();
        explosionCollider = prefab.GetComponent<SphereCollider>();
        explosionSC.enabled = false;
        explosionCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Arrow")
        {
            prefab.transform.position = transform.position + offset;
            explosionCollider.enabled = true;
            explosionSC.enabled = true;
            prefab.Play();
            InteractUI_SC.instance.HideInteractImage();
            Destroy(gameObject);
            //TO DO: Save it so it doesnt respawn.
        }
    }
}
