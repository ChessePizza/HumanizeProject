using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance; //Minimum distance for attack
    public float moveSpeed;
    public float timer; //Timer for cooldown between attacks
    public int AttackDamage;

    private RaycastHit2D hit;
    private GameObject target; //
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange; //
    private bool cooling;
    private bool IsAttack = true;
    private float intTimer;

    void Awake()
    {
        intTimer = timer; //Store the inital value of timer
        anim = GetComponent<Animator>();
		Debug.Log("[DEBUG]Timer awake");
    }

    void Update () {
        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        //When Player is detected
        if(hit.collider != null)
        {
            EnemyLogic();
        }
        else if(hit.collider == null)
        {
            inRange = false;
        }

        if(inRange == false)
        {
            anim.SetBool("canWalk", false);
            StopAttack();
        }
    }
    
    void OnTriggerEnter2D(Collider2D trig)
    {
        Debug.Log("[DEBUG]Enter Trigger");
        if (trig.gameObject.tag == "Objective")
        {
            Debug.Log("[DEBUG]Trigger Player");
            target = trig.gameObject;
            inRange = true;
        }
    }
    
    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);
		Debug.Log("[DEBUG]Enemy Logic Start");
        if(distance > attackDistance)
        {
			Debug.Log("[DEBUG]Distance higher than Attack Distance");
            Move();
            StopAttack();
        }
        else if(attackDistance >= distance && cooling == false)
        {
			Debug.Log("[DEBUG]Attack Distance higher than distance and cool down is false");
            Attack();
        }

        if (cooling)
        {
			Debug.Log("[DEBUG]cool down");
            Cooldown();
            anim.SetBool("Attack",false);
        }
        
    }

    void Move()
    {
        anim.SetBool("canWalk" ,true);
        Debug.Log("[DEBUG]Move Method");
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
			Debug.Log("[DEBUG]Long Attack");
            Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
        }
    }

    void Attack()
    {
        timer = intTimer; //Reset Timer when Player enter Attack Range
        attackMode = true; //To check if Enemy can still attack or not

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
        //HealthBarScript targetHealthBarScript = target.GetComponent<HealthBarScript>();
        //targetHealthBarScript.SetHealth(targetHealthBarScript.Health - 50);
        
        Debug.Log("[DEBUG]ATTACKING");
    }
    
    void Cooldown()
    {
        timer -= Time.deltaTime;
        if (IsAttack)
        {
            HealthBarScript targetHealthBarScript = target.GetComponent<HealthBarScript>();
            targetHealthBarScript.SetHealth(targetHealthBarScript.currentHealth - AttackDamage);
            //StructerHealth structerHealth = target.GetComponent<StructerHealth>();
            //structerHealth.SetHealth(structerHealth.currentHealth - AttackDamage);
            IsAttack = false;
        }

        if(timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
            //HealthBarScript targetHealthBarScript = target.GetComponent<HealthBarScript>();
            //targetHealthBarScript.SetHealth(targetHealthBarScript.currentHealth - 50);
            IsAttack = true;
        }
    }
    
    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
		Debug.Log("[DEBUG]STOP ATTACK");
    }
    
    

    void RaycastDebugger()
    {
        if (distance > attackDistance)
        {
			Debug.Log("[DEBUG]Distance higher than AttackDistance - RayCast");
            Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
			Debug.Log("[DEBUG]Distance lower than AttackDistance - RayCast");
            Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.green);
        }
    }
    
    public void TriggerCooling()
    {
        cooling = true;
    }

    public void Damage()
    {
        Debug.Log("Damage");
    }
}
