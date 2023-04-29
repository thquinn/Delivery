using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    static Vector3 OFFSET = new Vector3(-.5f, -.5f, 0);
    static float Z = 2;

    void Update() {
        Vector3 position = transform.parent.position + OFFSET;
        position.z = Z;
        transform.position = position;
    }
}
