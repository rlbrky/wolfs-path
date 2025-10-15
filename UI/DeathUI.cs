using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    public static DeathUI instance { get; set; }

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

    public void ActivateUI()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OnRetryPressed()
    { 
        Time.timeScale = 1.0f;
        PlayerCombat.instance.ReviveKilledEnemies();
        PlayerHealth.instance.FullHeal();
        PlayerMovement.instance.transform.position = new Vector3(PlayerMovement.instance.lastSaved_SavePoint.transform.position.x, PlayerMovement.instance.lastSaved_SavePoint.transform.position.y);

        gameObject.SetActive(false);

        if (DataManager.Instance.GameData.lastSceneName != SceneManager.GetActiveScene().name)
            SceneManager.LoadScene(DataManager.Instance.GameData.lastSceneName);
    }
}
