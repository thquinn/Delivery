using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OminoScript : MonoBehaviour
{
    static float INTERBLOCK_DISTANCE = 4.25f;
    public static Vector2Int[] NEIGHBOR_ORDER = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    public GameObject prefabBlock;

    public bool combineEnabled;

    protected Dictionary<Collider2D, Vector2Int> colliderToCoor;
    protected Dictionary<Vector2Int, BlockScript> coorToScript;
    protected Dictionary<Collider2D, (Vector2Int, Vector2Int)> combineColliderToCoorAndDirection;

    void Start() {
        colliderToCoor = new Dictionary<Collider2D, Vector2Int>();
        coorToScript = new Dictionary<Vector2Int, BlockScript>();
        combineColliderToCoorAndDirection = new Dictionary<Collider2D, (Vector2Int, Vector2Int)>();
        Init(new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) });
    }
    public void Init(IEnumerable<Vector2Int> coors) {
        foreach (Vector2Int coor in coors) {
            PutBlockAtCoor(Instantiate(prefabBlock, transform).GetComponent<BlockScript>(), coor);
        }
        SetCombineTriggers();
    }
    void PutBlockAtCoor(BlockScript blockScript, Vector2Int coor) {
        GameObject blockObject = blockScript.gameObject;
        blockObject.transform.localPosition = new Vector3(coor.x * INTERBLOCK_DISTANCE, coor.y * INTERBLOCK_DISTANCE);
        blockObject.transform.localRotation = Quaternion.identity;
        colliderToCoor[blockObject.GetComponent<Collider2D>()] = coor;
        blockScript.omino = this;
        coorToScript[coor] = blockScript;
        for (int i = 0; i < NEIGHBOR_ORDER.Length; i++) {
            combineColliderToCoorAndDirection[blockScript.combineColliders[i]] = (coor, NEIGHBOR_ORDER[i]);
        }
    }
    void SetCombineTriggers() {
        foreach (var kvp in coorToScript) {
            for (int i = 0; i < NEIGHBOR_ORDER.Length; i++) {
                kvp.Value.combineColliders[i].gameObject.SetActive(!coorToScript.ContainsKey(kvp.Key + NEIGHBOR_ORDER[i]));
            }
        }
    }

    public void Combine(Collider2D thisCollider, Collider2D otherCollider) {
        OminoScript otherOmino = otherCollider.transform.parent.parent.GetComponent<OminoScript>();
        if (otherOmino == this) {
            return;
        }
        var (coorOne, directionOne) = combineColliderToCoorAndDirection[thisCollider];
        var (coorTwo, directionTwo) = otherOmino.combineColliderToCoorAndDirection[otherCollider];
        Vector2Int combineOrigin = coorOne + directionOne;
        int rotations = (Util.GetRotationsBetweenDirections(directionOne, directionTwo) + 2) % 4;
        foreach (var kvp in otherOmino.coorToScript) {
            Vector2Int coorOffset = kvp.Key - coorTwo;
            if (rotations == 1) {
                coorOffset = new Vector2Int(coorOffset.y, -coorOffset.x);
            } else if (rotations == 2) {
                coorOffset = new Vector2Int(-coorOffset.x, -coorOffset.y);
            } else if (rotations == 3) {
                coorOffset = new Vector2Int(-coorOffset.y, coorOffset.x);
            }
            Vector2Int newCoor = combineOrigin + coorOffset;
            kvp.Value.transform.parent = transform;
            PutBlockAtCoor(kvp.Value, newCoor);
        }
        Destroy(otherOmino.gameObject);
        SetCombineTriggers();
    }

    void Update() {
        
    }
}
