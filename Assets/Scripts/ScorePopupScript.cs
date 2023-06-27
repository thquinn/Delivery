using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopupScript : MonoBehaviour
{
    static float FADE_TIME = .25f;
    static float LIFETIME = 1;

    public TextMeshPro tmp;

    float time = 0;
    Vector3 target;
    Vector3 velocity;

    public void Init(OminoScript omino) {
        Vector3 localPosition = transform.localPosition;
        localPosition.z = -5;
        transform.localPosition = localPosition;
        string text = "+" + (omino.Size() * 100).ToString("N0");
        int multiplier = omino.GetMultiplier();
        if (multiplier > 1) {
            text += "<color=#ffd000><voffset=.17em><size=66%> x </size></voffset>" + multiplier;
        }
        tmp.text = text;
        Color c = tmp.color;
        c.a = 0;
        tmp.color = c;
        target = transform.localPosition + new Vector3(0, 10, 0);
    }

    void Update() {
        time += Time.deltaTime;
        if (time > LIFETIME) {
            Destroy(gameObject);
            return;
        }
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, target, ref velocity, LIFETIME);
        Color c = tmp.color;
        if (time < FADE_TIME) {
            c.a = time / FADE_TIME;
        } else if (time > LIFETIME - FADE_TIME) {
            c.a = 1 - (time - (LIFETIME - FADE_TIME)) / FADE_TIME;
        } else {
            c.a = 1;
        }
        tmp.color = c;
    }
}
