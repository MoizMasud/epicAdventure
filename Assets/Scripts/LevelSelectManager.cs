using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelSelectManager : MonoBehaviour
{
    public Transform levelPanel;
    
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
            b.onClick.AddListener(() => this.LoadScene(currInd.ToString()));
            i++;
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
