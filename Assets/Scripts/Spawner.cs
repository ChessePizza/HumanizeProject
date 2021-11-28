using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public const int MAX_ENEMIES = 3;
    public const int MAX_ITEMS = 5;
    [Header("Max Enemies Spawn Settings")]
    public int[] maxEnemiesSpawnPerTypeWave1 = new int[MAX_ENEMIES];
    public int[] maxEnemiesSpawnPerTypeWave2 = new int[MAX_ENEMIES];
    public int[] maxEnemiesSpawnPerTypeWave3 = new int[MAX_ENEMIES];

    [Header("Cooldown Spawn Enemies Settings")]
    public int[] cooldownEnemiesSpawnPerTypeWave1 = new int[MAX_ENEMIES];
    public int[] cooldownEnemiesSpawnPerTypeWave2 = new int[MAX_ENEMIES];
    public int[] cooldownEnemiesSpawnPerTypeWave3 = new int[MAX_ENEMIES];

    [Header("Max Item Spawn Settings")]
    public int maxItemSpawnPrepare;
    public int maxItemSpawnWave1;
    public int maxItemSpawnWave2;
    public int maxItemSpawnWave3;

    [Header("Cooldown Item Spawn Settings")]
    public int cooldownItemSpawnPrepare;
    public int cooldownItemSpawnWave1;
    public int cooldownItemSpawnWave2;
    public int cooldownItemSpawnWave3;

    [Header("Game Object References")]
    public GameObject[] Enemies = new GameObject[MAX_ENEMIES];
    public GameObject[] Items = new GameObject[MAX_ITEMS];
    public GameObject paper;
    bool isPaperSpawn = false;

    [Header("Runtime Data")]
    public int currentItem = 0;
    public int[] currentEnemies = new int[MAX_ENEMIES];
    public int itemTimer = 0;
    public int[] enemiesTimer = new int[MAX_ENEMIES];

    Gameplay gameplay;
    GridMap grid;

    Vector2 gridSize;
    Vector2 cellSize;

    // Start is called before the first frame update
    void Start()
    {
        isPaperSpawn = false;
        currentEnemies = new int[MAX_ENEMIES];
        enemiesTimer = new int[MAX_ENEMIES];

        gameplay = Camera.main.GetComponent<Gameplay>();
        grid = gameplay.grid;

        // คำนวนขนาดไว้ล่วงหน้า เนื่องจากขนาดของฉากแผนที่ในเกม ไม่มีการเปลี่ยนแปลงระหว่างเล่น
        Sprite sprite = grid.level.GetComponentInParent<SpriteRenderer>().sprite;
        Rect rect = sprite.rect;

        // คำนวนหาขนาดของ Grid ในหน่วย Unity Unit
        gridSize = new Vector2(rect.width / sprite.pixelsPerUnit, rect.height / sprite.pixelsPerUnit);
        // คำนวณหาขนาดของ Cell ในหน่วย Unity Unit
        cellSize = new Vector2(rect.width / sprite.pixelsPerUnit / grid.size.x, rect.height / sprite.pixelsPerUnit / grid.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameplay.pause)
        {
            bool spawnable = false;

            if (!isPaperSpawn && TimeManeger.Hour == 23)
            {
                isPaperSpawn = true;
                Vector2Int gridPoint = new Vector2Int(0, 0);
                UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                int itemType = UnityEngine.Random.Range(0, MAX_ITEMS);
                do
                {
                    // หาจนกว่าจะเจอที่ว่าง
                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                    gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                    gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                    spawnable = isGridEmpty(gridPoint);
                } while (!spawnable);

                SpawnItem(paper, gridPoint);
            }
            // Error prevention
            if (currentItem < 0) currentItem = 0;
            for (int i = 0; i < MAX_ENEMIES; i++) if (currentEnemies[i] < 0) currentEnemies[i] = 0;

            switch (gameplay.state)
            {
                case GameState.Prepare:
                    // Prepare Spawn Items
                    if (itemTimer >= cooldownItemSpawnPrepare && currentItem < maxItemSpawnPrepare)
                    {
                        itemTimer = 0;
                        Vector2Int gridPoint = new Vector2Int(0, 0);
                        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                        int itemType = UnityEngine.Random.Range(0, MAX_ITEMS);
                        do
                        {
                            // หาจนกว่าจะเจอที่ว่าง
                            UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                            gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                            gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                            spawnable = isGridEmpty(gridPoint);
                        } while (!spawnable);

                        // Spawn
                        SpawnItem(Items[itemType], gridPoint);
                        currentItem++;
                    }
                    break;
                case GameState.Wave1:
                    // Wave 1 Spawn Items
                    if (itemTimer >= cooldownItemSpawnWave1 && currentItem < maxItemSpawnWave1)
                    {
                        itemTimer = 0;
                        Vector2Int gridPoint = new Vector2Int(0, 0);
                        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                        int itemType = UnityEngine.Random.Range(0, MAX_ITEMS);
                        do
                        {
                            // หาจนกว่าจะเจอที่ว่าง
                            UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                            gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                            gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                            spawnable = isGridEmpty(gridPoint);
                        } while (!spawnable);
                        // Spawn
                        SpawnItem(Items[itemType], gridPoint);
                        currentItem++;
                    }
                    // Wave 1 Spawn Enemies
                    for (int i = 0; i < MAX_ENEMIES; i++)
                    {
                        if (enemiesTimer[i] >= cooldownEnemiesSpawnPerTypeWave1[i] && currentEnemies[i] < maxEnemiesSpawnPerTypeWave1[i])
                        {
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
                                    gridPoint.y = side ? 0 : grid.size.y - 1;
                                }
                                else
                                {
                                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 2);
                                    gridPoint.x = side ? 0 : grid.size.x - 1;
                                    gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                                }
                                spawnable = isGridEmpty(gridPoint);
                            } while (!spawnable);
                            // Spawn
                            SpawnEnemy(Enemies[i], gridPoint);
                            currentEnemies[i]++;
                        }
                    }
                    break;
                case GameState.Wave2:
                    // Wave 2 Spawn Items
                    if (itemTimer >= cooldownItemSpawnWave2 && currentItem < maxItemSpawnWave2)
                    {
                        itemTimer = 0;
                        Vector2Int gridPoint = new Vector2Int(0, 0);
                        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                        int itemType = UnityEngine.Random.Range(0, MAX_ITEMS);
                        do
                        {
                            // หาจนกว่าจะเจอที่ว่าง
                            UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                            gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                            gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                            spawnable = isGridEmpty(gridPoint);
                        } while (!spawnable);
                        // Spawn
                        SpawnItem(Items[itemType], gridPoint);
                        currentItem++;
                    }
                    // Wave 2 Spawn Enemies
                    for (int i = 0; i < MAX_ENEMIES; i++)
                    {
                        if (enemiesTimer[i] >= cooldownEnemiesSpawnPerTypeWave2[i] && currentEnemies[i] < maxEnemiesSpawnPerTypeWave2[i])
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
                                    gridPoint.y = side ? 0 : grid.size.y - 1;
                                }
                                else
                                {
                                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 2);
                                    gridPoint.x = side ? 0 : grid.size.x - 1;
                                    gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                                }
                                spawnable = isGridEmpty(gridPoint);
                            } while (!spawnable);
                            // Spawn
                            SpawnEnemy(Enemies[i], gridPoint);
                            currentEnemies[i]++;
                        }
                    }
                    break;
                case GameState.Wave3:
                    // Wave 3 Spawn Items
                    if (itemTimer >= cooldownItemSpawnWave3 && currentItem < maxItemSpawnWave3)
                    {
                        spawnable = false;
                        itemTimer = 0;
                        Vector2Int gridPoint = new Vector2Int(0, 0);
                        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                        int itemType = UnityEngine.Random.Range(0, MAX_ITEMS);
                        do
                        {
                            // หาจนกว่าจะเจอที่ว่าง
                            UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 1);
                            gridPoint.x = UnityEngine.Random.Range(1, grid.size.x - 2);
                            gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                            spawnable = isGridEmpty(gridPoint);
                        } while (!spawnable);
                        // Spawn
                        SpawnItem(Items[itemType], gridPoint);
                        currentItem++;
                    }
                    // Wave 3 Spawn Enemies
                    for (int i = 0; i < MAX_ENEMIES; i++)
                    {
                        if (enemiesTimer[i] >= cooldownEnemiesSpawnPerTypeWave3[i] && currentEnemies[i] < maxEnemiesSpawnPerTypeWave3[i])
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
                                    gridPoint.y = side ? 0 : grid.size.y - 1;
                                }
                                else
                                {
                                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks + 2);
                                    gridPoint.x = side ? 0 : grid.size.x - 1;
                                    gridPoint.y = UnityEngine.Random.Range(1, grid.size.y - 2);
                                }
                                spawnable = isGridEmpty(gridPoint);
                            } while (!spawnable);
                            // Spawn
                            SpawnEnemy(Enemies[i], gridPoint);
                            currentEnemies[i]++;
                        }
                    }
                    break;

                default: break;
            }

            for (int i = 0; i < MAX_ENEMIES; i++) enemiesTimer[i]++;
            itemTimer++;
        }
    }


    public bool isGridEmpty(Vector2Int gridPoint)
    {
        if (grid.data[(gridPoint.x * grid.size.y) + gridPoint.y] == 0)
        {
            return true;
        }
        return false;
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
}

