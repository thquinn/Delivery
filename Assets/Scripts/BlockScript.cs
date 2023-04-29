using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    static float EVISCERATION_TIME = 1;

    public Collider2D c2d;
    public Collider2D[] combineColliders;
    public SpriteRenderer[] combineGlows;
    public SpriteRenderer eviscerationOverlay;

    public OminoScript omino;

    public bool beingEviscerated;
    float eviscerationTime;

    void Update() {
        // Evisceration.
        eviscerationTime = beingEviscerated ? (eviscerationTime + Time.deltaTime) : 0;
        eviscerationOverlay.enabled = beingEviscerated;
        if (eviscerationTime > 0) {
            Color c = eviscerationOverlay.color;
            c.a = eviscerationTime / EVISCERATION_TIME;
            eviscerationOverlay.color = c;
        }
        if (eviscerationTime >= EVISCERATION_TIME) {
            omino.Eviscerate(c2d);
        }
        // Combine glows.
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
}
