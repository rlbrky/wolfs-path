using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FindPlayerForCamera : MonoBehaviour
{
    public static FindPlayerForCamera instance { get; private set; }

    private CinemachineVirtualCamera _virtualCamera;

    void Awake()
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

        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void OnSpawnPlayer(Transform player)
    {
        if (PlayerCombat.instance != null)
            PlayerCombat.instance.cinemachineTransposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        else
            FindObjectOfType<PlayerCombat>().cinemachineTransposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        _virtualCamera.Follow = player;
        //_virtualCamera.LookAt = GameManager.instance.Player.transform;
    }
}
