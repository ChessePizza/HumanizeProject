using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int AttackDamage;

    private GameObject target;
    private Animator anim;
    private Health health;

    Vector3 previous_position; // ใช้สำหรับดูทิศทางเพื่อหมุน
    bool facingRight = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        previous_position = transform.position;
        anim.SetBool("canWalk", true);
        anim.SetBool("Attack", false);
    }

    void Update () {

        Vector3 current_position = transform.position;

        // Facing Right
        if ((previous_position.x < current_position.x && !facingRight) ||
            (previous_position.x > current_position.x && facingRight))
        {
            flip();
        }

        if (health.currentHealth <= 0) Break();
    }
    
    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Base")
        {
            Attack(trig.gameObject);
        }
    }

    void Attack(GameObject target)
    {
        this.target = target;

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }
    
    public void Damage()
    {
        if (target)
        {
            Health health = target.GetComponent<Health>();
            if (health.currentHealth > 0)
            {
                health.SetHealth(health.currentHealth - AttackDamage);
                Camera.main.GetComponent<Gameplay>().showWarning();
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
        transform.Find("Health").Rotate(0f, 180f, 0f);
    }

    void Break()
    {
        Camera.main.GetComponent<Gameplay>().updateKill();
        Destroy(this.gameObject);
    }
}
