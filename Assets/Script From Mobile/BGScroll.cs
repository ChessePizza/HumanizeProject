using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{
    // Start is called before the first frame update

    public float scroll_Speed = 0.1f;
    private MeshRenderer mesh_Renderer;
    //private AudioSource BGSound;

    private float x_Scroll;

    private void Awake()
    {
        mesh_Renderer = GetComponent<MeshRenderer>();
        //BGSound = GetComponent<AudioSource>();

    }


    // Update is called once per frame
    void Update()
    {
        Scroll();
    }

    void Scroll()
    {
        x_Scroll = Time.time * scroll_Speed;
        Vector2 offset = new Vector2(x_Scroll, 0f);
        mesh_Renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
