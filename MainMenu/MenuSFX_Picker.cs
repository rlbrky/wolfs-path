using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSFX_Picker : MonoBehaviour
{
    public static MenuSFX_Picker instance;

    [Header("Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip buttonSelect_Clip;
    [SerializeField] private AudioClip buttonClick_Clip;

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

    public void OnButtonSelected()
    {
        sfxSource.clip = buttonSelect_Clip;
        sfxSource.Play();
    }

    public void OnButtonClicked()
    {
        sfxSource.clip = buttonClick_Clip;
        sfxSource.Play();
    }
}
