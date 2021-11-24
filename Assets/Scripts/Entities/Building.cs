using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum TargetType { 
        single = 0,
        multiple = 1,
        all = 2,
        ultimate = 3
    }

    public string title;
    public string description;

    public int damage;
    public float speed;
    public int durability;
    public float cooldown;
    public int targetAmount;

    public BuildingType type;
    public TargetType targetType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
