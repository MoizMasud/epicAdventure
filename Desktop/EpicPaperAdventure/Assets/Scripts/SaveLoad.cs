using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
public class SaveLoad : MonoBehaviour
{
    public GameObject playerScoreEntryPrefab;
    LevelManager score;
    private SaveData somedata;
    [System.Serializable]
    public class PlayerInfo
    {
        public float score;
        public int level;
        public string playerName;

    }

    [System.Serializable]
    public class SaveData
    {

        public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    }

    private void Start()
    {

    }

    public void Save(float score, int level, string playerName)
    {

        SaveData data = new SaveData();
        Load();
        if (somedata != null)
        {
            
            data = somedata;
        }

        for (int i = 0; i < GameManager.instance.playersList.Count; i++)
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.score = score;
            playerInfo.level = level;
            playerInfo.playerName = playerName;
            data.playerInfoList.Add(playerInfo);
        }

        // do the saving

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/savingFile.epicAdv1", FileMode.Create);

        binaryFormatter.Serialize(stream, data);

        stream.Close();

        Debug.Log("game saved" + data.playerInfoList.Count);

    }
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savingFile.epicAdv1"))
        {
            // Do loading
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/savingFile.epicAdv1", FileMode.Open);
            //passing data back
            if (stream.Length > 0)
            {
                SaveData data = (SaveData)binaryFormatter.Deserialize(stream);
                somedata = data;

            }
            stream.Close();
        }
        else
        {
            Debug.Log("no file found");
        }
    }

    public SaveData getData()
    {
        return somedata;
    }


}
