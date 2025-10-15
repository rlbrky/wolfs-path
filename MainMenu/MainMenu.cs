using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settings_BackButton;
    
    [Header("Menu Navigation")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    private void Start()
    {
        Time.timeScale = 1;
        DisableButtonsDependingOnData();
    }

    public void OnNewGameClicked()
    {
        #region OLD Code
        //DisableMenuButtons();
        ////Create a new game and initialize game data.
        //DataManager.Instance.NewGame();
        ////Load scene to get things going.
        //SceneManager.LoadSceneAsync("BerkayTest");
        #endregion

        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }

    public void OnContinueClicked()
    {
        DisableMenuButtons();
        DataManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync(DataManager.Instance.GameData.lastSceneName);
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }

    public void OnSettingsClicked()
    {
        DeactivateMenu();
        settingsScreen.gameObject.SetActive(true);
    }

    public void OnSettingsBackButton()
    {
        ActivateMenu();
        settingsScreen.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        continueButton.interactable = false;
        loadGameButton.interactable = false;
        settingsButton.interactable = false;
    }

    public void ActivateMenu()
    {
        gameObject.SetActive(true);
        DisableButtonsDependingOnData();
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

    private void DisableButtonsDependingOnData()
    {
        if (!DataManager.Instance.HasGameData())
        {
            continueButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }
}
