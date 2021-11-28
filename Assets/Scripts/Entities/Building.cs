using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BulletType
{
    normal = 0,
    misslie = 1,
    surround = 2,
    ultimate = 3
}

public class Building : MonoBehaviour
{

    public string title;
    public string description;
    public BuildingType type;
    [Header("Attack Settings")]
    public int cost;
    public int durability;
    public int cooldown;

    public Transform spawner;
    [Header("Bullet Settings")]
    public Bullet bullet;
    public float bulletSpeed;
    public int bulletDurability;
    public int bulletDamage;
    public BulletType bulletType;
    public int bulletCount = 0;

    private Animator anim;
    private float animSpeed;

    private Gameplay gameplay;

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
        health.SetMaxHealth(durability);
        anim = GetComponent<Animator>();
        animSpeed = anim.speed;

        gameplay = Camera.main.GetComponent<Gameplay>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameplay.pause)
        {
            anim.speed = 0;
        }
        else
        {
            anim.speed = animSpeed;
            if (isReady)
            {
                if (health.currentHealth <= 0) Break();

                if (bulletCount < 0) bulletCount = 0;
                timer = Mathf.Clamp(timer, 0, cooldown);

                if (targets.Count > 0 && timer == 0)
                {
                    anim.SetBool("BuilldingIsAttack", true);
                    //anim.SetBool("BuilldingIsAttack",false);//Animation Code Here
                    if (bulletType == BulletType.misslie || bulletType == BulletType.normal)
                    {
                        //for (int i = 0; i < targets.Count; i++)
                        if (targets.Count > 0)
                        {
                            GameObject entity = Instantiate(bullet.gameObject, spawner.position, Quaternion.identity);
                            Bullet entityBullet = entity.GetComponent<Bullet>();
                            entity.transform.SetParent(this.transform);
                            bulletCount++;
                            timer = cooldown;

                            entityBullet.Setup(this, targets[0].transform);
                            SoundManeger.PlaySound("TowerMultiGunFire");

                        }
                    }
                    else
                    {
                        float angle = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            GameObject entity = Instantiate(bullet.gameObject, spawner.position, Quaternion.identity);
                            Bullet entityBullet = entity.GetComponent<Bullet>();
                            entity.transform.SetParent(this.transform);
                            bulletCount++;
                            timer = cooldown;

                            entityBullet.Setup(this, null);
                            SoundManeger.PlaySound("TowerMultiGunFire");
                            entityBullet.SetAngle(angle);
                            angle += 45;
                        }
                    }
                }
                else if (timer > 0)
                {
                    anim.SetBool("BuilldingIsAttack", false);
                    timer--;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Enemy" && isReady)
        {
            int targetIndex = FindTarget(trig.gameObject.name);
            if (targetIndex < 0)
            {
                targets.Add(trig.gameObject);
            }
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
        //Animation Tower Destoyed and Sound Here
        SoundManeger.PlaySound("TowerBrokenWithGuitarSound");
        grid.data[(this.girdPosition.x * grid.size.y) + this.girdPosition.y] = 0;
        Camera.main.GetComponent<GameUI>().UpdatePassability();
        Destroy(this.gameObject);
    }
}
