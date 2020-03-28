using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float score = 0.0f;
    public Text scoreText;
    public Text level;
    private int levelNumber = 1;//start with level 1
    private int maxLevel = 4;// lets say max we have 4 levels.
    private bool hasWon = false;
    public string playerName;
    public LeaderboardManager leaderboardManager;
    public SaveLoad saveLoad;
    private static LevelManager _instance;

    public static LevelManager Instance { get { return _instance; } }

    public List<GameObject> dynamicObjects = new List<GameObject>();
    private List<Vector3> startPositions = new List<Vector3>();
    public GameObject startMenu;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        playerName = NameMenuManager.characterName;
        for (int i = 0; i < dynamicObjects.Count; i++)
        {
            startPositions.Add(dynamicObjects[i].transform.position);
        }
    }

    private void Update()
    {
        if (dynamicObjects[0].transform.position.y < -70)
        {
            Reset();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleStart();
        }
        level.text = "Level: " + levelNumber; // set initial level
        score += Time.deltaTime; // set score be the time.

		string minutes = Mathf.Floor(score / 60).ToString("00");
		string seconds = Mathf.Floor(score % 60).ToString("00");
		scoreText.text = string.Format("{0}:{1}", minutes, seconds);
	}

    public void Reset()
    {
        for (int i = 0; i < dynamicObjects.Count; i++)
        {
            dynamicObjects[i].transform.position = startPositions[i];
            dynamicObjects[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void ToggleStart()
    {
        startMenu.SetActive(!startMenu.activeInHierarchy);
        if (startMenu.activeInHierarchy)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
    }
    public void levelup()
    {
        //load next level here
        levelNumber++;
        if ((levelNumber <= maxLevel) && !hasWon) 
        {

            // load next level HERE

            level.text = "Level: " + levelNumber; //increment the score board level text
            
        }
        else
        {
            hasWon = true;
            // load some type of win screen
            win();
        }
    }

    public void win()
    {
        savePlayerScores();
        //load some type of win screen

    }

    void savePlayerScores()
    {
		string minutes = Mathf.Floor(score / 60).ToString("00");
		string seconds = Mathf.Floor(score % 60).ToString("00");

		saveLoad.Save(string.Format("{0}:{1}", minutes, seconds), levelNumber, playerName);
	}

    public void setName(string name)
    { 
       playerName = name;
    }

}