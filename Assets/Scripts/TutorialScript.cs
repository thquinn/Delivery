using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    float vAlpha;

    private void Start() {
        canvasGroup.alpha = 0;
    }

    private void Update() {
        float targetAlpha = GameHelper.instance.paused ? 1 : 0;
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, targetAlpha, ref vAlpha, .1f);
        if (canvasGroup.alpha < .01f) {
            Destroy(gameObject);
        }
    }
}
