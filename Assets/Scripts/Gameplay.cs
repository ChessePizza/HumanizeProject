using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum GameState
{
    Start = 0,
    Prepare = 1,
    Wave1 = 2,
    Wave2 = 3,
    Wave3 = 4,
    End = 5,
    GameOver = 6
};

[RequireComponent(typeof(GameUI))]
public class Gameplay : MonoBehaviour
{

    public enum GameMode 
    {
        Scout = 0,
        Build = 1
    };

    [Header("Level Scene")]
    public int levelId = 0;
    public string levelName;
    public string levelDescription;

    [Header("Player")]
    public Health health;

    public const int MAX_INVENTORY = 7;
    public GameObject[] inventory;

    public GameMode mode;
    public GameState state;

    [Header("References")]
    public GridMap grid;
    public Spawner spawner;
    public GameObject storage;
    public QuestManager quest;

    GameObject buildSlot;
    GameUI gameUI;
    CameraController controller;

    [Header("Data")]
    public bool pause = false;
    public bool isSuccessfullyBuildOnce = false;
    public int maxItem;
    public int countItem = 0;
    public int killCount = 0;
    public Vector2Int startTime;

    public Vector2 gridSize;
    public Vector2 cellSize;

    
    
    void Start()
    {
        isSuccessfullyBuildOnce = false;
        pause = true; 

        inventory = new GameObject[MAX_INVENTORY];
        gameUI = GetComponent<GameUI>();
        gameUI.HideAll();

        controller = GetComponent<CameraController>();
        ResetKill();

        // คำนวนขนาดไว้ล่วงหน้า เนื่องจากขนาดของฉากแผนที่ในเกม ไม่มีการเปลี่ยนแปลงระหว่างเล่น
        Sprite sprite = grid.level.GetComponentInParent<SpriteRenderer>().sprite;
        Rect rect = sprite.rect;

        // คำนวนหาขนาดของ Grid ในหน่วย Unity Unit
        gridSize = new Vector2(rect.width / sprite.pixelsPerUnit, rect.height / sprite.pixelsPerUnit);
        // คำนวณหาขนาดของ Cell ในหน่วย Unity Unit
        cellSize = new Vector2(rect.width / sprite.pixelsPerUnit / grid.size.x, rect.height / sprite.pixelsPerUnit / grid.size.y);
        InitState(GameState.Start);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(5);
        quest.isReady = true;
        quest.ResetRoute();
        yield return null;
    }

    void Update()
    {
        UpdateBuild();
        UpdateState();
    }
    
    // Game State Functions

    public void InitState(GameState state)
    {
        this.state = state;

        string title = levelName;
        string description = levelDescription;
        switch (state) {
            case GameState.Prepare: 
                title = "Build Your Base"; 
                description = "Prepare For The Night";
                spawner.itemTimer = spawner.cooldownItemSpawnPrepare;
                break;
            case GameState.Wave1: 
                title = "Wave 1"; 
                description = "Sunset Horde";
                spawner.itemTimer = spawner.cooldownItemSpawnWave1;
                break;
            case GameState.Wave2: 
                title = "Wave 2"; 
                description = "Midnight Crisis";
                spawner.itemTimer = spawner.cooldownItemSpawnWave2;
                break;
            case GameState.Wave3: 
                title = "Wave 3"; 
                description = "Until Dawn";
                spawner.itemTimer = spawner.cooldownItemSpawnWave3;
                break;
            case GameState.End: 
                title = "Survive"; 
                description = "For This Night";
                break;
            case GameState.GameOver:
                title = "You Died";
                description = "Game Over!";
                break;
            default: break;
        }

        gameUI.Announce(title, description);
    }

    public void UpdateState()
    {
        if (health.currentHealth <= 0)
        {
            gameUI.SetGameEnd("Game Over", "Your base is broken.\nAll hope is gone.\nYou killed " + killCount + " enemies.", true);
            gameUI.HideAll();
            InitState(GameState.GameOver);
            StartCoroutine(EndGame());
        }

        if (TimeManeger.Hour >= 20 && TimeManeger.Hour <= 23 && state != GameState.Wave1)
        {
            // wave 1
            InitState(GameState.Wave1);
        }
        else if (TimeManeger.Hour >= 0 && TimeManeger.Hour < 4 && state != GameState.Wave2)
        {
            // Wave 2
            InitState(GameState.Wave2);
        }
        else if (TimeManeger.Hour >= 4 && TimeManeger.Hour < 6 && state != GameState.Wave3)
        {
            // Wave 3
            InitState(GameState.Wave3);
        }
        else if (TimeManeger.Hour >= 6 && TimeManeger.Hour < 9 && state != GameState.End)
        {
            // End Game
            bool win = quest.CheckQuest();
            if (win)
            {
                gameUI.SetGameEnd("Congratuation", "You survived the night!\nYou killed " + killCount + " enemies.", false);
                gameUI.HideAll();
                InitState(GameState.End);
                StartCoroutine(EndGame());
            }
            else
            {
                gameUI.SetGameEnd("Game Over", "Objective is not fulfilled.\nAll hope is gone.\nYou killed " + killCount + " enemies.", true);
                gameUI.HideAll();
                InitState(GameState.GameOver);
                StartCoroutine(EndGame());
            }
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3);
        gameUI.backgroundUI.SetActive(true);
        gameUI.endUI.SetActive(true);

        yield return null;
    }

    // Functions

    public void Pause()
    {
        pause = true;
    }

    public void Resume()
    {
        pause = false;
    }

    public void TogglePause()
    {
        pause = !pause;
    }

    public void ResetKill()
    {
        killCount = 0;
        gameUI.killCountText.text = killCount.ToString();
    }

    public void updateKill()
    {
        killCount++;
        gameUI.killCountText.text = killCount.ToString();
    }

    // Item Functions

    public int getEmptySlot() 
    {
        for (int i = 0; i < MAX_INVENTORY; i++)
        {
            if (!inventory[i]) return i;
        }
        return -1;
    }
    
    public bool findItem(ItemType type)
    {
        for (int i = 0; i < MAX_INVENTORY; i++)
        {
            if (inventory[i])
            {
                if (inventory[i].GetComponent<PickableItem>().type == type)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void clearItem()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i])
            {
                GameObject icon = gameUI.inventoryUI.transform.Find("Slot" + (i + 1) + "/Icon").gameObject;
                Destroy(icon.GetComponent<Button>());
                Destroy(icon.GetComponent<Image>());
                Destroy(icon);

                GameObject item = inventory[i];
                inventory[i] = null;
                spawner.currentItem--;
                Destroy(item);
            }
        }
    }

    public void addItem(PickableItem item,int slot)
    {
        //สร้าง Icon ของ Item เพื่อใส่ไว้ใน Slot
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(gameUI.inventoryUI.transform.Find("Slot" + (slot + 1)));

        RectTransform rect = icon.gameObject.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0.0f, 0.0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchorMin = new Vector2(0.0f, 0.0f);
        rect.anchorMax = new Vector2(1.0f, 1.0f);
        rect.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Image image = icon.gameObject.AddComponent<Image>();
        image.sprite = item.GetComponent<SpriteRenderer>().sprite;

        Button button = icon.gameObject.AddComponent<Button>();
        button.interactable = true;
        button.image = image;
        button.onClick.AddListener(() => discardItem(slot, false));

        inventory[slot] = item.gameObject;
        item.gameObject.SetActive(false);
    }

    public void discardItem(int slot, bool fulfillment) 
    {
        GameObject icon = gameUI.inventoryUI.transform.Find("Slot" + (slot + 1) + "/Icon").gameObject;
        Destroy(icon.GetComponent<Button>());
        Destroy(icon.GetComponent<Image>());
        Destroy(icon);

        GameObject item = inventory[slot];
        inventory[slot] = null;
        if (fulfillment)
        {
            spawner.currentItem--;
            Destroy(item);
        }
        else
        {
            Vector3 position = controller.player.transform.position;
            item.transform.position = position - new Vector3(0.0f, 2.5f, 2.5f);
            item.SetActive(true);
            item.GetComponent<PickableItem>().slot = -1;
        }
    }

    public void changeMode(GameMode mode) 
    {
        this.mode = mode;
    }

    // Building Functions

    public void build(Building building) 
    {
        gameUI.CloseBuildInfo();
        gameUI.OpenBuildConfirmation();

        buildSlot = new GameObject();
        buildSlot.name = "BuildSlot";
        GameObject entity = Instantiate(building.gameObject, Vector3.zero, Quaternion.identity);
        entity.name = building.name;
        entity.GetComponent<Building>().girdPosition = new Vector2Int(-1, -1);
        entity.transform.SetParent(buildSlot.transform);
        entity.transform.position += new Vector3(building.positionAdjust.x, building.positionAdjust.y, 0.0f);               
    }

    public void addBuild()
    {
        GameObject baseBuilder = buildSlot.gameObject;
        Building building = baseBuilder.transform.GetChild(0).GetComponent<Building>();
        building.isReady = false;
        building.grid = grid;
        building.transform.Find("Fulfillment").gameObject.SetActive(true);

        // Update Grid Map
        grid.data[(building.girdPosition.x * grid.size.y) + building.girdPosition.y] = 2;
        gameUI.UpdatePassability();

        // Add Building TO Grid
        baseBuilder.name = "build_" + building.girdPosition.x + "_" + building.girdPosition.y;
        baseBuilder.transform.SetParent(storage.transform);
        buildSlot = null;
        gameUI.CloseBuildConfirmation();
    }

    public void discardBuild()
    {
        Destroy(buildSlot.gameObject);
        buildSlot = null;
        gameUI.CloseBuildConfirmation();
    }

    void UpdateBuild()
    {
        if (buildSlot)
        {
            Vector3 position = controller.player.transform.position;

            Vector2Int gridPoint = new Vector2Int(
            (int)Mathf.Floor(((gridSize.x / 2) + position.x) / cellSize.x),
            (int)Mathf.Floor(((gridSize.y / 2) - position.y) / cellSize.y));

            Vector3 point = new Vector3(
                (-gridSize.x / 2) + ((gridPoint.x * cellSize.x) + (cellSize.x / 2)),
                 (gridSize.y / 2) - ((gridPoint.y * cellSize.y) + (cellSize.y / 2)), 1);

            if (grid.data[(gridPoint.x * grid.size.y) + gridPoint.y] == 0)
            {
                buildSlot.transform.GetChild(0).GetComponent<Building>().girdPosition = gridPoint;
                gameUI.buildConfirmButton.gameObject.SetActive(true);
            }
            else
            {
                gameUI.buildConfirmButton.gameObject.SetActive(false);
            }

            buildSlot.transform.position = point;
        }
    }
}
