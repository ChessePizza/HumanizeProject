using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected void Update()
    {
        // ทำการ sort วัตถุในฉาก 2D โดยอัตโนมัติ
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
}
