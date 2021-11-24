using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameUI))]
public class Gameplay : MonoBehaviour
{
    public const int MAX_INVENTORY = 7;

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
        item.transform.SetParent(GetComponent<GameUI>().inventoryUI.transform.Find("Slot" + (slot + 1)));

        RectTransform rect = item.gameObject.AddComponent<RectTransform>();
        rect.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        rect.anchoredPosition = new Vector2(0.0f, 0.0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchorMin = new Vector2(0.0f, 0.0f);
        rect.anchorMax = new Vector2(1.0f, 1.0f);

        Image image = item.gameObject.AddComponent<Image>();
        image.sprite = item.GetComponent<SpriteRenderer>().sprite;

        Button button = item.gameObject.AddComponent<Button>();
        button.interactable = true;
        button.image = image;
        button.onClick.AddListener(() => discardItem(slot));

        inventory[slot] = item.gameObject;
    }

    public void discardItem(int slot) {
        Debug.Log("Discard:" + slot);
        //inventory[slot] = null;
    }
}
