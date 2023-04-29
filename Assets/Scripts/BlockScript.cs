using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public Collider2D[] combineColliders;

    public OminoScript omino;

    public void TryCombine(Collider2D thisCollider, Collider2D otherCollider) {
        if (omino.combineEnabled) {
            omino.Combine(thisCollider, otherCollider);
        }
    }

    static int debug;
    private void Start() {
        GetComponentInChildren<TextMeshPro>().text = (++debug).ToString();
    }
}
