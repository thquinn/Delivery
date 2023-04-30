using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    static float SPEED = 20;

    public Rigidbody2D rb2d;
    public GrabberScript grabberScript;

    void FixedUpdate() {
        UpdateInput();
    }
    void UpdateInput() {
        float speed = SPEED;
        if (grabberScript.grabbedOmino != null) {
            float size = grabberScript.grabbedOmino.Size();
            float multiplier = 1 / (.2f * size + 1);
            speed *= Mathf.Lerp(multiplier, 1, .75f);
        }
        Vector3 desiredVelocity = GetMovementVector() * speed;
        rb2d.velocity = Vector3.Lerp(rb2d.velocity, desiredVelocity, .25f);
    }
    Vector2 GetMovementVector() {
        float jx = Input.GetAxis("Horizontal");
        float jy = Input.GetAxis("Vertical");
        Vector2 v = new Vector2(jx, jy);
        if (v != Vector2.zero) {
            if (v.sqrMagnitude > 1) {
                v.Normalize();
            }
            return v;
        }
        float x = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);
        float y = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
        v = new Vector2(x, y);
        if (v.sqrMagnitude > 1) {
            v.Normalize();
        }
        return v;
    }
}
