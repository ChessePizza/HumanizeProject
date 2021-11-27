using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(GameUI))]
public class Gameplay : MonoBehaviour
{
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

    public enum GameMode 
    {
        Scout = 0,
        Build = 1
    };

    public int level = 0;
    public Health baseHealth;

    public const int MAX_ENEMIES = 3;
    public const int MAX_ITEMS = 5;
    public int[] maxEnemiesSpawnPerTypeWave1 = new int[MAX_ENEMIES];
    public int[] maxEnemiesSpawnPerTypeWave2 = new int[MAX_ENEMIES];
    public int[] maxEnemiesSpawnPerTypeWave3 = new int[MAX_ENEMIES];
    public int[] cooldownEnemiesSpawnPerTypeWave1 = new int[MAX_ENEMIES];
    public int[] cooldownEnemiesSpawnPerTypeWave2 = new int[MAX_ENEMIES];
    public int[] cooldownEnemiesSpawnPerTypeWave3 = new int[MAX_ENEMIES];

    public GameObject[] Enemies = new GameObject[MAX_ENEMIES];
    public GameObject[] Items = new GameObject[MAX_ITEMS];

    public int maxItemSpawnPrepare;
    public int maxItemSpawnWave1;
    public int maxItemSpawnWave2;
    public int maxItemSpawnWave3;
    public int cooldownItemSpawnPrepare;
    public int cooldownItemSpawnWave1;
    public int cooldownItemSpawnWave2;
    public int cooldownItemSpawnWave3;

    int currentItem = 0;
    int[] currentEnemies = new int[MAX_ENEMIES];
    int itemTimer = 0;
    int[] enemiesTimer = new int[MAX_ENEMIES];

    public const int MAX_INVENTORY = 7;

    public GameMode mode;
    public GameState state;
    public bool isStateReady = false;
    public GridMap grid;
    public GameObject storage;
    public GameObject[] inventory;
    public int maxItem;
    public int countItem = 0;
    public int killCount = 0;
    public Vector2Int startTime;

    GameObject buildSlot;
    GameUI gameUI;
    CameraController controller;

    //จริงๆต้องอยู่ใน GameUI แต่ไม่ทันแล้ว
    public Text endTitle;
    public Image endIcon;
    public Text endInfo;
    public GameObject endWindow;
    public Sprite gameOver;
    public Sprite gameWin;

    public Image warning;
    bool attacking;

    public Vector2 gridSize;
    public Vector2 cellSize;

    public void showWarning()
    {
        warning.gameObject.SetActive(true);
        if (!attacking)
        {
            StartCoroutine(EndWarning());
        }
    }

    IEnumerator EndWarning()
    {
        attacking = true;
        yield return new WaitForSeconds(7);
        warning.gameObject.SetActive(false);
        attacking = false;
        yield return null;
    }

    void Start()
    {
        currentEnemies = new int[MAX_ENEMIES];
        enemiesTimer = new int[MAX_ENEMIES];

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
        yield return new WaitForSeconds(7);
        gameUI.ShowAll();
        yield return new WaitForSeconds(2);
        InitState(GameState.Prepare);
        yield return new WaitForSeconds(2);
        gameUI.SetTimer(startTime.x, startTime.y);
        gameUI.StartTimer();

        yield return null;
    }

    void Update()
    {
        UpdateBuild();
        UpdateState();
        UpdateSpawner();
    }

    public void UpdateSpawner()
    {
        bool spawnable = false;
        // Error prevention
        if (currentItem < 0) currentItem = 0;
        for(int i = 0; i < MAX_ENEMIES; i++) if (currentEnemies[i] < 0) currentEnemies[i] = 0;

        switch (state) {
            case GameState.Prepare:
                if (itemTimer >= cooldownItemSpawnPrepare && currentItem < maxItemSpawnPrepare)
                {
                    spawnable = false;
                    itemTimer = 0;
                    Vector2Int gridPoint = new Vector2Int(0, 0);
                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                    int itemType = UnityEngine.Random.Range(0, MAX_ITEMS);
                    do {
                        // หาจนกว่าจะเจอที่ว่าง
                        UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                        gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                        gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                        spawnable = canSpawnEntity(gridPoint);
                    } while (!spawnable);
                    // Spawn
                    SpawnItem(Items[itemType], gridPoint);
                    currentItem++;
                }
                for (int i = 0; i < MAX_ENEMIES; i++)
                {
                    if (enemiesTimer[i] >= cooldownEnemiesSpawnPerTypeWave1[i] && currentEnemies[i] < maxEnemiesSpawnPerTypeWave1[i])
                    {
                        spawnable = false;
                        enemiesTimer[i] = 0;
                        Vector2Int gridPoint = new Vector2Int(0, 0);
                        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                        do
                        {
                            // หาจนกว่าจะเจอที่ว่าง
                            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                            bool vertical = UnityEngine.Random.value > 0.5f; ;
                            UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                            bool side = UnityEngine.Random.value > 0.5f; ;
                            if (vertical)
                            {
                                UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 2);
                                gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                                gridPoint.y = side ? 0: grid.size.y - 1;
                            }
                            else
                            {
                                UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 2);
                                gridPoint.x = side ? 0 : grid.size.x - 1;
                                gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                            }
                            spawnable = canSpawnEntity(gridPoint);
                        } while (!spawnable);
                        // Spawn
                        SpawnEnemy(Enemies[i], gridPoint);
                        currentEnemies[i]++;
                    }
                }
                break;
            case GameState.Wave1:
                break;
            default: break;        
        }

        for (int i = 0; i < MAX_ENEMIES; i++) enemiesTimer[i]++;
        itemTimer++;
    }

    public void SpawnEnemy(GameObject baseEntity, Vector2Int gridPoint)
    {
        Vector3 point = new Vector3(
                (-gridSize.x / 2) + ((gridPoint.x * cellSize.x) + (cellSize.x / 2)),
                 (gridSize.y / 2) - ((gridPoint.y * cellSize.y) + (cellSize.y / 2)), 1);

        GameObject entity = Instantiate(baseEntity, point, Quaternion.identity);
        entity.transform.SetParent(grid.transform.parent.transform);
        entity.GetComponent<AIDestinationSetter>().target = grid.transform.parent.Find("Base/Target");
    }

    public void SpawnItem(GameObject baseEntity, Vector2Int gridPoint)
    {
        Vector3 point = new Vector3(
                (-gridSize.x / 2) + ((gridPoint.x * cellSize.x) + (cellSize.x / 2)),
                 (gridSize.y / 2) - ((gridPoint.y * cellSize.y) + (cellSize.y / 2)), 1);

        GameObject entity = Instantiate(baseEntity, point, Quaternion.identity);
        entity.transform.SetParent(grid.transform.parent.transform);
    }
    public bool canSpawnEntity(Vector2Int gridPoint)
    {
        if (grid.data[(gridPoint.x * grid.size.y) + gridPoint.y] == 0)
        {
            return true;
        }
        return false;
    }

    // Game State Functions

    public void InitState(GameState state)
    {
        this.state = state;
        this.isStateReady = false;

        string title = gameUI.levelName;
        string description = gameUI.levelDescription;
        switch (state) {
            case GameState.Prepare: 
                title = "Build Your Base"; 
                description = "Prepare For The Night";
                itemTimer = cooldownItemSpawnPrepare;
                break;
            case GameState.Wave1: 
                title = "Wave 1"; 
                description = "Easy One";
                itemTimer = cooldownItemSpawnWave1;
                break;
            case GameState.Wave2: 
                title = "Wave 2"; 
                description = "Midnight Crisis";
                itemTimer = cooldownItemSpawnWave2;
                break;
            case GameState.Wave3: 
                title = "Wave 3"; 
                description = "Last Stage";
                itemTimer = cooldownItemSpawnWave3;
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
        if (baseHealth.currentHealth <= 0)
        {
            endTitle.text = "Game Over";
            endIcon.sprite = gameOver;
            endInfo.text = "Your base is broken.";
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
        else if (TimeManeger.Hour >= 6 && TimeManeger.Hour < 15 && state != GameState.End)
        {
            // End Game
            bool win = passObjective();
            if (win)
            {
                endTitle.text = "Win";
                endIcon.sprite = gameWin;
                endInfo.text = "Congratuation! You survived!";
                gameUI.HideAll();
                InitState(GameState.End);
                StartCoroutine(EndGame());
            }
            else
            {
                endTitle.text = "Game Over";
                endIcon.sprite = gameOver;
                endInfo.text = "You seems to forget something?\n(Objective is not fulfilled.)";
                gameUI.HideAll();
                InitState(GameState.GameOver);
                StartCoroutine(EndGame());
            }
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(7);
        gameUI.backgroundUI.SetActive(true);
        endWindow.SetActive(true);

        yield return null;
    }

    bool passObjective()
	{
        if (level == 0) return true;
        for (int i = 0; i < MAX_INVENTORY; i++)
        {
            if (inventory[i].GetComponent<PickableItem>().type == ItemType.paper)
            {
                return true;
            }
        }
        return false;
	}

    // Functions
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
            currentItem--;
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
