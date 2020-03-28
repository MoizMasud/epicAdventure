using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelSelectManager : MonoBehaviour
{
    private static LevelSelectManager _instance;

    public static LevelSelectManager Instance { get { return _instance; } }

    public Transform levelPanel;

    public int selectedLevel = 1;

    public int GetSelectedLevelAndDestroy { get {
        Destroy(this.gameObject);
        return selectedLevel;
    } }

    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    
    void Start()
    {
        this.InitLevelPanel();
    }

    private void InitLevelPanel()
    {
        // Check references
        if (levelPanel == null)
            Debug.Log("Level panel refence(s) has not been assigned in inspector");

        // Add event listerners to every child of color/trail panel
        int i = 1;
        foreach (Transform t in levelPanel)
        {
            int currInd = i;
            Button b = t.GetComponent<Button> ();
            b.onClick.AddListener(() => {
                selectedLevel = currInd;
                this.LoadScene("NameScreen");
            });
            i++;
        }
    }

    private void LoadScene(string sceneName)
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene(sceneName);
    }
}
