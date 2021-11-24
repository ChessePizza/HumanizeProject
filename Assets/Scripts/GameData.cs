using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Game Data
public enum Item
{
    pipe = 0,
    keylock = 1,
    screw = 2,
    sheet = 3,
    special = 4
}

public enum Building
{
    fastgun = 0,
    stronggun = 1,
    machinegun = 2,
    barbwire = 3,
    superweapon = 4
}

public enum Monster
{
    zombie = 0
}

public class GameData : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource bgm;
    public AudioSource sfx;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}