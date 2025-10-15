using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool disableDataPersistance = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<ISaveManager> dataSavers;
    private FileDataHandler dataHandler;
    private string selectedProfileID = "";

    public GameData GameData => gameData;

    public static DataManager Instance { get; private set; }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataSavers = FindAllDataSaveObjects();
        LoadGame();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Manager in the scene.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (disableDataPersistance)
            Debug.LogWarning("Data Persistance is disabled!");

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        InitializeSelectedProfileID();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        dataSavers = FindAllDataSaveObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileID(string newProfileID)
    {
        selectedProfileID = newProfileID;

        //Load the game which will update our game data according to the new profile
        LoadGame();
    }

    public void DeleteProfileData(string profileID)
    {
        //Delete data for this profile ID
        dataHandler.Delete(profileID);

        InitializeSelectedProfileID();

        LoadGame();
    }

    private void InitializeSelectedProfileID()
    {
        selectedProfileID = dataHandler.GetMostRecentlyUpdatedProfileID();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        if (disableDataPersistance)
            return;

        //Load any saved data from a file to the data handler
        gameData = dataHandler.Load(selectedProfileID);

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        //if no data can be loaded, initialize to a new game
        if (gameData == null)
        {
            Debug.LogWarning("No data was found. A new game should be started in order to continue.");
            return;
        }

        //push the loaded data to all other scripts that need it
        foreach(var dataSaver in dataSavers)
            dataSaver.LoadData(gameData);
    }

    public void SaveGame()
    {
        if (disableDataPersistance)
            return;

        if(gameData == null)
        {
            Debug.LogWarning("No data was found. A new game should be started in order to continue.");
            return;
        }
        //Pass the data to other scripts so they can update it.
        foreach(ISaveManager dataSaver in dataSavers)
        {
            dataSaver.SaveData(gameData);
        }

        //Timestamp the data for latest save.
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        //Save the data to a file using the data handler.
        dataHandler.Save(gameData, selectedProfileID);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private List<ISaveManager> FindAllDataSaveObjects()
    {
        //FindObjectsOfType takes an extra bool that takes inactive objects as well
        IEnumerable<ISaveManager> dataSavers = FindObjectsOfType<MonoBehaviour>(false).OfType<ISaveManager>();

        return new List<ISaveManager>(dataSavers);
    }
}
