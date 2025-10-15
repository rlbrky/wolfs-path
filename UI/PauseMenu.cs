using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button ControlsButton;
    [SerializeField] private Button ControlsBackButton;
    [SerializeField] private GameObject controlsScreen;

    private void Start()
    {
        CheckForMenu();
    }

    private void OnEnable()
    {
        //CheckForMenu();
        controlsScreen.SetActive(false);
        ControlsBackButton.gameObject.SetActive(false);
        MainMenuButton.interactable = true;
        ResumeButton.interactable = true;
        ControlsButton.interactable = true;
    }

    public void OnMainMenuClicked()
    {
        DisableEverything();
        PlayerMovement.instance.OnReturnToMainMenu();
        Destroy(InteractUI_SC.instance.gameObject);
        Destroy(PlayerCombat.instance.gameObject);
        Destroy(GameManager.instance.gameObject);
        SceneManager.LoadSceneAsync("MainMenu");
        Destroy(gameObject);
    }

    public void OnResumeClicked()
    {
        DisableEverything();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Continue game time
        Time.timeScale = 1f;
        //DisableMenu
        gameObject.SetActive(false);
    }

    public void OnControlsClicked()
    {
        DisableEverything();
        controlsScreen.SetActive(true);
        ControlsBackButton.gameObject.SetActive(true);
        MainMenuButton.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
    }

    public void OnControlsBackClicked()
    {
        controlsScreen.SetActive(false);
        MainMenuButton.gameObject.SetActive(true);
        ResumeButton.gameObject.SetActive(true);
        ControlsButton.gameObject.SetActive(true);
        MainMenuButton.interactable = true;
        ResumeButton.interactable = true;
        ControlsButton.interactable = true;
    }

    private void DisableEverything()
    {
        MainMenuButton.interactable = false;
        ResumeButton.interactable = false;
        ControlsButton.interactable = false;
    }

    private void CheckForMenu()
    {
        if (PlayerMovement.instance.pauseMenu == this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (PlayerMovement.instance.pauseMenu != null)
                Destroy(PlayerMovement.instance.pauseMenu.gameObject);

            PlayerMovement.instance.pauseMenu = this;
            gameObject.SetActive(false);
        }
    }
}
