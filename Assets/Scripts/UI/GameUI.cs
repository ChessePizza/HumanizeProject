using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Gameplay))]
public class GameUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject buildcategoryUI;
    public GameObject pauseUI;
    public GameObject settingsUI;
    public GameObject menuListUI;
    public GameObject goalUI;
    public GameObject backgroundUI;
    public Text pauseText;
    public Image inventoryButton;
    public Image buildcategoryButton;
    public GameData data;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Data"))
        {
            data = GameObject.Find("Data").GetComponent<GameData>();
            if (!data.bgm.isPlaying) data.bgm.Play();
            if (!data.sfx.isPlaying) data.sfx.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Settings Functions
    public void SetVolume(float volume)
    {
        data.audioMixer.SetFloat("MainVolumePara", volume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        data.audioMixer.SetFloat("MusicVolumePara", musicVolume);
    }

    public void SetEffectVolume(float effectVolume)
    {
        data.audioMixer.SetFloat("EffectVolumePara", effectVolume);
    }

    // Navigation Functions

    public void SwitchToInventory()
    {
        buildcategoryUI.SetActive(false);
        inventoryUI.SetActive(true);
        inventoryButton.color = Color.white;
        buildcategoryButton.color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
    }
    public void SwitchToBuildCategory()
    {
        buildcategoryUI.SetActive(true);
        inventoryUI.SetActive(false);
        inventoryButton.color = new Color(0.45f, 0.45f, 0.45f, 1.0f);
        buildcategoryButton.color = Color.white;
    }

    public void ToggleGoal()
    {
        goalUI.SetActive(!goalUI.activeSelf);
        backgroundUI.SetActive(!backgroundUI.activeSelf);
    }
    public void TogglePause()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
        backgroundUI.SetActive(!backgroundUI.activeSelf);
    }

    public void ToggleSettings()
    {
        settingsUI.SetActive(!settingsUI.activeSelf);
        menuListUI.SetActive(!menuListUI.activeSelf);
        pauseText.text = settingsUI.activeSelf ? "SETTINGS" : "PAUSE";
    }

    public void GotoMain()
    {
        StartCoroutine(LoadLevelAsync());
    }

    // System
    IEnumerator LoadLevelAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/Main", LoadSceneMode.Single);

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