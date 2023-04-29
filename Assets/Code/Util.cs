using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
        static Camera cam;

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
}
