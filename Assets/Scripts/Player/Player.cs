﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    Vector2 inputValue;
    bool facingRight = true;

    void Start()
    {
        
    }

    void Update()
    {        
        inputValue = GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();
        Animator anim = GetComponent<Animator>();

        if (inputValue.x != 0 || inputValue.y != 0)
        {
            anim.SetBool("iswalk", true);
        }
        else
        {
            anim.SetBool("iswalk", false);
        }

        if ((inputValue.x < 0 && facingRight) || (inputValue.x > 0 && !facingRight))
        {
            flip();
        }
    }
    void FixedUpdate()
    {
        // Move Player
        GetComponent<Rigidbody2D>().velocity = new Vector2(inputValue.x * moveSpeed, inputValue.y * moveSpeed);
    }

    void flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }


}
