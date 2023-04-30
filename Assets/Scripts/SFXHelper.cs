using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXHelper : MonoBehaviour
{
    public static SFXHelper instance;

    public AudioSource sfxEviscerate, sfxSnap, sfxZoneClear;

    void Start() {
        instance = this;
    }

    public void Eviscerate() {
        sfxEviscerate.PlayOneShot(sfxEviscerate.clip);
    }
    public void Snap() {
        sfxSnap.PlayOneShot(sfxSnap.clip);
    }
    public void ZoneClear() {
        sfxZoneClear.PlayOneShot(sfxZoneClear.clip);
    }
}
