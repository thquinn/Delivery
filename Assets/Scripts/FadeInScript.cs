using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Start() {
        if (Application.isEditor) {
            Destroy(gameObject);
            return;
        }
        canvasGroup.alpha = 1;
    }

    void Update() {
        canvasGroup.alpha -= Time.deltaTime * 3;
        if (canvasGroup.alpha <= 0) {
            Destroy(gameObject);
        }
    }
}
