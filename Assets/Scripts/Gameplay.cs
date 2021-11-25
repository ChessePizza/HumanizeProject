using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(GameUI))]
public class Gameplay : MonoBehaviour
{
    public enum GameMode 
    {
        Scout = 0,
        Build = 1
    };

    public const int MAX_INVENTORY = 7;

    public GameMode mode;
    public GridMap grid;
    public GameObject[] inventory;
    public int maxItem;
    public int countItem;

    GameObject buildSlot;

    public Vector2 gridSize;
    public Vector2 cellSize;

    void Start()
    {
        inventory = new GameObject[MAX_INVENTORY];

        // คำนวนขนาดไว้ล่วงหน้า เนื่องจากขนาดของฉากแผนที่ในเกม ไม่มีการเปลี่ยนแปลงระหว่างเล่น
        Sprite sprite = grid.level.GetComponentInParent<SpriteRenderer>().sprite;
        Rect rect = sprite.rect;

        // คำนวนหาขนาดของ Grid ในหน่วย Unity Unit
        gridSize = new Vector2(rect.width / sprite.pixelsPerUnit, rect.height / sprite.pixelsPerUnit);
        // คำนวณหาขนาดของ Cell ในหน่วย Unity Unit
        cellSize = new Vector2(rect.width / sprite.pixelsPerUnit / grid.size.x, rect.height / sprite.pixelsPerUnit / grid.size.y);
    }


    void Update()
    {
        if (buildSlot)
        {
            Vector3 position = GetComponent<CameraController>().player.transform.position;

            Vector2Int gridPoint = new Vector2Int(
            (int)Mathf.Floor(((gridSize.x / 2) + position.x) / cellSize.x),
            (int)Mathf.Floor(((gridSize.y / 2) - position.y) / cellSize.y));

            Vector3 point = new Vector3(
                (-gridSize.x / 2) + ((gridPoint.x * cellSize.x) + (cellSize.x / 2)),
                 (gridSize.y / 2) - ((gridPoint.y * cellSize.y) + (cellSize.y / 2)), 1);

            if (grid.data[(gridPoint.x * grid.size.y) + gridPoint.y] == 0)
            {
                buildSlot.transform.GetChild(0).GetComponent<Building>().girdPosition = gridPoint;
                GetComponent<GameUI>().buildConfirmButton.gameObject.SetActive(true);
            }
            else
            {
                GetComponent<GameUI>().buildConfirmButton.gameObject.SetActive(false);
            }

            buildSlot.transform.position = point;
        }
    }
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
        icon.transform.SetParent(GetComponent<GameUI>().inventoryUI.transform.Find("Slot" + (slot + 1)));

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
        button.onClick.AddListener(() => discardItem(slot));

        inventory[slot] = item.gameObject;
        item.gameObject.SetActive(false);
    }

    public void discardItem(int slot) 
    {
        GameObject icon = GetComponent<GameUI>().inventoryUI.transform.Find("Slot" + (slot + 1) + "/Icon").gameObject;
        Destroy(icon.GetComponent<Button>());
        Destroy(icon.GetComponent<Image>());
        Destroy(icon);

        Vector3 position = GetComponent<CameraController>().player.transform.position;

        GameObject item = inventory[slot];
        inventory[slot] = null;
        item.transform.position = position - new Vector3(0.0f, 2.5f, 2.5f);
        item.SetActive(true);
        item.GetComponent<PickableItem>().slot = -1;
    }

    public void changeMode(GameMode mode) 
    {
        this.mode = mode;
    }

    public void build(Building building) 
    {
        GameUI gameUI = GetComponent<GameUI>();
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
        building.gameObject.GetComponent<BoxCollider2D>().enabled = true;

        // Update Grid Map
        grid.data[(building.girdPosition.x * grid.size.y) + building.girdPosition.y] = 2;
        GetComponent<GameUI>().UpdatePassability();

        // Add Building TO Grid
        baseBuilder.name = "build_" + building.girdPosition.x + "_" + building.girdPosition.y;
        baseBuilder.transform.SetParent(grid.transform);
        buildSlot = null;
        GetComponent<GameUI>().CloseBuildConfirmation();
    }

    public void discardBuild()
    {
        Destroy(buildSlot.gameObject);
        buildSlot = null;
        GetComponent<GameUI>().CloseBuildConfirmation();
    }
}
