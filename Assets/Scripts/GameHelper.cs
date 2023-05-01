using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper : MonoBehaviour
{
    public static GameHelper instance;

    static float RADIUS_INITIAL = 100;
    static float RADIUS_GROWTH_RATE = 4;

    public bool paused;
    public float timePassed;
    public Vector2 timer;
    public float arenaRadius;
    public int score;
    public bool gameOver;

    void Start() {
        instance = this;
        paused = true;
        timer = new Vector2(60, 60);
    }

    void Update() {
        if (paused && PlayerScript.instance.transform.position.magnitude > 30) {
            paused = false;
        }
        if (!paused && !gameOver) {
            timePassed += Time.deltaTime;
            timer.x = Mathf.Max(0, timer.x - Time.deltaTime);
            if (timer.x <= 0) {
                gameOver = true;
            }
        }
        arenaRadius = RADIUS_INITIAL + RADIUS_GROWTH_RATE * Mathf.Sqrt(timePassed);
        Shader.SetGlobalFloat("_ArenaRadius", arenaRadius);
    }

    public void Deliver(OminoScript omino) {
        paused = false;
        int size = omino.Size();
        float seconds = size * 5;
        float timeMultiplier = 66 / (66 + timePassed * .1f);
        float missingTime = 1 - timer.x / timer.y;
        timeMultiplier = Mathf.Lerp(timeMultiplier, 1, missingTime);
        seconds *= timeMultiplier;
        timer.x = Mathf.Min(timer.y, timer.x + seconds);
        score += size * 100 * omino.GetMultiplier();
    }
}
