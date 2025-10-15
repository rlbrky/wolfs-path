using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Buttons")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup")]
    [SerializeField] private ConfirmationPopup confirmationPopupMenu;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot slot)
    {
        //Disable all buttons
        DisableMenuButtons();

        #region OLD CODE
        ////Update the selected profile ID to be used for data persistence
        //DataManager.Instance.ChangeSelectedProfileID(slot.GetProfileID());

        //if (!isLoadingGame)
        //{
        //    //Create a new game - which will initalize our data to a clean state.
        //    DataManager.Instance.NewGame();
        //}
        #endregion

        // case - loading game
        if (isLoadingGame) 
        {
            DataManager.Instance.ChangeSelectedProfileID(slot.GetProfileID());
            SaveGameAndLoadScene();
        }
        // case - new game
        else if (slot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a New Game will override data on this slot. Do you accept ?",
                // this will execute if the button 'yes' is selected
                () =>
                {
                    DataManager.Instance.ChangeSelectedProfileID(slot.GetProfileID());
                    DataManager.Instance.NewGame();
                    SaveGameAndLoadScene();
                },
                // this will execute if the action is cancelled
                () =>
                {
                    Debug.LogWarning("Cancel called");
                    this.ActivateMenu(isLoadingGame);
                }
                );
        }
        // case - new game, save slot empty
        else
        {
            DataManager.Instance.ChangeSelectedProfileID(slot.GetProfileID());
            DataManager.Instance.NewGame();
            SaveGameAndLoadScene();
        }
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        //Set this menu to be active
        gameObject.SetActive(true);

        //Set mode
        this.isLoadingGame = isLoadingGame;

        //Load all profiles that exist
        Dictionary<string, GameData> profilesGameData = DataManager.Instance.GetAllProfilesData();

        //Ensure the back button is enabled
        backButton.interactable = true;

        //Loop through each save slot in the UI and set data accordingly
        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);
            
            if (profileData != null)
                Debug.Log("Profile data: " + profileData);
            else
                Debug.Log("Profile data is null");

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                    firstSelected = saveSlot.gameObject;
            }
        }

        //Set the first selected button
        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        SetFirstSelected(firstSelectedButton);
    }

    public void DeactivateMenu() 
    {
        gameObject.SetActive(false);
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void SaveGameAndLoadScene()
    {
        //Load the scene - and save the game.
        DataManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync("Kreya1");
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to delete this saved data ?",
            () =>
            {
                DataManager.Instance.DeleteProfileData(saveSlot.GetProfileID());
                ActivateMenu(isLoadingGame);
            },
            () =>
            {
                ActivateMenu(isLoadingGame);
            }
            );
    }

    private void DisableMenuButtons()
    {
        foreach(SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }
}
