using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OminoScript : MonoBehaviour
{
    public static float INTERBLOCK_DISTANCE = 4.25f;
    public static Vector2Int[] NEIGHBOR_ORDER = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    public GameObject prefabBlock;

    public Rigidbody2D rb2d;

    public int ID;
    public bool combineEnabled;

    protected Dictionary<Collider2D, Vector2Int> colliderToCoor;
    public Dictionary<Vector2Int, BlockScript> coorToScript;
    protected Dictionary<Collider2D, (Vector2Int, Vector2Int)> combineColliderToCoorAndDirection;

    public void Init(IEnumerable<Vector2Int> coors) {
        colliderToCoor = new Dictionary<Collider2D, Vector2Int>();
        coorToScript = new Dictionary<Vector2Int, BlockScript>();
        combineColliderToCoorAndDirection = new Dictionary<Collider2D, (Vector2Int, Vector2Int)>();
        foreach (Vector2Int coor in coors) {
            PutBlockAtCoor(Instantiate(prefabBlock, transform).GetComponent<BlockScript>(), coor);
        }
        FinalizeOmino();
    }
    void PutBlockAtCoor(BlockScript blockScript, Vector2Int coor, Transform oldTransform = null) {
        Vector3 oldPosition = oldTransform == null ? Vector3.zero : oldTransform.position;
        Quaternion oldRotation = oldTransform == null ? Quaternion.identity : oldTransform.rotation;
        GameObject blockObject = blockScript.gameObject;
        blockObject.transform.localPosition = new Vector3(coor.x * INTERBLOCK_DISTANCE, coor.y * INTERBLOCK_DISTANCE);
        blockObject.transform.localRotation = Quaternion.identity;
        if (oldTransform != null) {
            blockScript.spritesTransform.position = oldPosition;
            blockScript.spritesTransform.rotation = Util.GetMinRotationMod90(oldRotation, transform.rotation.eulerAngles.z);
            blockScript.combineLerping = true;
        }
        colliderToCoor[blockObject.GetComponent<Collider2D>()] = coor;
        blockScript.omino = this;
        coorToScript[coor] = blockScript;
        for (int i = 0; i < NEIGHBOR_ORDER.Length; i++) {
            combineColliderToCoorAndDirection[blockScript.combineColliders[i]] = (coor, NEIGHBOR_ORDER[i]);
        }
    }
    void FinalizeOmino() {
        ID = Util.GetCanonicalPolyominoID(coorToScript.Keys);
        rb2d.mass = Size();
        foreach (var kvp in coorToScript) {
            for (int i = 0; i < NEIGHBOR_ORDER.Length; i++) {
                kvp.Value.combineColliders[i].gameObject.SetActive(!coorToScript.ContainsKey(kvp.Key + NEIGHBOR_ORDER[i]));
            }
        }
    }

    public void Combine(Collider2D thisCollider, Collider2D otherCollider) {
        HashSet<Vector2Int> originalCoors = new HashSet<Vector2Int>(coorToScript.Keys);
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
            PutBlockAtCoor(kvp.Value, newCoor, kvp.Value.transform);
            // Queue VFX for attaching.
            foreach (Vector2Int direction in NEIGHBOR_ORDER) {
                if (originalCoors.Contains(newCoor + direction)) {
                    kvp.Value.QueueConnectVFX(direction);
                }
            }
        }
        Destroy(otherOmino.gameObject);
        FinalizeOmino();
    }

    public void Eviscerate(Collider2D collider) {
        Vector2Int coor = colliderToCoor[collider];
        DestroyBlock(coor, true);
        if (colliderToCoor.Count == 0) {
            Destroy(gameObject);
            return;
        }
        // Create new ominos if split into 2+.
        HashSet<Vector2Int> coors = new HashSet<Vector2Int>(coorToScript.Keys);
        coors.ExceptWith(GetConnected(coors.First()));
        while (coors.Count > 0) {
            Vector2Int startCoor = coors.First();
            Vector2 startPosition = transform.position + transform.right * startCoor.x * INTERBLOCK_DISTANCE + transform.up * startCoor.y * INTERBLOCK_DISTANCE;
            HashSet<Vector2Int> connected = GetConnected(startCoor);
            var rootedCoors = connected.Select(c => c - startCoor);
            OminoScript newOmino = SpawnerScript.instance.SpawnEmpty();
            newOmino.transform.position = startPosition;
            newOmino.transform.rotation = transform.rotation;
            newOmino.Init(rootedCoors);
            foreach (Vector2Int c in connected) {
                DestroyBlock(c);
            }
            coors.ExceptWith(connected);
        }
        FinalizeOmino();
    }
    void DestroyBlock(Vector2Int coor, bool particle = false) {
        Collider2D collider = coorToScript[coor].c2d;
        colliderToCoor.Remove(collider);
        BlockScript blockScript = coorToScript[coor];
        coorToScript.Remove(coor);
        foreach (Collider2D c in blockScript.combineColliders) {
            combineColliderToCoorAndDirection.Remove(c);
        }
        if (particle) {
            ParticleHelper.instance.Eviscerate(blockScript.transform.position);
        }
        Destroy(blockScript.gameObject);
    }
    HashSet<Vector2Int> GetConnected(Vector2Int coor) {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(coor);
        HashSet<Vector2Int> seen = new HashSet<Vector2Int>();
        seen.Add(coor);
        while (queue.Count > 0) {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int direction in NEIGHBOR_ORDER) {
                Vector2Int neighbor = current + direction;
                if (!seen.Contains(neighbor) && coorToScript.ContainsKey(neighbor)) {
                    queue.Enqueue(neighbor);
                    seen.Add(neighbor);
                }
            }
        }
        return seen;
    }

    public int Size() {
        return colliderToCoor.Count;
    }
    public bool ContainsAll(Dictionary<Collider2D, OminoScript> colliders) {
        return colliderToCoor.Keys.All(c => colliders.ContainsKey(c));
    }
}
