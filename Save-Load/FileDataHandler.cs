using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "xdlmao";
    private readonly string backupExtension = ".bak";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID, bool allowRestoreFromBackup = true)
    {
        //base case - if the profileID is null, return.
        if(profileID == null)
            return null;

        //use Path.Combine to account for different OS's having different path seperators. It is / for windows only.
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //optionally decrypt the data
                if (useEncryption)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                //deserialize the data from Json back to the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception ex)
            {
                // we are calling Load function recursively,
                // in order to not loop it until the game crashes we are ensuring that it will run only once
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + ex);
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                    {
                        // try to load again recursively
                        loadedData = Load(profileID, false);
                    }
                }
                else
                {
                    Debug.LogError("Error occured when trying to load file at path: " + fullPath
                        + " and backup did not work. \n" + ex);
                }
            }
        }

        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        //base case - if the profileID is null, return.
        if (profileID == null)
            return;

        // accounts for different OS's having different path seperators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        string backupFilePath = fullPath + backupExtension;

        try
        {
            //Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            //Optionally Encrypt the data
            if(useEncryption)
                dataToStore = EncryptDecrypt(dataToStore);

            //Write the serialized data to the file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                    writer.Write(dataToStore);
            }

            // verify new save that can be loaded
            GameData verifiedGameData = Load(profileID);
            // if it isn't corrupted back it up
            if(verifiedGameData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            }
            else
            {
                throw new Exception("Save file couldn't be verified and backup could not be created.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public string GetMostRecentlyUpdatedProfileID()
    {
        string mostRecentProfileID = null;

        Dictionary<string, GameData> profilesData = LoadAllProfiles();
        foreach(KeyValuePair<string, GameData> kvp in profilesData)
        {
            string profileID = kvp.Key;
            GameData data = kvp.Value;

            //skip entry if gamedata is null
            if(data == null)
                continue;

            //if this is the first data we've come across that exists, it"s the most recent one so far
            if (mostRecentProfileID == null)
                mostRecentProfileID = profileID;
            //otherwise, compare to see which date is most recent
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesData[mostRecentProfileID].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(data.lastUpdated);

                if(newDateTime > mostRecentDateTime) 
                    mostRecentProfileID = profileID;
            }
        }

        return mostRecentProfileID;
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        //Loop over all directory names in the data directory path.
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID  = dirInfo.Name;

            //Check if the data file exists
            //If it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading  all profiles because it doesn't contain data: " + profileID);
                continue;
            }

            //Load game data for this profile and put it in the dictionary
            GameData profileData = Load(profileID);

            //Ensure the profile data isn't null because if it is then something went wrong and we should let it known.
            if (profileData != null) 
            {
                profileDictionary.Add(profileID, profileData);
            }
            else
                Debug.LogError("Tried to load profile but something went wrong. ProfileID: " + profileID);
        }

        return profileDictionary;
    }

    public void Delete(string profileID)
    {
        if (profileID == null)
            return;

        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        try
        {
            if (File.Exists(fullPath))
            {
                //Delete everything and profile folder
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete profile data, but data was not found at path: " + fullPath);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Failed to delete profile data for profileID: " + profileID + " at path: " + fullPath + "\n" + e);
        }
    }

    //Simple implementation of XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }

    private bool AttemptRollback(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;
        try
        {
            // if the file exists, attempts to roll back to it by overwriting the original file
            if(File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back to backup file at: " + backupFilePath);
            }
            //otherwise, we don't yet have a backup file - so there's nothing to roll back to
            else
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to roll back to backup file at: "
                + backupFilePath + "\n" + e);
        }

        return success;
    }
}
