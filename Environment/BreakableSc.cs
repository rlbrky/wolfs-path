using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableSc : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject brokenVersion;
    [SerializeField] private float lifeTime = 2f;

    public void ExplodeObject(float explosionForce)
    {
        //brokenVersion.transform.SetParent(null);       
        brokenVersion.SetActive(true);
        var rigidbodies = brokenVersion.GetComponentsInChildren<Rigidbody>();
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.AddExplosionForce(explosionForce, transform.position, 4);
        }

        boxCollider.enabled = false;
        meshRenderer.enabled = false;
        Destroy(rb);
        StartCoroutine(DestroyObjectWithLifeTime());
    }

    IEnumerator DestroyObjectWithLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
