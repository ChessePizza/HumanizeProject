using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    GameUI gameUI;
    bool inRange = false;
    Gameplay gameplay;
    // Start is called before the first frame update
    void Start()
    {
        gameUI = Camera.main.GetComponent<GameUI>();
        gameplay = Camera.main.GetComponent<Gameplay>();
        inRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameplay.pause)
        {
            gameUI.removeButton.SetActive(false);
        }
        else
        {
            if (inRange && gameUI.inventoryUI.activeSelf)
            {
                gameUI.removeButton.SetActive(true);
            }
            else
            {
                gameUI.removeButton.SetActive(false);
            }
        }
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
