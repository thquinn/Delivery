using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    public GrabberScript grabberScript;
    public Canvas canvas;
    public RectTransform rt;
    public CanvasGroup canvasGroup;
    public Image imageBlur;

    float originalAlpha, vAlpha;

    void Start() {
        originalAlpha = canvasGroup.alpha;
    }

    void Update() {
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, grabberScript.mouseDisabled ? 0 : originalAlpha, ref vAlpha, .1f);
        rt.anchoredPosition = Input.mousePosition / canvas.scaleFactor;
        float blurScale = Mathf.Lerp(.66f, 1, Mathf.Sin(Time.time * 5) * .5f + .5f);
        imageBlur.transform.localScale = new Vector3(blurScale, blurScale, 1);
    }
}
