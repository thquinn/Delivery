using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabberScript : MonoBehaviour
{
    static float DISTANCE = 10;
    static float STRENGTH = 1000;
    static Vector3 SCALE_INNER = new Vector3(.85f, .85f, 1);
    static Vector3 SCALE_OUTER = new Vector3(1.2f, 1.2f, 1);

    public SpriteRenderer grabInnerRenderer, grabOuterRenderer;
    public Rigidbody2D rb2d;
    public FixedJoint2D fixedJoint;
    public LayerMask layerMaskBlock, layerMaskEviscerate;

    float lastAngle;
    bool lastGrabbing;
    public OminoScript grabbedOmino;
    BlockScript evisceratingBlock;
    Vector3 scaleVInner, scaleVOuter;
    float vAngle;
    public bool mouseDisabled;
    Vector3 lastMousePosition;

    void Start() {
        Cursor.visible = false;
    }

    void FixedUpdate() {
        float angle = GetAngle();
        if (grabbedOmino != null) {
            angle = Mathf.SmoothDampAngle(lastAngle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg, ref vAngle, .1f, STRENGTH / grabbedOmino.Size(), Time.fixedDeltaTime);
            angle *= Mathf.Deg2Rad;
        }
        Vector3 localPosition = new Vector3(DISTANCE * Mathf.Cos(angle), DISTANCE * Mathf.Sin(angle), -1);
        rb2d.MovePosition(transform.parent.position + localPosition);
        rb2d.rotation -= Mathf.DeltaAngle(angle * Mathf.Rad2Deg, lastAngle * Mathf.Rad2Deg);
        // Grabbing.
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
        grabInnerRenderer.transform.localScale = Vector3.SmoothDamp(grabInnerRenderer.transform.localScale, grabbing ? Vector3.one : SCALE_INNER, ref scaleVInner, .05f);
        grabOuterRenderer.transform.localScale = Vector3.SmoothDamp(grabOuterRenderer.transform.localScale, grabbing ? Vector3.one : SCALE_OUTER, ref scaleVOuter, .05f);
        // Eviscerating.
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
        float jx = Input.GetAxis("RHorizontal");
        float jy = Input.GetAxis("RVertical");
        Vector2 v = new Vector2(jx, jy);
        if (v != Vector2.zero) {
            mouseDisabled = true;
            v.Normalize();
            return Mathf.Atan2(v.y, v.x);
        }
        if (mouseDisabled) {
            if (Input.mousePosition != lastMousePosition) {
                mouseDisabled = false;
            } else {
                return lastAngle;
            }
            lastMousePosition = Input.mousePosition;
        }
        Vector3 mouseDelta = Util.GetMouseWorldPosition() - transform.parent.position;
        if (mouseDelta.sqrMagnitude > 5) {
            return Mathf.Atan2(mouseDelta.y, mouseDelta.x);
        }
        return lastAngle;
    }
    bool IsGrabbing() {
        return Input.GetMouseButton(0) || Input.GetAxis("RTrigger") > .5f;
    }
    bool IsCombineEnabled() {
        return IsGrabbing() && (Input.GetMouseButton(1) || Input.GetAxis("LTrigger") > .5f);
    }
    bool IsEviscerating() {
        return !IsGrabbing() && (Input.GetMouseButton(1) || Input.GetAxis("LTrigger") > .5f);
    }
}
