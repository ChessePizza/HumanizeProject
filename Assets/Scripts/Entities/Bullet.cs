﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public Transform target;
    public float missileRotateSpeed = 200.0f;

    private int durability;
    private Building owner;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            if (owner.bulletType == Building.BulletType.misslie)
            {
                Vector2 direction = (Vector2)target.position - rb.position;
                direction.Normalize();

                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb.angularVelocity = -rotateAmount * missileRotateSpeed;
            }
            rb.velocity = transform.up * owner.bulletSpeed;
        }
    }

	public void Update()
	{
        if (durability <= 0) 
        {
            Break();
        }
        missileRotateSpeed += 5;
        durability--;
	}

	void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Enemy")
        {
            if (owner.bulletType == Building.BulletType.misslie)
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

        Health targetHealth = target.GetComponent<Health>();
        targetHealth.SetHealth(targetHealth.currentHealth - owner.bulletDamage);
        Destroy(this.gameObject);
    }

    public void Setup(Building owner, Transform target)
    {
        this.owner = owner;
        this.durability = owner.bulletDurability;

        if(target) this.target = target;
    }
}