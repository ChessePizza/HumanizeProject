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

        // ต้องไม่อยู่ในโหมด Edit Passability ถึงจะสามารถ Bake Grid ได้
        if (grid.gameObject.transform.Find("Editor") == null)
        {
            // แก้ไขช่องใน Grid ที่ไม่ต้องการให้สร้างป้อมหรือ spawn ทรัพยากรได้
            if (GUILayout.Button("Edit Passability"))
            {
                PassabilityEditor.swap = false;
                CreatePassabilityEditor(grid);
            }

            // ทำให้ Grid ทั้งหมดเป็น Passable
            if (GUILayout.Button("Bake Grid"))
            {
                BakeGrid(grid);
            }
        }
        else
        {
            if (GUILayout.Button("Apply Passability"))
            {
                DestroyPassabilityEditor(grid);
            }
        }
    }

    public void CreatePassabilityEditor(GridMap grid)
    {
        // Grid ต้องมีขนาดมากกว่า 1 และ ขนาดของ Data ต้องเท่ากับ Grid (ถ้าไม่เท่าต้องกด Bake ก่อน)
        if (grid.size.x > 1 && grid.size.y > 1 && (grid.size.x * grid.size.y) == grid.data.Length)
        {
            Sprite sprite = grid.gameObject.GetComponent<SpriteRenderer>().sprite;
            Rect rect = sprite.rect;

            GameObject editor = new GameObject("Editor");
            editor.transform.SetParent(grid.gameObject.transform);

            Canvas canvas = editor.AddComponent<Canvas>();

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = canvasRect.anchoredPosition = canvasRect.anchorMin = new Vector2(0, 0);
            canvasRect.anchorMax = new Vector2(1, 1);

            CanvasScaler scalar = editor.AddComponent<CanvasScaler>();
            scalar.dynamicPixelsPerUnit = 100; // เพื่อให้สเกลของฟอนต์เข้ากับสเกลของฉากแผนที่

            for (int x = 0; x < grid.size.x; x++)
                for (int y = 0; y < grid.size.y; y++)
                {
                    GameObject cell = new GameObject("Passability_" + x + "_" + y);
                    cell.transform.SetParent(editor.transform);

                    Passability text = cell.AddComponent<Passability>();
                    text.fontSize = 1;
                    text.fontStyle = FontStyle.Bold;
                    text.alignment = TextAnchor.MiddleCenter;

                    if (grid.data[(x * grid.size.y) + y] == 0)
                    {
                        text.text = "O";
                        text.passable = true;
                    }
                    else
                    {
                        text.text = "X";
                        text.passable = false;
                    }

                    RectTransform textRect = text.GetComponent<RectTransform>();
                    textRect.sizeDelta = new Vector2(1.5f, 1.5f);
                    textRect.anchorMax = textRect.anchorMin = new Vector2(0, 1);

                    Vector2 cellSize = new Vector2(rect.width / grid.ratio / sprite.pixelsPerUnit / grid.size.x, rect.height / grid.ratio / sprite.pixelsPerUnit / grid.size.y);
                    textRect.anchoredPosition = new Vector2(1 + (cellSize.x * x), -1 - (cellSize.y * y));
                    textRect.pivot = new Vector2(0.5f, 0.5f);
                }
        }
    }

    public void DestroyPassabilityEditor(GridMap grid)
    {
        GameObject editor = grid.gameObject.transform.Find("Editor").gameObject;
        Object.DestroyImmediate(editor);
    }

    public void ResetPassability(GridMap grid)
    {
        if (grid.data != null)
            for (int i = 0; i < grid.data.Length; i++) {
                grid.data[i] = 0;
            }
    }

    public void BakeGrid(GridMap grid)
    {
        Sprite sprite = grid.level.GetComponentInParent<SpriteRenderer>().sprite;
        Rect rect = sprite.rect;
        // คำนวนหาขนาดของ Texture ของ Grid
        Vector2 size = new Vector2(rect.width * grid.ratio, rect.height * grid.ratio);

        // สร้าง Texture2D
        Texture2D image = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);
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
            ResetPassability(grid);
                                  
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
                    image.SetPixel(px, (int)size.y - i, grid.color);
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
                    image.SetPixel((int)size.x - i, py, grid.color);
                }
            }
        }
        image.Apply();

        // ใส่ Sprite ใน Sprite Renderer
        Sprite spriteTexture = Sprite.Create(image, new Rect(0, 0, (int)size.x, (int)size.y), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
        spriteTexture.name = "GridMap";
        grid.gameObject.GetComponent<SpriteRenderer>().sprite = spriteTexture;

        EditorUtility.SetDirty(grid);

        Debug.Log("Grid Successfully Baked");
    }
}
