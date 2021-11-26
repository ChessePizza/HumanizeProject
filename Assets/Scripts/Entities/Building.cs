using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BulletType
    {
        normal = 0,
        misslie = 1,
        surround = 2,
        ultimate = 3
    }

    public string title;
    public string description;

    public int cost;
    public int durability;
    public int cooldown;

    public Transform spawner;
    public Bullet bullet;
    public float bulletSpeed;
    public int bulletDurability;
    public int bulletDamage;
    public BulletType bulletType;
    public int bulletCount = 0;

    public BuildingType type;

    public Vector2 positionAdjust;
    public Vector2Int girdPosition;

    public bool isReady = false;

    public GridMap grid;
    List<GameObject> targets;
    Health health;
    int timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isReady)
        {
            if (health.currentHealth <= 0) Break();

            if (bulletCount < 0) bulletCount = 0;
            timer = Mathf.Clamp(timer, 0, cooldown);
            Debug.Log("Target:" + targets.Count);
            if (targets.Count > 0 && timer == 0)
            {
                GameObject entity = Instantiate(bullet.gameObject, spawner.position, Quaternion.identity);
                Bullet entityBullet = entity.GetComponent<Bullet>();
                entity.transform.SetParent(this.transform);
                bulletCount++;
                timer = cooldown;

                entityBullet.Setup(this, targets[0].transform);
            }
            else if(timer > 0)
            {
                timer--;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Enemy" && isReady)
        {
            targets.Add(trig.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Enemy" && isReady)
        {
            int targetIndex = FindTarget(trig.gameObject.name);
            if (targetIndex >= 0)
            {
                targets.RemoveAt(targetIndex);
            }
        }
    }

    int FindTarget(string name)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].name == name)
            {
                return i;
            }
        }
        return -1;
    }

    void Break()
    {
        grid.data[(this.girdPosition.x * grid.size.y) + this.girdPosition.y] = 0;
        Camera.main.GetComponent<GameUI>().UpdatePassability();
        Destroy(this.gameObject);
    }
}
