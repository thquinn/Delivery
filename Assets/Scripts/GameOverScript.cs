using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject prefabFadeOutScript;

    public CanvasGroup canvasGroup;

    float vAlpha;

    void Start() {
        canvasGroup.alpha = 0;
    }

    void Update() {
        if (GameHelper.instance.gameOver && (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Start"))) {
            Instantiate(prefabFadeOutScript, transform.parent);
            Destroy(this);
        }

        float targetAlpha = GameHelper.instance.gameOver ? 1 : 0;
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, targetAlpha, ref vAlpha, .1f);
    }
}
