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
    public GameObject settingsUI;
    public GameObject menuListUI;
    public GameObject goalUI;
    public GameObject backgroundUI;
    public GameObject impassable;
    public Vector2 magicNumber; // 0.09, 0.39
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

        Gameplay gameplay = GetComponent<Gameplay>();
        gameplay.changeMode(Gameplay.GameMode.Scout);
        gameplay.grid.gameObject.SetActive(false);
    }
    public void SwitchToBuildCategory()
    {
        buildcategoryUI.SetActive(true);
        inventoryUI.SetActive(false);
        inventoryButton.color = new Color(0.45f, 0.45f, 0.45f, 1.0f);
        buildcategoryButton.color = Color.white;

        Gameplay gameplay = GetComponent<Gameplay>();
        gameplay.changeMode(Gameplay.GameMode.Build);
        UpdatePassability();
        gameplay.grid.gameObject.SetActive(true);
    }

    public void UpdatePassability()
    {
        Gameplay gameplay = GetComponent<Gameplay>();

        Sprite sprite = gameplay.grid.level.GetComponentInParent<SpriteRenderer>().sprite;
        Rect rect = sprite.rect;

        // คำนวนหาขนาดของ Grid
        Vector2Int size = new Vector2Int((int)(rect.width / sprite.pixelsPerUnit), (int)(rect.height / sprite.pixelsPerUnit));
        // คำนวณหาขนาดของ Cell
        Vector2 cellSize = new Vector2(rect.width / sprite.pixelsPerUnit / gameplay.grid.size.x, rect.height / sprite.pixelsPerUnit / gameplay.grid.size.y);

        for (int x = 0; x < gameplay.grid.size.x; x++)
            for (int y = 0; y < gameplay.grid.size.y; y++)
            {
                if (gameplay.grid.data[(x * gameplay.grid.size.y) + y] == 0)
                {
                    if (!gameplay.grid.transform.Find("impassable_" + x + "_" + y))
                    {
                        GameObject o = Instantiate(impassable, new Vector3(0,0,0), Quaternion.identity);
                        o.name = "impassable_" + x + "_" + y;
                        o.transform.position = new Vector3(
                            (cellSize.x * x) - (size.x / 2.0f) - magicNumber.x,
                            (cellSize.y * (gameplay.grid.size.y - y)) - (size.y / 2.0f) - magicNumber.y,
                            1.0f);
                        o.transform.SetParent(gameplay.grid.transform);
                    }
                }
            }
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
        button.onClick.AddListener(() => GetComponent<Gameplay>().build(item.building));

        backgroundUI.SetActive(true);
    }

    public void CloseBuildInfo()
    {
        buildInfoUI.SetActive(false);
        backgroundUI.SetActive(false);
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