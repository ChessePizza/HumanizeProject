using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    Gameplay gameplay;
    public int slot = -1;
    public ItemType type;
    private void Start()
    {
        gameplay = Camera.main.GetComponent<Gameplay>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && slot < 0)
        {
            int slot = gameplay.getEmptySlot();
            if (slot >= 0)
            {
                //ของเข้ากระเป๋า
                this.slot = slot;
                gameplay.addItem(this, slot);
            }
        }
    }
}
