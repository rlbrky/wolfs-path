using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileID = "";

    [Header("Content")]
    [SerializeField] private GameObject noData;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI content;

    [Header("Clear Data Button")]
    [SerializeField] private Button clearButton;

    public bool hasData { get; private set; } = false;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        //No data found for this profileId
        if(data == null)
        {
            hasData = false;
            noData.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        else
        {
            hasData = true;
            noData.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            content.text = "Last Saved Scene: " + data.lastSceneName;
        }
    }

    public string GetProfileID()
    {
        return profileID;
    }

    public void SetInteractable(bool interactable) 
    { 
        saveSlotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }
}
