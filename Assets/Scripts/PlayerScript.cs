using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;
    static float SPEED = 30;
    static float BOOST_MULT = 1.33f;

    public Rigidbody2D rb2d;
    public GrabberScript grabberScript;

    public float boostSeconds;

    void Start() {
        instance = this;
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !Application.isEditor && Application.platform != RuntimePlatform.WebGLPlayer) {
            Application.Quit();
        }
        boostSeconds = Mathf.Max(0, boostSeconds - Time.deltaTime);
    }

    void FixedUpdate() {
        UpdateInput();
    }
    void UpdateInput() {
        float speed = SPEED;
        float boostMultiplier = Mathf.Lerp(1, BOOST_MULT, Mathf.Clamp01(Mathf.InverseLerp(0, 2, boostSeconds)));
        speed *= boostMultiplier;
        if (grabberScript.grabbedOmino != null) {
            float size = grabberScript.grabbedOmino.Size();
            float multiplier = 1 / (.2f * size + 1);
            speed *= Mathf.Lerp(multiplier, 1, .75f);
        }
        Vector3 desiredVelocity = GetMovementVector() * speed;
        if (GrabberScript.instance.grabbedOmino?.combineEnabled == true) {
            desiredVelocity *= .66f;
        }
        rb2d.velocity = Vector3.Lerp(rb2d.velocity, desiredVelocity, .25f);
    }
    Vector2 GetMovementVector() {
        if (GameHelper.instance.gameOver) {
            return Vector2.zero;
        }
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
