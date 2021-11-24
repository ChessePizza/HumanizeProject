using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Player player;
    public SpriteRenderer map;
    public Slider zoom;

    Vector2 minValue;
    Vector2 maxValue;
    public Vector2 magicNumber; // Default: 2.0f,1.6f

    void Start()
    {
        if (map)
        {
            Vector2 pivot = map.gameObject.GetComponent<RectTransform>().pivot;
            // หาค่าในหน่วย Unity unit
            Vector2 rectMin = new Vector2(map.sprite.rect.xMin / map.sprite.pixelsPerUnit, map.sprite.rect.yMin / map.sprite.pixelsPerUnit);
            Vector2 rectMax = new Vector2(map.sprite.rect.xMax / map.sprite.pixelsPerUnit, map.sprite.rect.yMax / map.sprite.pixelsPerUnit);

            // หาค่าที่จะใช้ clamp
            minValue = new Vector2(rectMin.x - (rectMax.x * pivot.x), rectMin.y - (rectMax.y * pivot.y));
            maxValue = new Vector2(rectMax.x - (rectMax.x * (1.0f - pivot.x)), rectMax.y - (rectMax.y * (1.0f - pivot.y)));
        }

        if (player)
        {
            // หาค่า clamp ให้ player ไปในตัว โดย player ไม่จำเป็นต้องเดินในช่องนอกสุด
            GridMap grid = map.transform.Find("Grid").GetComponent<GridMap>();
            Vector2 size = new Vector2(map.sprite.rect.xMax / grid.size.x / map.sprite.pixelsPerUnit / 2.0f, map.sprite.rect.yMax / grid.size.y / map.sprite.pixelsPerUnit / 2.0f);
            player.SetClampValues(minValue + size, maxValue - size);
        }
    }

    void Update()
    {
        if (zoom)
        {
            // orthographic size จะมีค่าเริ่มต้นเป็น 4.0f
            Camera.main.orthographicSize = 4.0f + zoom.value;

            if (map)
            {
                // ตัวเลขนี้ยังไม่สามารถอธิบายได้ว่ามาจากอะไร แต่จากการทดลอง ค่า orthographic size จะมีค่าเริ่มต้นเป็น 4.0f
                // ตัวเกมอนุญาติให้ zoom ออกได้ถึง 8.0f จริงเป็นที่มาของส่วนแรก (8.0f / 4.0f) แต่ในส่วนของค่าด้านหลังเกิดจากการทดลองปรับค่า clamp value ไปเรื่อย
                // สมการเบื้องต้น (8.0f / 4.0f) * 2.0f = 4.0f กับ (8.0f / 4.0f) * 1.6f = 3.2f
                // แปลงเป็น (16.0f / 4.0f) กับ (12.8f / 4.0f)
                Vector2 clampRatio = new Vector2((8.0f * magicNumber.x) / Camera.main.orthographicSize, (8.0f * magicNumber.y) / Camera.main.orthographicSize);

                Vector2 _minValue = minValue - (minValue / clampRatio);
                Vector2 _maxValue = maxValue - (maxValue / clampRatio);

                if (player)
                {
                    Camera.main.transform.position = new Vector3(
                        Mathf.Clamp(player.transform.position.x, _minValue.x, _maxValue.x),
                        Mathf.Clamp(player.transform.position.y, _minValue.y, _maxValue.y),
                        Camera.main.transform.position.z);
                }
            }
        }
    }
}
