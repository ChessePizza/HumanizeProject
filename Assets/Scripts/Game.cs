using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject background;
    public GameObject main_menu;
    public GameObject settings_menu;
    public GameObject level_menu;

    void Start()
    {
        
    }

    void Update()
    {
    }

    // Goto Functions
    public void GotoMainMenu() {
        main_menu.SetActive(true);
        if(!background.activeSelf) background.SetActive(true);

        settings_menu.SetActive(false);
        level_menu.SetActive(false);
    }
    public void GotoSettingsMenu()
    {
        settings_menu.SetActive(true);
        if (!background.activeSelf) background.SetActive(true);

        main_menu.SetActive(false);
        level_menu.SetActive(false);
    }
    public void GotoLevelMenu()
    {
        level_menu.SetActive(true);
        if (!background.activeSelf) background.SetActive(true);

        main_menu.SetActive(false);
        settings_menu.SetActive(false);
    }

    public void GotoLevel(int level)
    {
        StartCoroutine(LoadLevelAsync());
    }

    // System
    IEnumerator LoadLevelAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/Level", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        background.SetActive(false);
        level_menu.SetActive(false);
        main_menu.SetActive(false);
        settings_menu.SetActive(false);

        print("Done!");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
