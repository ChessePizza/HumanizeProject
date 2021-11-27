using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Game Data
public enum ItemType
{
    pipe = 0,
    keylock = 1,
    screw = 2,
    sheet = 3,
    battery = 4,
    bomb = 5,
    paper = 6
}

public enum BuildingType
{
    fastgun = 0,
    stronggun = 1,
    machinegun = 2,
    barbwire = 3,
    superweapon = 4
}

public enum EnemyType
{
    zombie = 0
}

public class GameData : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource bgm;
    public AudioSource sfx;

    bool backFromLevel = false;

    public int level = 0;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}