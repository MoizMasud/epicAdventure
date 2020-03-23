using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class display : MonoBehaviour
{
    public GameObject playerScoreEntryPrefab;
    // Start is called before the first frame update
    LeaderboardManager leaderboardManager;
    //	public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    SaveLoad load;
    private SaveLoad.SaveData data;
    void Start()
    {

        load = FindObjectOfType<SaveLoad>();
        load.Load();
        data = load.getData();
        
        displayData();

    }

    // Update is called once per frame
    void Update()
    {


    }
    public void displayData()
    {

        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }

        
       
       
        data.playerInfoList = data.playerInfoList.OrderBy(x => x.score).ToList();
        Debug.Log(data.playerInfoList.Count);
        for (int i = 0; i < data.playerInfoList.Count; i++)
        {

            Debug.Log("came into loop");
            GameObject go = (GameObject)Instantiate(playerScoreEntryPrefab);
            go.transform.SetParent(this.transform);
            go.transform.Find("Name").GetComponent<Text>().text = data.playerInfoList[i].playerName;
            go.transform.Find("Level").GetComponent<Text>().text = data.playerInfoList[i].level.ToString();
            go.transform.Find("Score").GetComponent<Text>().text = data.playerInfoList[i].score.ToString();
        }
    }
}
