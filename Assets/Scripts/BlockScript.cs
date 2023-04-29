using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public Collider2D[] combineColliders;
    public SpriteRenderer[] combineGlows;

    public OminoScript omino;

    void Update() {
        for (int i = 0; i < combineGlows.Length; i++) {
            if (!omino.combineEnabled || !combineColliders[i].gameObject.activeInHierarchy) {
                combineGlows[i].gameObject.SetActive(false);
            } else {
                combineGlows[i].gameObject.SetActive(true);
                Color c = combineGlows[i].color;
                c.a = Mathf.Sin(Time.time * 4) * .5f + .5f;
                combineGlows[i].color = c;
            }
        }
    }

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
