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

        if (PlayerPrefs.GetInt("everSettings") > 0)
        {
            data.audioMixer.SetFloat("MainVolumePara", PlayerPrefs.GetFloat("MainVolumePara"));
            data.audioMixer.SetFloat("MusicVolumePara", PlayerPrefs.GetFloat("MusicVolumePara"));
            data.audioMixer.SetFloat("EffectVolumePara", PlayerPrefs.GetFloat("EffectVolumePara"));
        }
        else
        {
            PlayerPrefs.SetInt("everSettings", 1);
            float mainVolumePara;
            data.audioMixer.GetFloat("MainVolumePara", out mainVolumePara);
            PlayerPrefs.SetFloat("MainVolumePara", mainVolumePara);

            float musicVolumePara;
            data.audioMixer.GetFloat("MusicVolumePara", out musicVolumePara);
            PlayerPrefs.SetFloat("MusicVolumePara", musicVolumePara);

            float effectVolumePara;
            data.audioMixer.GetFloat("EffectVolumePara", out effectVolumePara);
            PlayerPrefs.SetFloat("EffectVolumePara", effectVolumePara);
        }

        if (data.backFromLevel)
        {
            main_menu.SetActive(false);
            settings_menu.SetActive(false);
            level_menu.SetActive(true);
        }
        else
        {
            main_menu.SetActive(true);
            settings_menu.SetActive(false);
            level_menu.SetActive(false);
        }
    }

    void Update()
    {
    }

    // Settings Functions

    public void SetVolume(float volume)
    {
        if(data) data.audioMixer.SetFloat("MainVolumePara", volume);
        PlayerPrefs.SetFloat("MainVolumePara", volume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        if (data) data.audioMixer.SetFloat("MusicVolumePara", musicVolume);
        PlayerPrefs.SetFloat("MusicVolumePara", musicVolume);
    }

    public void SetEffectVolume(float effectVolume)
    {
        if (data) data.audioMixer.SetFloat("EffectVolumePara", effectVolume);
        PlayerPrefs.SetFloat("EffectVolumePara", effectVolume);
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
        StartCoroutine(LoadLevelAsync(level));
    }

    // System
    IEnumerator LoadLevelAsync(int level)
    {
        AsyncOperation asyncLoad;
        if (level == 1)
        {
            asyncLoad = SceneManager.LoadSceneAsync("Scenes/CutscenePrologue", LoadSceneMode.Single);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync("Scenes/Level2", LoadSceneMode.Single);
        }

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
