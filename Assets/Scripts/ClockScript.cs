using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockScript : MonoBehaviour
{
    public Image circle;
    public RectTransform rtText;
    public TextMeshProUGUI tmp;

    float lastPercent;
    float percentSpeed;
    float textDistance;
    Color colorWhite, colorRed;

    void Start() {
        lastPercent = 1;
        textDistance = rtText.anchoredPosition.y;
        colorWhite = circle.color;
        colorRed = new Color(1, 0, 0, colorWhite.a);
    }

    void Update() {
        Vector2 time = GameHelper.instance.timer;
        float percent = time.x / time.y;
        lastPercent = Mathf.SmoothDamp(lastPercent, percent, ref percentSpeed, .1f);
        circle.material.SetFloat("_Revealed", lastPercent);
        float angle = Mathf.PI / 2 - 2 * Mathf.PI * lastPercent;
        rtText.anchoredPosition = new Vector2(textDistance * Mathf.Cos(angle), textDistance * Mathf.Sin(angle));
        tmp.text = Mathf.CeilToInt(time.x) + "s";
        float colorT = Mathf.Clamp01(Mathf.InverseLerp(12, 8, time.x));
        circle.color = Color.Lerp(colorWhite, colorRed, colorT);
    }
}
