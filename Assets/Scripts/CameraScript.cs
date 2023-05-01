using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject target;

    Camera cam;
    float originalSize, zoomOutSize;
    Vector3 velocity;
    float vSize;

    void Start() {
        cam = Camera.main;
        originalSize = cam.orthographicSize;
        zoomOutSize = originalSize * 2.5f;
    }

    void Update() {
        Vector3 targetPosition = target.transform.position;
        targetPosition.z = transform.localPosition.z;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, .1f);
        bool zoomOut = Input.GetButton("Zoom");
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoomOut ? zoomOutSize : originalSize, ref vSize, .1f);
    }
}
