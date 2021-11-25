using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Text text;
    public Gradient gradient;
    //public Image fill;
    public int MaxHealth = 5000;
    public int currentHealth;

    public void Start()
    {
        SetMaxHealth(MaxHealth);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        this.MaxHealth = health;
        this.currentHealth = health;
        text.text = $"Health:{health}/{MaxHealth}";

        //fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        this.currentHealth = health;
        text.text = $"Health:{health}/{MaxHealth}";
        //fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    

    
}
