using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    float lastScore, vScore;

    void Update() {
        lastScore = Mathf.SmoothDamp(lastScore, GameHelper.instance.score, ref vScore, .1f);
        tmp.text = $"<voffset=.45em><size=50%>score</size></voffset> " + Mathf.RoundToInt(lastScore).ToString("N0");
    }
}
