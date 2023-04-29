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
    public LayerMask layerMaskBlock, layerMaskEviscerate;

    float lastAngle;
    bool lastGrabbing;
    OminoScript grabbedOmino;
    BlockScript evisceratingBlock;

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
            Rigidbody2D closestRB = Util.GetClosest(transform.position, 1.1f, layerMaskBlock);
            fixedJoint.enabled = closestRB != null;
            fixedJoint.connectedBody = closestRB;
            grabbedOmino = closestRB?.GetComponent<OminoScript>();
        } else if (!grabbing || grabbedOmino == null) {
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
        if (IsEviscerating()) {
            if (evisceratingBlock != null) {
                evisceratingBlock.beingEviscerated = false;
            }
            Rigidbody2D closestRB = Util.GetClosest(transform.position, .1f, layerMaskEviscerate);
            evisceratingBlock = closestRB?.transform.parent.GetComponent<BlockScript>();
            if (evisceratingBlock != null) {
                evisceratingBlock.beingEviscerated = true;
            }
        } else if (evisceratingBlock != null) {
            evisceratingBlock.beingEviscerated = false;
            evisceratingBlock = null;
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
        return IsGrabbing() && Input.GetMouseButton(1);
    }
    bool IsEviscerating() {
        return !IsGrabbing() && Input.GetMouseButton(1);
    }
}
