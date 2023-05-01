using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    static WindScript instance;

    public AudioSource wind;
    public float targetVolume, fadeSpeed;

    private void Start() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        wind.volume = Mathf.Min(targetVolume, wind.volume + Time.deltaTime * fadeSpeed);
    }
}
