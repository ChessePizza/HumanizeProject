using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    public GameObject background;
    public GameObject main_menu;
    public GameObject settings_menu;
    public GameObject level_menu;
    public GameData data;

    void Start()
    {
        if (!GameObject.Find("Data"))
        {
            data = Instantiate(data, transform.position, transform.rotation);
            data.gameObject.name = "Data";
        }
        else 
        {
            data = GameObject.Find("Data").GetComponent<GameData>();
        }
        if (!data.sfx.isPlaying) data.sfx.Play();
        if (!data.bgm.isPlaying) data.bgm.Play();
    }

    void Update()
    {
    }

    // Settings Functions

    public void SetVolume(float volume)
    {
        if(data) data.audioMixer.SetFloat("MainVolumePara", volume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        if (data) data.audioMixer.SetFloat("MusicVolumePara", musicVolume);
    }

    public void SetEffectVolume(float effectVolume)
    {
        if (data) data.audioMixer.SetFloat("EffectVolumePara", effectVolume);
    }

    // Navigation Functions
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/Level", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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
