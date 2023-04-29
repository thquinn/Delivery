using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
        static Camera cam;

        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime) {
            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static int GetRotationsBetweenDirections(Vector2Int d1, Vector2Int d2) {
            if (d1 == d2) return 0;
            if (d1.x == d2.x || d1.y == d2.y) return 2;
            if ((d1 == Vector2Int.right && d2 == Vector2Int.up) ||
                (d1 == Vector2Int.up && d2 == Vector2Int.left) ||
                (d1 == Vector2Int.left && d2 == Vector2Int.down) ||
                (d1 == Vector2Int.down && d2 == Vector2Int.right)) {
                return 1;
            }
            return 3;
        }

        public static Quaternion GetMinRotationMod90(Quaternion q, float targetAngle) {
            Vector3 euler = q.eulerAngles;
            for (int i = 0; i < 4; i++) {
                float delta = Mathf.Abs(Mathf.DeltaAngle(euler.z, targetAngle));
                if (delta < 45) {
                    return Quaternion.Euler(euler);
                }
                euler.z += 90;
            }
            return q;
        }

        static Dictionary<int, int> POLYOMINO_ID_TO_CANONICAL_ID = new Dictionary<int, int>();
        static HashBuilder hb = new HashBuilder();
        public static int GetCanonicalPolyominoID(IEnumerable<Vector2Int> origCoors) {
            int minX = origCoors.Min(c => c.x);
            int minY = origCoors.Min(c => c.y);
            List<Vector2Int> coors = new List<Vector2Int>(origCoors.Select(c => new Vector2Int(c.x - minX, c.y - minY)));
            int firstID = GetPolyominoID(coors);
            if (POLYOMINO_ID_TO_CANONICAL_ID.ContainsKey(firstID)) {
                return POLYOMINO_ID_TO_CANONICAL_ID[firstID];
            }
            int maxX = coors.Max(c => c.x);
            int maxY = coors.Max(c => c.y);
            List<int> ids = new List<int>();
            ids.Add(firstID);
            for (int i = 0; i < 3; i++) {
                coors = RotatePolyomino(coors, maxY);
                int swap = maxX;
                maxX = maxY;
                maxY = swap;
                ids.Add(GetPolyominoID(coors));
            }
            coors = FlipPolyomino(coors, maxX);
            ids.Add(GetPolyominoID(coors));
            for (int i = 0; i < 3; i++) {
                coors = RotatePolyomino(coors, maxY);
                int swap = maxX;
                maxX = maxY;
                maxY = swap;
                ids.Add(GetPolyominoID(coors));
            }
            int canonical = ids.Min();
            foreach (int id in ids) {
                POLYOMINO_ID_TO_CANONICAL_ID[id] = canonical;
            }
            return canonical;
        }
        static int GetPolyominoID(List<Vector2Int> coors) {
            hb.Clear();
            foreach (Vector2Int coor in coors.OrderBy(c => c.y).ThenBy(c => c.x)) {
                hb.Add(coor.x);
                hb.Add(coor.y);
            }
            return hb.GetHashCode();
        }
        static List<Vector2Int> RotatePolyomino(List<Vector2Int> coors, int maxY) {
            return coors.Select(c => new Vector2Int(maxY - c.y, c.x)).ToList();
        }
        static List<Vector2Int> FlipPolyomino(List<Vector2Int> coors, int maxX) {
            return coors.Select(c => new Vector2Int(maxX - c.x, c.y)).ToList();
        }

        public static Rigidbody2D GetClosest(Vector2 position, float radius, LayerMask layerMask) {
            Rigidbody2D closestRB = null;
            float closestDistance = float.MaxValue;
            foreach (Collider2D c in Physics2D.OverlapCircleAll(position, radius, layerMask)) {
                float distance = Vector2.Distance(c.transform.position, position);
                if (distance < closestDistance) {
                    closestRB = c.attachedRigidbody;
                    closestDistance = distance;
                }
            }
            return closestRB;
        }

        public static Vector3 GetMouseWorldPosition() {
            if (cam == null) cam = Camera.main;
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            return worldPos;
        }
    }

    // from weston on StackOverflow
    public sealed class HashBuilder {
        private int hash = 17;

        public HashBuilder Add(int value) {
            unchecked {
                hash = hash * 31 + value;
            }
            return this;
        }

        public HashBuilder Add(object value) {
            return Add(value != null ? value.GetHashCode() : 0);
        }

        public HashBuilder Add(float value) {
            return Add(value.GetHashCode());
        }

        public HashBuilder Add(double value) {
            return Add(value.GetHashCode());
        }

        public void Clear() {
            hash = 0;
        }

        public override int GetHashCode() {
            return hash;
        }
    }
}
