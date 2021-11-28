using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManeger : MonoBehaviour
{
    
    public static AudioClip 
        buildComplete,
        clickSound,
        electricTowerFire,
        enemyDead,
        enemyDamage,
        towerDestory,
        cannonFire,
        MachineFire,
        explosion;

    private static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        buildComplete = Resources.Load<AudioClip>("Audio/BuildCompleteSound");
        clickSound = Resources.Load<AudioClip>("Audio/ClickSound02");
        electricTowerFire = Resources.Load<AudioClip>("Audio/ElecticTowerFire01");
        enemyDead = Resources.Load<AudioClip>("Audio/EnemyDeadSound");
        enemyDamage = Resources.Load<AudioClip>("Audio/EnemyGetDamageSound02");
        towerDestory = Resources.Load<AudioClip>("Audio/TowerBrokenWithGuitarSound");
        cannonFire = Resources.Load<AudioClip>("Audio/TowerCannonFire");
        MachineFire = Resources.Load<AudioClip>("Audio/TowerMultiGunFire");
        explosion = Resources.Load<AudioClip>("Audio/UltimateExplorsion");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "BuildCompleteSound":
                audioSrc.PlayOneShot (buildComplete);
                break;
            case "ClickSound02":
                audioSrc.PlayOneShot(clickSound);
                break;
            case "ElecticTowerFire01":
                audioSrc.PlayOneShot(electricTowerFire);
                break;
            case "EnemyDeadSound":
                audioSrc.PlayOneShot(enemyDead);
                break;
            case "EnemyGetDamageSound02":
                audioSrc.PlayOneShot(enemyDamage);
                break;
            case "TowerBrokenWithGuitarSound":
                audioSrc.PlayOneShot(towerDestory);
                break;
            case "TowerCannonFire":
                audioSrc.PlayOneShot(cannonFire);
                break;
            case "TowerMultiGunFire":
                audioSrc.PlayOneShot(MachineFire);
                break;
            case "UltimateExplorsion":
                audioSrc.PlayOneShot(explosion);
                break;
        }
    }
}
