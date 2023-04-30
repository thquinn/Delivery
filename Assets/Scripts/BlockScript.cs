using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    static float EVISCERATION_TIME = 1;

    public GameObject prefabConnectVFX;

    public Transform spritesTransform;
    public Collider2D c2d;
    public Collider2D[] combineColliders;
    public SpriteRenderer[] combineGlows;
    public SpriteRenderer eviscerationOverlay;

    public OminoScript omino;

    public bool combineLerping;
    Vector3 lerpV, lerpAV;
    public bool beingEviscerated;
    float eviscerationTime;
    List<Vector2Int> connectVFX;

    void Update() {
        // Combine lerping.
        if (combineLerping) {
            spritesTransform.localPosition = Vector3.SmoothDamp(spritesTransform.localPosition, Vector3.zero, ref lerpV, .05f);
            spritesTransform.localRotation = Util.SmoothDampQuaternion(spritesTransform.localRotation, Quaternion.identity, ref lerpAV, .05f);
            if (spritesTransform.localPosition.sqrMagnitude < .1f) {
                spritesTransform.localPosition = Vector3.zero;
                spritesTransform.localRotation = Quaternion.identity;
                combineLerping = false;
                if (connectVFX != null) {
                    foreach (Vector2Int direction in connectVFX) {
                        GameObject connectVFX = Instantiate(prefabConnectVFX);
                        Vector3 position = transform.position;
                        position.z = connectVFX.transform.position.z;
                        Vector3 direction3 = new Vector3(direction.x, direction.y, 0) * OminoScript.INTERBLOCK_DISTANCE / 2;
                        position += transform.TransformVector(direction3);
                        connectVFX.transform.position = position;
                        connectVFX.transform.rotation = transform.rotation;
                        if (direction == Vector2Int.left || direction == Vector2Int.right) {
                            connectVFX.transform.Rotate(0, 0, 90);
                        }
                    }
                    connectVFX.Clear();
                }
            }
        }
        // Evisceration.
        eviscerationTime = beingEviscerated ? (eviscerationTime + Time.deltaTime) : 0;
        eviscerationOverlay.enabled = beingEviscerated;
        if (eviscerationTime > 0) {
            Color c = eviscerationOverlay.color;
            c.a = eviscerationTime / EVISCERATION_TIME;
            eviscerationOverlay.color = c;
        }
        if (eviscerationTime >= EVISCERATION_TIME) {
            omino.Eviscerate(c2d);
        }
        // Combine glows.
        for (int i = 0; i < combineGlows.Length; i++) {
            if (!omino.combineEnabled || !combineColliders[i].gameObject.activeInHierarchy) {
                combineGlows[i].gameObject.SetActive(false);
            } else {
                combineGlows[i].gameObject.SetActive(true);
                Color c = combineGlows[i].color;
                c.a = Mathf.Sin(Time.time * 4) * .5f + .5f;
                combineGlows[i].color = c;
            }
        }
    }

    public void TryCombine(Collider2D thisCollider, Collider2D otherCollider) {
        if (omino.combineEnabled) {
            omino.Combine(thisCollider, otherCollider);
        }
    }

    public void QueueConnectVFX(Vector2Int direction) {
        if (connectVFX == null) {
            connectVFX = new List<Vector2Int>();
        }
        connectVFX.Add(direction);
    }
}
