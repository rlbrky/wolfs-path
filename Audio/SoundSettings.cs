using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour, ISaveManager
{
    [SerializeField] private Slider SFX_Slider;
    [SerializeField] private Slider Music_Slider;

    [Header("Audio")]
    [SerializeField] private AudioMixer SFX_Manager;

    public void LoadData(GameData data)
    {
        SFX_Slider.value = data.sfxValue;
        Music_Slider.value = data.musicValue;
        SFX_OnSliderValueChanged();
        Music_OnSliderValueChanged();
    }

    public void SaveData(GameData data)
    {
        data.sfxValue = SFX_Slider.value;
        data.musicValue = Music_Slider.value;
    }

    private void OnEnable()
    {
        SFX_Slider.interactable = true;
        Music_Slider.interactable = true;
    }

    private void OnDisable()
    {
        SaveData(DataManager.Instance.GameData);
    }

    private void Start()
    {
        LoadData(DataManager.Instance.GameData);
    }

    public void SFX_OnSliderValueChanged()
    {
        SFX_Manager.SetFloat("SFX_Volume", Mathf.Log10(SFX_Slider.value) * 20);
    }

    public void Music_OnSliderValueChanged() 
    {
        SFX_Manager.SetFloat("Music_Volume", Mathf.Log10(Music_Slider.value) * 20);
    }
}
