using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectVFXScript : MonoBehaviour
{
    static float DURATION = .166f;
    static Vector3 TARGET_SCALE = new Vector3(1.2f, 1.2f, 1);

    public SpriteRenderer[] spriteRenderers;

    public void SetColor(int blockColor) {
        Color c = Util.HueshiftBlockColor(spriteRenderers[0].color, blockColor);
        foreach (SpriteRenderer sr in spriteRenderers) {
            sr.color = c;
        }
    }

    void Update() {
        transform.localScale = Util.Damp(transform.localScale, TARGET_SCALE, .001f, Time.deltaTime);
        float a = spriteRenderers[0].color.a;
        a -= Time.deltaTime / DURATION;
        if (a <= 0) {
            Destroy(gameObject);
            return;
        }
        Color c = spriteRenderers[0].color;
        c.a = a;
        foreach (SpriteRenderer sr in spriteRenderers) {
            sr.color = c;
        }
    }
}
