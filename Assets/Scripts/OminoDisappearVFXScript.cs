using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OminoDisappearVFXScript : MonoBehaviour
{
    static Vector3 SCALE_TARGET = new Vector3(2, 2, 1);

    public GameObject prefabBlockExample;

    List<SpriteRenderer> renderers;
    Vector3 vScale;

    public void Init(OminoScript omino) {
        transform.position = omino.coorToScript.Values.Select(c => c.transform.position).Aggregate((a, b) => a + b) / omino.Size();
        renderers = new List<SpriteRenderer>();
        foreach (BlockScript blockScript in omino.coorToScript.Values) {
            GameObject block = Instantiate(prefabBlockExample, transform);
            block.transform.position = blockScript.transform.position;
            block.transform.rotation = blockScript.transform.rotation;
            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            sr.color = Color.white;
            renderers.Add(sr);
        }
    }

    void Update() {
        foreach (SpriteRenderer sr in renderers) {
            Color c = sr.color;
            c.a -= Time.deltaTime * 5f;
            if (c.a <= 0) {
                Destroy(gameObject);
                return;
            }
            sr.color = c;
        }
        transform.localScale = Vector3.SmoothDamp(transform.localScale, SCALE_TARGET, ref vScale, .2f);
    }
}
