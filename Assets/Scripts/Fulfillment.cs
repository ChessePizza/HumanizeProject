using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fulfillment : MonoBehaviour
{
    public const int MAX_RESOURCE = 5;
    public Building building;
    public Text[] text;
    public int[] requiredAmount; // ต้องกรอกมาก่อน
    public SpriteRenderer[] sprites;
    public Color color; // สีขณะที่อยู่ระหว่างการเติมเต็ม

    bool inRange = false;
    int[] amount;
    Gameplay gameplay;

    // Start is called before the first frame update
    void Start()
    {
        amount = new int[MAX_RESOURCE];
        for (int i = 0; i < MAX_RESOURCE; i++)
        {
            text[i].text = amount[i] + "/" + requiredAmount[i];
        }
        gameplay = Camera.main.GetComponent<Gameplay>();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            bool needUpdate = false;
            for (int i = 0; i < Gameplay.MAX_INVENTORY; i++)
            {
                if (gameplay.inventory[i])
                {
                    PickableItem item = gameplay.inventory[i].GetComponent<PickableItem>();
                    if (amount[(int)item.type] < requiredAmount[(int)item.type])
                    {
                        // ต้องการ fulfillment
                        gameplay.discardItem(i, true);
                        amount[(int)item.type]++;
                        text[(int)item.type].text = amount[(int)item.type] + "/" + requiredAmount[(int)item.type];
                        needUpdate = true;
                    }
                }
            }

            if (needUpdate)
            {
                int countFulfillment = 0;
                for (int i = 0; i < MAX_RESOURCE; i++)
                {
                    if (amount[i] >= requiredAmount[i])
                    {
                        countFulfillment++;
                    }
                }
                // ถ้าทรัพยากรเติมเต็มจนครบก็จะสามารถสร้างได้
                if (countFulfillment >= MAX_RESOURCE) Fulfill();
            }
        }
    }

    void Fulfill()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = Color.white;
        }
        
        building.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        building.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        building.transform.Find("Health").gameObject.SetActive(true);
        Health health = building.gameObject.GetComponent<Health>();
        health.enabled = true;
        health.SetMaxHealth(building.durability);

        building.isReady = true;
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
}
