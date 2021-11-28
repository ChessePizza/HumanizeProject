using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public Transform target;
    public float missileRotateSpeed = 200.0f;

    private Animator anim;
    private float animSpeed;

    private Gameplay gameplay;

    private int durability;
    private Building owner;

    float angle;
    bool kia = false;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        animSpeed = anim.speed;

        gameplay = Camera.main.GetComponent<Gameplay>();

        kia = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameplay.pause)
        {
            if (owner.bulletType == BulletType.misslie && target)
            {
                Vector2 direction = (Vector2)target.position - rb.position;
                direction.Normalize();

                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb.angularVelocity = -rotateAmount * missileRotateSpeed;
            }
            if (owner.bulletType == BulletType.normal || owner.bulletType == BulletType.surround)
            {
                rb.MoveRotation(angle);
            }
            rb.velocity = transform.up * owner.bulletSpeed;
        }
    }

	public void Update()
	{
        if (gameplay.pause)
        {
            anim.speed = 0;
        }
        else
        {
            anim.speed = animSpeed;
            if (durability <= 0)
            {
                Break();
            }
            missileRotateSpeed += 5;
            durability--;
        }
	}

	void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Enemy")
        {
            if (owner.bulletType == BulletType.misslie)
            {
                if (target && trig.gameObject.name == target.name) Break();
            }
            else
            {
                target = trig.transform;
                Break();
            }
        }
    }

    void Break()
    {
        Health ownerHealth = owner.GetComponent<Health>();
        ownerHealth.SetHealth(ownerHealth.currentHealth - owner.cost);

        owner.bulletCount--;

        if (target && !kia)
        {
            kia = true;
            anim.SetBool("BulletDestory", true); //Play Animation Break here
            StartCoroutine(DestroyWithEnemies(target));
        }
        else if(!kia)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator DestroyWithEnemies(Transform target)
    {
        yield return new WaitForSeconds(.5f);
        SoundManeger.PlaySound("EnemyGetDamageSound02");
        target.GetComponent<Enemy>().GetDamage();
        Health targetHealth = target.GetComponent<Health>();
        targetHealth.SetHealth(targetHealth.currentHealth - owner.bulletDamage);
        Destroy(this.gameObject);
        yield return null;
    }

    public void Setup(Building owner, Transform target)
    {
        rb = GetComponent<Rigidbody2D>();
        this.owner = owner;
        this.durability = owner.bulletDurability;

        if (target)
        {
            this.target = target;

            if (owner || owner.bulletType == BulletType.normal)
            {
                Vector2 originPos = transform.position;
                Vector2 targetPos = target.transform.position;

                Vector2 direction = targetPos - originPos;

                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
            }
        }
    }

    public void SetAngle(float angle)
    {
        this.angle = angle;
    }
}
