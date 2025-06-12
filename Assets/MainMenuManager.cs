using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject PlayMenu;
    public GameObject MapSelector;

    public void PlayButton()
    {
        PlayMenu.SetActive(false);
        MapSelector.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void BackButton()
    {
        PlayMenu.SetActive(true);
        MapSelector.SetActive(false);
    }

    public void MapButton(String mapName)
    {
        SceneManager.LoadScene(mapName);
    }

}
