using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWallSC : MonoBehaviour
{
    private Material objMat;
    private Coroutine coroutine;

    private void Awake()
    {
        objMat = GetComponent<MeshRenderer>().material;
    }

    public void DestroyWall()
    {
        if(coroutine == null)
            coroutine = StartCoroutine(FadeEffect());
    }

    IEnumerator FadeEffect()
    {
        float time = 0;
        float duration = 1.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            objMat.color = new Color(objMat.color.r, objMat.color.g, objMat.color.b, Mathf.Lerp(1f, 0f, time / duration));
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }
}