using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Gameplay))]
public class GameUI : MonoBehaviour
{
    [Header("User Interface")]
    public GameObject inventoryUI;
    public GameObject statusUI;
    public GameObject joystickUI;
    public GameObject zoomSliderUI;
    public GameObject buildcategoryUI;
    public GameObject pauseUI;
    public GameObject buildInfoUI;
    public GameObject buildConfirmationUI;
    public GameObject settingsUI;
    public GameObject menuListUI;
    public GameObject goalUI;
    public GameObject backgroundUI;
    public GameObject screenshotUI;
    public GameObject endUI;
    public GameObject menuTab;    

    [Header("Buttons")]
    public GameObject goalButton;
    public GameObject settingsButton;
    public GameObject buildConfirmButton;
    public GameObject removeButton;

    [Header("Sprite")]
    public Sprite gameOverSprite;
    public Sprite gameWinSprite;

    [Header("Grid UI")]
    public GameObject impassable;
    public Vector2 impassableAdjust;

    [Header("Text")]
    public Text announceTitleText;
    public Text announceSubText;
    public Text pauseText;
    public Text timer;
    public Text killCountText;
    public Text endTitle;
    public Text endInfo;

    [Header("Images")]
    public Image warning;
    public Image endIcon;
    public Image inventoryButton;
    public Image buildcategoryButton;

    [Header("References")]
    public GameData data;
    Gameplay gameplay;

    public bool isRealEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        isRealEnd = false;
        if (GameObject.Find("Data"))
        {
            data = GameObject.Find("Data").GetComponent<GameData>();
            if (!data.bgm.isPlaying) data.bgm.Play();
            if (!data.sfx.isPlaying) data.sfx.Play();

            if (PlayerPrefs.GetInt("everSettings") > 0)
            {
                data.audioMixer.SetFloat("MainVolumePara", PlayerPrefs.GetFloat("MainVolumePara"));
                data.audioMixer.SetFloat("MusicVolumePara", PlayerPrefs.GetFloat("MusicVolumePara"));
                data.audioMixer.SetFloat("EffectVolumePara", PlayerPrefs.GetFloat("EffectVolumePara"));
            }
        }

        gameplay = GetComponent<Gameplay>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Game End UI

    public void SetGameEnd(string title, string description, bool isGameOver)
    {
        endTitle.text = title;
        endIcon.sprite = (isGameOver) ? gameOverSprite : gameWinSprite;
        endInfo.text = description;
    }

    // Warning UI
    bool isWarning;
    public void showWarning()
    {
        warning.gameObject.SetActive(true);
        if (!isWarning)
        {
            StartCoroutine(EndWarning());
        }
    }

    IEnumerator EndWarning()
    {
        isWarning = true;
        yield return new WaitForSeconds(7);
        warning.gameObject.SetActive(false);
        isWarning = false;
        yield return null;
    }

    // Game State UI

    public void Announce(string title, string description)
    {
        announceTitleText.text = title;
        announceSubText.text = description;
        Animator anim = announceTitleText.GetComponent<Animator>();
        if (anim.GetBool("rerun")) anim.SetBool("rerun", false);
        anim.SetBool("start", true);
        anim.Play("Start");
    }

    public void HideAll()
    {
        inventoryUI.SetActive(false);
        statusUI.SetActive(false);
        joystickUI.SetActive(false);
        zoomSliderUI.SetActive(false);
        buildcategoryUI.SetActive(false);
        pauseUI.SetActive(false);
        buildInfoUI.SetActive(false);
        buildConfirmationUI.SetActive(false);
        settingsUI.SetActive(false);
        menuListUI.SetActive(false);
        goalUI.SetActive(false);
        backgroundUI.SetActive(false);
        screenshotUI.SetActive(false);
        menuTab.SetActive(false);
        goalButton.SetActive(false);
        settingsButton.SetActive(false);
        buildConfirmButton.SetActive(false);
        removeButton.SetActive(false);
    }

    public void ShowAll()
    {
        inventoryUI.SetActive(true);
        statusUI.SetActive(true);
        joystickUI.SetActive(true);
        zoomSliderUI.SetActive(true);
        menuListUI.SetActive(true);
        menuTab.SetActive(true);
        goalButton.SetActive(true);
        settingsButton.SetActive(true);
        removeButton.SetActive(true);
    }

    // Timer
    public void SetTimer(int hour, int minute)
    {
        TimeManeger.Hour = hour;
        TimeManeger.Minute = minute;
    }

    public void StartTimer()
    {
        TimeManeger.OnMinuteChanged += UpdateTimer;
        TimeManeger.OnHourChanged += UpdateTimer;
    }

    public void StopTimer()
    {
        TimeManeger.OnMinuteChanged -= UpdateTimer;
        TimeManeger.OnHourChanged -= UpdateTimer;
    }
    public void UpdateTimer()
    {
        timer.text = $"{TimeManeger.Hour:00}:{TimeManeger.Minute:00}";
    }

    // Settings Functions
    public void SetVolume(float volume)
    {
        data.audioMixer.SetFloat("MainVolumePara", volume);
        PlayerPrefs.SetFloat("MainVolumePara", volume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        data.audioMixer.SetFloat("MusicVolumePara", musicVolume);
        PlayerPrefs.SetFloat("MusicVolumePara", musicVolume);
    }

    public void SetEffectVolume(float effectVolume)
    {
        data.audioMixer.SetFloat("EffectVolumePara", effectVolume);
        PlayerPrefs.SetFloat("EffectVolumePara", effectVolume);
    }

    // Navigation Functions

    public void SwitchToInventory()
    {
        removeButton.SetActive(true);
        buildcategoryUI.SetActive(false);
        inventoryUI.SetActive(true);
        inventoryButton.color = Color.white;
        buildcategoryButton.color = new Color(0.25f, 0.25f, 0.25f, 1.0f);

        gameplay.changeMode(Gameplay.GameMode.Scout);
        gameplay.grid.gameObject.SetActive(false);
    }
    public void SwitchToBuildCategory()
    {
        removeButton.SetActive(false);
        buildcategoryUI.SetActive(true);
        inventoryUI.SetActive(false);
        inventoryButton.color = new Color(0.45f, 0.45f, 0.45f, 1.0f);
        buildcategoryButton.color = Color.white;

        gameplay.changeMode(Gameplay.GameMode.Build);
        UpdatePassability();
        gameplay.grid.gameObject.SetActive(true);
    }

    public void UpdatePassability()
    {
        bool needRescan = false;
        for (int x = 0; x < gameplay.grid.size.x; x++)
            for (int y = 0; y < gameplay.grid.size.y; y++)
            {
                int value = gameplay.grid.data[(x * gameplay.grid.size.y) + y];
                if (value > 0)
                {
                    if (!gameplay.grid.transform.Find("impassable_" + x + "_" + y))
                    {
                        GameObject o = Instantiate(impassable, new Vector3(0, 0, 0), Quaternion.identity);
                        o.name = "impassable_" + x + "_" + y;
                        if (value == 2)
                        {
                            o.layer = LayerMask.NameToLayer("Impassable");
                            needRescan = true;
                        }
                        o.transform.position = new Vector3(
                            (gameplay.cellSize.x * x) - (gameplay.gridSize.x / 2.0f) + impassableAdjust.x,
                            (gameplay.cellSize.y * (gameplay.grid.size.y - y)) - (gameplay.gridSize.y / 2.0f) + impassableAdjust.y,
                            1.0f);
                        o.transform.SetParent(gameplay.grid.transform);
                    }
                }
                else 
                {
                    Transform cell = gameplay.grid.transform.Find("impassable_" + x + "_" + y);
                    if (cell != null) Destroy(cell.gameObject);
                }
            }

        if (needRescan) {
            // Astar Rescan
            // Recalculate all graphs
            AstarPath.active.Scan();
        }
    }

    public void ToggleGoal()
    {
        goalUI.SetActive(!goalUI.activeSelf);
        backgroundUI.SetActive(!backgroundUI.activeSelf);
    }
    public void ToggleScreenShot()
    {
        screenshotUI.SetActive(!screenshotUI.activeSelf);
    }

    public void TogglePause()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
        backgroundUI.SetActive(!backgroundUI.activeSelf);
    }
    public void OpenBuildInfo(BuildItemUI item)
    {
        buildInfoUI.SetActive(true);

        Text title = buildInfoUI.transform.Find("UIBar/Text").GetComponent<Text>();
        title.text = item.building.title;

        Image icon = buildInfoUI.transform.Find("Info/Icon").GetComponent<Image>();
        icon.sprite = item.GetComponent<Image>().sprite;

        Text description = buildInfoUI.transform.Find("Info/Description").GetComponent<Text>();
        description.text = item.building.description;

        Button button = buildInfoUI.transform.Find("Info/BuildButton").GetComponent<Button>();
        button.onClick.AddListener(() => gameplay.build(item.building));

        backgroundUI.SetActive(true);
    }

    public void CloseBuildInfo()
    {
        buildInfoUI.SetActive(false);
        backgroundUI.SetActive(false);
    }
    public void OpenBuildConfirmation()
    {
        buildConfirmationUI.SetActive(true);
        removeButton.SetActive(false);
        inventoryUI.SetActive(false);
        buildcategoryUI.SetActive(false);
        menuTab.SetActive(false);
        goalButton.SetActive(false);
        settingsButton.SetActive(false);
    }
    public void CloseBuildConfirmation()
    {
        buildConfirmationUI.SetActive(false);
        removeButton.SetActive(false);
        inventoryUI.SetActive(false);
        buildcategoryUI.SetActive(true);
        menuTab.SetActive(true);
        goalButton.SetActive(true);
        settingsButton.SetActive(true);
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
        AsyncOperation asyncLoad;
        if (gameplay.levelId == 1 && isRealEnd)
        {
            asyncLoad = SceneManager.LoadSceneAsync("Scenes/CutsceneEpilogue", LoadSceneMode.Single);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync("Scenes/Main", LoadSceneMode.Single);
        }

        while (!asyncLoad.isDone)
        {
            data.backFromLevel = true;
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