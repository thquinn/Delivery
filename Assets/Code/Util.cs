using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
        static Camera cam;

        public static float Damp(float source, float target, float smoothing, float dt) {
            return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
        }
        public static Vector2 Damp(Vector2 source, Vector2 target, float smoothing, float dt) {
            return Vector2.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
        }
        public static Vector3 Damp(Vector3 source, Vector3 target, float smoothing, float dt) {
            return Vector3.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
        }
        public static Quaternion Damp(Quaternion source, Quaternion target, float smoothing, float dt) {
            return Quaternion.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
        }

        public static Vector2 GetRandomPointWithinRadius(float radius) {
            float rSq = radius * radius;
            while (true) {
                Vector2 v = new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
                if (v.sqrMagnitude <= rSq) {
                    return v;
                }
            }
        }

        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime) {
            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        static Dictionary<(Color, int), Color> MEMO_HUESHIFT = new Dictionary<(Color, int), Color>();
        public static Color HueshiftBlockColor(Color c, int blockColor) {
            if (blockColor == 0) {
                return c;
            }
            if (MEMO_HUESHIFT.ContainsKey((c, blockColor))) {
                return MEMO_HUESHIFT[(c, blockColor)];
            }
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            switch (blockColor) {
                case 1:
                    h = .136111f;
                    break;
                default:
                    throw new System.Exception("Unexpected block color to hueshift.");
            }
            Color shifted = Color.HSVToRGB(h, s, v);
            shifted.a = c.a;
            MEMO_HUESHIFT[(c, blockColor)] = shifted;
            return shifted;
        }
        static Dictionary<Color, Color> ORBITAL_COLOR_MEMO = new Dictionary<Color, Color>();
        public static Color OrbitalColor(Color c) {
            if (ORBITAL_COLOR_MEMO.ContainsKey(c)) {
                return ORBITAL_COLOR_MEMO[c];
            }
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            s = 1;
            v = 1;
            Color output = Color.HSVToRGB(h, s, v);
            output.a = .5f;
            ORBITAL_COLOR_MEMO[c] = output;
            return output;
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

        public static Vector2Int GetCoorsDimensions(IEnumerable<Vector2Int> coors) {
            int minX = coors.Min(c => c.x);
            int maxX = coors.Max(c => c.x);
            int minY = coors.Min(c => c.y);
            int maxY = coors.Max(c => c.y);
            return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
        }

        static Dictionary<int, int> POLYOMINO_ID_TO_CANONICAL_ID = new Dictionary<int, int>();
        static HashBuilder hb = new HashBuilder();
        public static int GetCanonicalPolyominoID(IEnumerable<Vector3Int> origCoors) {
            int minX = origCoors.Min(c => c.x);
            int minY = origCoors.Min(c => c.y);
            List<Vector3Int> coors = new List<Vector3Int>(origCoors.Select(c => new Vector3Int(c.x - minX, c.y - minY, c.z)));
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
        static int GetPolyominoID(List<Vector3Int> coors) {
            hb.Clear();
            foreach (Vector3Int coor in coors.OrderBy(c => c.y).ThenBy(c => c.x)) {
                hb.Add(987354);
                hb.Add(coor.x);
                hb.Add(916785);
                hb.Add(coor.y);
                hb.Add(2347869);
                hb.Add(coor.z);
            }
            return hb.GetHashCode();
        }
        static List<Vector3Int> RotatePolyomino(List<Vector3Int> coors, int maxY) {
            return coors.Select(c => new Vector3Int(maxY - c.y, c.x, c.z)).ToList();
        }
        static List<Vector3Int> FlipPolyomino(List<Vector3Int> coors, int maxX) {
            return coors.Select(c => new Vector3Int(maxX - c.x, c.y, c.z)).ToList();
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
        public static bool IsPointOnCamera(Vector2 position, float radius) {
            if (cam == null) cam = Camera.main;
            float diameter = radius * 2;
            Bounds bounds = new Bounds(position, new Vector3(diameter, diameter, 1));
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
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

    public static class ArrayExtensions {
        public static T[] Shuffle<T>(this T[] array) {
            int n = array.Length;
            for (int i = 0; i < n; i++) {
                int r = i + UnityEngine.Random.Range(0, n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
            return array;
        }
    }
}
