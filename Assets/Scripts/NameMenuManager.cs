using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class NameMenuManager : MonoBehaviour
{
    public Text username;
    static public string characterName;
    private bool hasName = false;

    private void Start()
    {
    }
    public void TextChanged(string newText)
    {
        newText = username.text;
        if (newText.Length > 0)
        {
            hasName = true;
        }
        else
        {
            hasName = false;
        }
        characterName = newText;
    }
    public void ToGame()
    {
        if (hasName)
        {
            
            SceneManager.LoadScene("MainScene");
        }



    }

    public void ToMenu()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("MenuScene");
    }
    public string getName()
    {
        if (hasName)
        {
            return characterName;
        }
        else
        {
            Debug.Log("camehere");
            return "";
        }

    }
}
