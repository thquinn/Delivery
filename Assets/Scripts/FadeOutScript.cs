using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Update() {
        canvasGroup.alpha += Time.deltaTime * 3;
        if (canvasGroup.alpha >= 1) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
