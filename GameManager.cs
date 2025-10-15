using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject _player;
    public static GameManager instance {  get; private set; }

    string _spawnPointName;
    bool alreadyLoadingScene;

    public UnityEvent _spikeTrapSetup = new UnityEvent();

    public string areaName;
    public GameObject LoadingScreen;

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
        _player = (GameObject)Resources.Load("Player", typeof(GameObject));
        //areaName = "TestArea";
        //DontDestroyOnLoad(_playerCopy);
        SpawnPlayer();
    }

    private void Start()
    {
        if(DataManager.Instance.GameData.lastSavePointID != 0)
        {
            var allSavePoints = FindObjectsOfType<SavePoint>();
            foreach (SavePoint savePoint in allSavePoints)
            {
                PlayerMovement.instance.transform.position = new Vector3(savePoint.transform.position.x, savePoint.transform.position.y);
            }
        }
        FindPlayerForCamera.instance.OnSpawnPlayer(PlayerMovement.instance.transform);
    }

    private void Update()
    {
        //Debug.Log("Player has " + _playerHealth.Health + " health left.");
    }

    public void ChangeScene(string sceneName, string spawnPointName, string givenAreaName)
    {
        _spawnPointName = spawnPointName;
        areaName = givenAreaName;
        if(!alreadyLoadingScene)
            StartCoroutine(LoadSceneRoutine(sceneName));   
    }

    public void SpawnPlayer()
    {
        GameObject copy = Instantiate(_player, GameObject.Find("SpawnPoint1").transform.position, Quaternion.identity);
         //UIManager.instance.ShowKeyItems(areaName);
         _spikeTrapSetup.Invoke();
         DontDestroyOnLoad(PlayerMovement.instance.gameObject);
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        alreadyLoadingScene = true;
        LoadingScreen.SetActive(true);
        SceneManager.LoadScene(sceneName);
        yield return null;
        //_playerCopy = GameObject.Instantiate(_player, GameObject.Find(_spawnPointName).transform.position, Quaternion.identity);
        //UIManager.instance.ShowKeyItems(areaName);
        _spikeTrapSetup.Invoke();
        GameObject spawnLoc = GameObject.Find(_spawnPointName);
        yield return new WaitForSeconds(0.3f);
        PlayerMovement.instance.transform.position = spawnLoc.transform.position;
        FindPlayerForCamera.instance.OnSpawnPlayer(PlayerMovement.instance.transform);

        yield return null;
        alreadyLoadingScene = false;
        LoadingScreen.SetActive(false);
    }
}
