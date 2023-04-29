using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    static float SPEED = 20;

    public Rigidbody2D rb2d;

    void Start() {
        
    }

    void FixedUpdate() {
        UpdateInput();
    }
    void UpdateInput() {
        rb2d.velocity = GetMovementVector() * SPEED;
    }
    Vector2 GetMovementVector() {
        float x = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);
        float y = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
        Vector2 v = new Vector2(x, y);
        if (v.sqrMagnitude > 1) {
            v.Normalize();
        }
        return v;
    }
}
