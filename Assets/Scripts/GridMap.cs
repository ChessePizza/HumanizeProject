using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridMap : MonoBehaviour
{
    public GameObject level;
    public Color color = Color.black;
    public int borderSize = 1;
    public Vector2Int size;
    [Tooltip("เนื่องจาก Texture ของฉากแผนที่อาจมีขนาดใหญ่มาก เช่น 4K เราสามารถลดค่านี้เพื่อให้ Texture ของ Grid มีขนาดเล็กกว่าได้")]
    public float ratio = 1.0f;
    public byte[] data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

