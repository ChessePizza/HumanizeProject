using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Player player;
    public SpriteRenderer map;
    public Slider zoom;

    void Start()
    {
        
    }

    void Update()
    {
        if (zoom)
        {
            // ให้ 4 เป็นค่า zoom เริ่มต้น
            Camera.main.orthographicSize = 4 + zoom.value;
        }

        if (player) 
        {
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        }

        if (map)
        {
            //Debug.Log("miX:" + map.sprite.rect.xMin);
            //Debug.Log("maX:" + map.sprite.rect.xMax);
            //Debug.Log("miY:" + map.sprite.rect.yMin);
            //Debug.Log("maY:" + map.sprite.rect.yMax);
            //float x = Mathf.Clamp(transform.position.x, map.sprite.rect.xMin / map.sprite.pixelsPerUnit, map.sprite.rect.width / map.sprite.pixelsPerUnit);
        }
    }
}
