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
    public GameObject buildInfoUI;
    public GameObject buildConfirmationUI;
    public GameObject settingsUI;
    public GameObject menuListUI;
    public GameObject goalUI;
    public GameObject backgroundUI;
    public GameObject screenshotUI;

    public GameObject menuTab;
    public GameObject goalButton;
    public GameObject settingsButton;
    public GameObject buildConfirmButton;

    public GameObject impassable;
    public Vector2 impassableAdjust;

    public Text pauseText;
    public Text timer;
    public Text killCountText;

    public Image inventoryButton;
    public Image buildcategoryButton;

    public GameData data;
    Gameplay gameplay;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Data"))
        {
            data = GameObject.Find("Data").GetComponent<GameData>();
            if (!data.bgm.isPlaying) data.bgm.Play();
            if (!data.sfx.isPlaying) data.sfx.Play();
        }
        StartTimer();

        gameplay = GetComponent<Gameplay>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Timer
    private void StartTimer()
    {
        TimeManeger.OnMinuteChanged += UpdateTimer;
        TimeManeger.OnHourChanged += UpdateTimer;
    }

    private void StopTimer()
    {
        TimeManeger.OnMinuteChanged -= UpdateTimer;
        TimeManeger.OnHourChanged -= UpdateTimer;
    }
    private void UpdateTimer()
    {
        timer.text = $"{TimeManeger.Hour:00}:{TimeManeger.Minute:00}";
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

        gameplay.changeMode(Gameplay.GameMode.Scout);
        gameplay.grid.gameObject.SetActive(false);
    }
    public void SwitchToBuildCategory()
    {
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
        inventoryUI.SetActive(false);
        buildcategoryUI.SetActive(false);
        menuTab.SetActive(false);
        goalButton.SetActive(false);
        settingsButton.SetActive(false);
    }
    public void CloseBuildConfirmation()
    {
        buildConfirmationUI.SetActive(false);
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