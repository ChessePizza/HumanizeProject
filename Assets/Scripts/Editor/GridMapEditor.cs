using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(GridMap))]
public class GridMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridMap grid = (GridMap)target;

        // แก้ไขช่องใน Grid ที่ไม่ต้องการให้สร้างป้อมหรือ spawn ทรัพยากรได้
        if (grid.gameObject.transform.Find("Editor") == null)
        {
            if (GUILayout.Button("Edit Passability"))
            {
                CreatePassabilityEditor(grid);
            }
        }
        else
        {
            if (GUILayout.Button("Apply Passability"))
            {
                DestroyPassabilityEditor(grid);
            }
        }

        // Bake Grid ลงไปใน Texture เพื่อใช้ใน runtime
        // โค้ดส่วนนี้ทำงานเฉพาะใน Editor เท่านั้น ไม่มีผลกับเกมใน runtiem
        if (GUILayout.Button("Bake"))
        {
            BakeGrid(grid);
        }
    }

    public void CreatePassabilityEditor(GridMap grid)
    {
        if (grid.size.x > 1 && grid.size.y > 1 && (grid.size.x * grid.size.y) == grid.data.Length)
        {
            GameObject editor = new GameObject("Editor");
            editor.transform.SetParent(grid.gameObject.transform);

            Canvas canvas = editor.AddComponent<Canvas>();

            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.sizeDelta = rect.anchoredPosition = rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);

           CanvasScaler scalar = editor.AddComponent<CanvasScaler>();
            scalar.dynamicPixelsPerUnit = 100; // เพื่อให้สเกลของฟอนต์เข้ากับสเกลของฉากแผนที่

            for (int x = 0; x < grid.size.x; x++)
                for (int y = 0; y < grid.size.y; y++)
                {
                    GameObject tile = new GameObject("Passability_" + x + "_" + y);
                    tile.transform.SetParent(editor.transform);

                    Text text = tile.AddComponent<Text>();
                    text.fontSize = 1;
                    text.fontStyle = FontStyle.Bold;
                    text.alignment = TextAnchor.MiddleCenter;

                    if (grid.data[(x * grid.size.y) + y] > 0)
                    {
                        text.text = "O";
                    }
                    else
                    {
                        text.text = "X";
                    }

                    RectTransform text_rect = text.GetComponent<RectTransform>();
                    text_rect.sizeDelta = new Vector2(1.5f, 1.5f);
                    rect.anchorMax = rect.anchorMin = new Vector2(0, 1);
                    text_rect.anchoredPosition = new Vector2(1 * 2.2f * x, -1 * 2.2f * y);
                    text_rect.pivot = new Vector2(0.5f, 0.5f);
                }
        }
    }

    public void DestroyPassabilityEditor(GridMap grid)
    {
        GameObject editor = grid.gameObject.transform.Find("Editor").gameObject;
        Object.DestroyImmediate(editor);
    }


    public void BakeGrid(GridMap grid)
    {
        Rect rect = grid.level.GetComponentInParent<SpriteRenderer>().sprite.rect;
        // คำนวนหาขนาดของ Texture ของ Grid
        Vector2Int size = new Vector2Int((int)(rect.width * grid.ratio), (int)(rect.height * grid.ratio));

        // สร้าง Texture2D
        Texture2D image = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
        image.name = "GridImage";

        // ทำให้รูปเป็นโปร่งใสทั้งหมด
        for (int px = 0; px < size.x; px++)
        {
            for (int py = 0; py < size.y; py++)
            {
                image.SetPixel(px, py, Color.clear);
            }
        }

        // Grid ต้องมีขนาดมากกว่า 1 เช่น กริดแบบ 2x2
        if (grid.size.x > 1 && grid.size.y > 1)
        {
            grid.data = new byte[grid.size.x * grid.size.y];

            Vector2 gridPixelSize = new Vector2(size.x / grid.size.x, size.y / grid.size.y);
            //วาดเส้นแนวตั้ง
            for (int x = 0; x < grid.size.x; x++)
            {
                for (int py = 0; py < size.y; py++)
                {
                    for (int i = 0; i < grid.borderSize; i++)
                    {
                        image.SetPixel((int)(x * gridPixelSize.x) + i, py, grid.color);
                    }
                }
            }

            //วาดเส้นแนวนอน
            for (int y = 0; y < grid.size.y; y++)
            {
                for (int px = 0; px < size.x; px++)
                {
                    for (int i = 0; i < grid.borderSize; i++)
                    {
                        image.SetPixel(px, (int)(y * gridPixelSize.y) + i, grid.color);
                    }
                }
            }

            //เก็บขอบแนวนอน
            for (int px = 0; px < size.x; px++)
            {
                for (int i = 0; i < grid.borderSize * 2; i++)
                {
                    image.SetPixel(px, i, grid.color);
                }
                for (int i = 0; i < grid.borderSize * 2; i++)
                {
                    image.SetPixel(px, size.y - i, grid.color);
                }
            }

            //เก็บขอบแนวตั้ง
            for (int py = 0; py < size.y; py++)
            {
                for (int i = 0; i < grid.borderSize * 2; i++)
                {
                    image.SetPixel(i, py, grid.color);
                }
                for (int i = 0; i < grid.borderSize * 2; i++)
                {
                    image.SetPixel(size.x - i, py, grid.color);
                }
            }
        }
        image.Apply();

        // ใส่ Sprite ใน Sprite Renderer
        Sprite sprite = Sprite.Create(image, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
        sprite.name = "GridMap";
        grid.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

        EditorUtility.SetDirty(grid);

        Debug.Log("Grid Successfully Baked");
    }
}
