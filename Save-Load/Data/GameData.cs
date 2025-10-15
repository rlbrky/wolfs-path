using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Last saved time for save slot.
    public long lastUpdated;

    //public bool hasBow;
    public int lastSavePointID; //Last save point player saved at.
    public string lastSceneName; //Last scene player saved at.

    public int healthUpgradeFragmentCount;
    public int manaUpgradeFragmentCount;

    public float maxPlayerMana;
    public float maxPlayerHealth;
    public Vector2 healthSliderSize;
    public Vector2 manaSliderSize;
    //Audio Settings
    public float sfxValue;
    public float musicValue;
    //public SerializableDictionary<string, bool> coinsCollected;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        //hasBow = false;
        healthSliderSize = new Vector2(300, 100);
        manaSliderSize = new Vector2(300, 100);
        sfxValue = 0.5f;
        musicValue = 0.5f;
        lastSavePointID = 0;
        lastSceneName = "Kreya1";
        healthUpgradeFragmentCount = 0;
        manaUpgradeFragmentCount = 0;
        maxPlayerMana = 100;
        maxPlayerHealth = 100;
        // coinsCollected = new SerializableDictionary<string, bool>();
    }
}