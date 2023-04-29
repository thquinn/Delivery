using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineScript : MonoBehaviour
{
    public Collider2D c2d;
    public BlockScript blockScript;

    private void OnTriggerStay2D(Collider2D collision) {
        blockScript.TryCombine(c2d, collision);
    }
}
