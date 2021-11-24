using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        inventory = new GameObject[MAX_INVENTORY];
    }

    // Update is called once per frame
    void Update()
    {
        
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
        GetComponent<GameUI>().CloseBuildInfo();
        GameObject o = Instantiate(building.gameObject, Vector3.zero, Quaternion.identity);
        
        gameObject.transform.Find("Temp");
    }

	internal void build(object building)
	{
		throw new NotImplementedException();
	}
}
