using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    public static ParticleHelper instance;

    public ParticleSystem eviscerate;

    void Start() {
        instance = this;
    }

    public void Eviscerate(Vector2 position) {
        eviscerate.transform.localPosition = position;
        eviscerate.Play();
    }
}
