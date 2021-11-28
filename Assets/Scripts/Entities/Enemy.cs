using Pathfinding;
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

    private AILerp aiLerp;
    private Gameplay gameplay;
    private float moveSpeed;
    private float animSpeed;

    Vector3 previous_position; // ใช้สำหรับดูทิศทางเพื่อหมุน
    bool facingRight = false;
    bool isGettingDamage = false;
    bool isAttacking = false;

    void Awake()
    {
        facingRight = false;
        isGettingDamage = false;
        isAttacking = false;

        anim = GetComponent<Animator>();
        animSpeed = anim.speed;
        health = GetComponent<Health>();
        aiLerp = GetComponent<AILerp>();
        moveSpeed = aiLerp.speed;
        gameplay = Camera.main.GetComponent<Gameplay>();

        previous_position = transform.position;

        anim.SetBool("canWalk", true);
        anim.SetBool("Attack", false);
        anim.SetBool("EnemyDead",false);
    }

    void Update () {
        if (gameplay.pause)
        {
            aiLerp.speed = 0;
            anim.speed = 0;
        }
        else
        {
            aiLerp.speed = moveSpeed;
            anim.speed = animSpeed;
            Vector3 current_position = transform.position;

            // Facing Right
            if ((previous_position.x < current_position.x && !facingRight) ||
                (previous_position.x > current_position.x && facingRight))
            {
                flip();
            }

            if (health.currentHealth <= 0) Break();
        }
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
        if (!gameplay.pause)
        {
            anim.speed = animSpeed;
            if (target)
            {
                Health health = target.GetComponent<Health>();
                //SoundManeger.PlaySound("EnemyGetDamage Sound 02");
                if (health.currentHealth > 0)
                {
                    Camera.main.GetComponent<GameUI>().showWarning();
                    health.SetHealth(health.currentHealth - AttackDamage);
                    if (!isAttacking)
                    {
                        StartCoroutine(DamageEffect(target));
                    }
                }
                else
                {
                    anim.SetBool("Attack", false);
                }
            }
        }
    }

    public void GetDamage()
    {
        if (!isGettingDamage)
        {            
            StartCoroutine(GetDamageEffect());
        }
    }

    IEnumerator GetDamageEffect()
    {
        isGettingDamage = true;
        SetSpritesColor(this.gameObject, Color.red);
        yield return new WaitForSeconds(.5f);
        SetSpritesColor(this.gameObject, Color.white);
        yield return new WaitForSeconds(.5f);
        SetSpritesColor(this.gameObject, Color.red);
        yield return new WaitForSeconds(.5f);
        SetSpritesColor(this.gameObject, Color.white);
        yield return new WaitForSeconds(.5f);
        isGettingDamage = false;
        yield return null;
    }
    IEnumerator DamageEffect(GameObject o)
    {
        isAttacking = true;
        SetSpritesColor(o, Color.red);
        yield return new WaitForSeconds(.5f);
        SetSpritesColor(o, Color.white);
        yield return new WaitForSeconds(.5f);
        SetSpritesColor(o, Color.red);
        yield return new WaitForSeconds(.5f);
        SetSpritesColor(o, Color.white);
        yield return new WaitForSeconds(.5f);
        isAttacking = false;
        yield return null;
    }

    public void SetSpritesColor(GameObject o, Color color)
    {
        SpriteRenderer sprite = o.GetComponent<SpriteRenderer>();
        if (sprite) sprite.color = color;

        SpriteRenderer[] sprites = o.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer s in sprites) s.color = color;
    }

    void flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
        transform.Find("Health").Rotate(0f, 180f, 0f);
    }

    void Break()
    {
        //SoundManeger.PlaySound("EnemyDeadSound");
        anim.SetBool("canWalk",false); //Animation Here
        anim.SetBool("EnemyDead",true);
        Camera.main.GetComponent<Gameplay>().updateKill();
        Destroy(this.gameObject);
    }
}
