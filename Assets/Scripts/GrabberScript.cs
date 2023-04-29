using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabberScript : MonoBehaviour
{
    static float DISTANCE = 10;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb2d;
    public FixedJoint2D fixedJoint;
    public LayerMask layerMaskBlock;

    float lastAngle;
    bool lastGrabbing;
    OminoScript grabbedOmino;

    void FixedUpdate() {
        float angle = GetAngle();
        if (!float.IsNaN(angle)) {
            Vector3 localPosition = new Vector3(DISTANCE * Mathf.Cos(angle), DISTANCE * Mathf.Sin(angle), -1);
            rb2d.position = transform.parent.position + localPosition;
            if (!float.IsNaN(lastAngle)) {
                rb2d.rotation -= Mathf.DeltaAngle(angle * Mathf.Rad2Deg, lastAngle * Mathf.Rad2Deg);
            }
        }
        bool grabbing = IsGrabbing();
        if (!lastGrabbing && grabbing) {
            Rigidbody2D closestRB = null;
            float closestDistance = float.MaxValue;
            foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 1.1f, layerMaskBlock)) {
                float distance = Vector2.Distance(c.transform.position, transform.position);
                if (distance < closestDistance) {
                    closestRB = c.attachedRigidbody;
                    closestDistance = distance;
                }
            }
            fixedJoint.enabled = closestRB != null;
            fixedJoint.connectedBody = closestRB;
            grabbedOmino = closestRB?.GetComponent<OminoScript>();
        } else if (!grabbing) {
            fixedJoint.enabled = false;
            fixedJoint.connectedBody = null;
            if (grabbedOmino != null) {
                grabbedOmino.combineEnabled = false;
                grabbedOmino = null;
            }
        }
        if (grabbedOmino != null) {
            grabbedOmino.combineEnabled = IsCombineEnabled();
        }
        lastAngle = angle;
        lastGrabbing = grabbing;
    }
    float GetAngle() {
        Vector3 mouseDelta = Util.GetMouseWorldPosition() - transform.parent.position;
        if (mouseDelta.sqrMagnitude > 5) {
            return Mathf.Atan2(mouseDelta.y, mouseDelta.x);
        }
        return float.NaN;
    }
    bool IsGrabbing() {
        return Input.GetMouseButton(0);
    }
    bool IsCombineEnabled() {
        return Input.GetMouseButton(1);
    }
}
