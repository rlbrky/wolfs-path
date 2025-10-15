using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI_SC : MonoBehaviour
{
    public static InteractUI_SC instance { get; set; }

    public Vector3 offset;

    public Image interactImage;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }


    public void ShowIneractImage(Vector3 pos)
    {
        transform.position = pos + offset;
        interactImage.transform.LookAt(Camera.main.transform, Vector3.down);
        interactImage.gameObject.SetActive(true);
    }

    public void HideInteractImage()
    {
        interactImage.gameObject.SetActive(false);
    }
}
