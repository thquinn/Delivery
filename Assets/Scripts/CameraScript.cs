using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject target;

    Vector3 velocity;

    void Start() {
        
    }

    void Update() {
        Vector3 targetPosition = target.transform.position;
        targetPosition.z = transform.localPosition.z;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, .1f);
    }
}
