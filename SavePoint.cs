using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    public Vector3 offset;
    public ParticleSystem interactEffect;
    public int ID;

    public void InteractWithSavePoint()
    {
        DataManager.Instance.GameData.lastSavePointID = ID;
        DataManager.Instance.GameData.lastSceneName = SceneManager.GetActiveScene().name;
        PlayerMovement.instance.lastSaved_SavePoint = this;
        interactEffect.Play();
        PlayerHealth.instance.FullHeal();
        PlayerCombat.instance.ReviveKilledEnemies();
        DataManager.Instance.SaveGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerMovement.instance.activeSavePoint = this;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            InteractUI_SC.instance.ShowIneractImage(PlayerMovement.instance.transform.position + offset);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerMovement.instance.activeSavePoint = null;
            InteractUI_SC.instance.HideInteractImage();
        }
    }
}
