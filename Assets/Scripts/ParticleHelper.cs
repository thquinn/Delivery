using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    public static ParticleHelper instance;

    public ParticleSystem eviscerate;

    Color eviscerateMinColor, eviscerateMaxColor;

    void Start() {
        instance = this;
        eviscerateMinColor = eviscerate.colorOverLifetime.color.gradient.colorKeys[0].color;
        eviscerateMaxColor = eviscerate.colorOverLifetime.color.gradient.colorKeys[1].color;
    }

    public void Eviscerate(Vector2 position, int blockColor) {
        var gradient = eviscerate.colorOverLifetime.color.gradient;
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Util.HueshiftBlockColor(eviscerateMinColor, blockColor), gradient.colorKeys[0].time),
                                                  new GradientColorKey(Util.HueshiftBlockColor(eviscerateMaxColor, blockColor), gradient.colorKeys[1].time),
                         }, gradient.alphaKeys);
        var gColor = eviscerate.colorOverLifetime.color;
        gColor.gradient = gradient;
        var gCOL = eviscerate.colorOverLifetime;
        gCOL.color = gColor;
        eviscerate.transform.localPosition = position;
        eviscerate.Play();
    }
}
