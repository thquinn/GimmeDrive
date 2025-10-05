using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
        public static Vector2Int INVALID_COOR = new Vector2Int(-999, -999);
        public static float STRAIGHT_SIZE = 0.4f;
        public static Quaternion GetRotationFromDirection(Vector2Int direction) {
            if (direction.x == 1) return Quaternion.identity;
            if (direction.y == 1) return Quaternion.Euler(0, 90, 0);
            if (direction.x == -1) return Quaternion.Euler(0, 180, 0);
            return Quaternion.Euler(0, 270, 0);
        }
        public static Vector2Int Turn(Vector2Int direction, int turn) {
            if (turn == -1) {
                if (direction.x == 1) return new(0, -1);
                else if (direction.y == -1) return new(-1, 0);
                else if (direction.x == -1) return new(0, 1);
                return new(1, 0);
            }
            if (direction.x == 1) return new(0, 1);
            else if (direction.y == -1) return new(1, 0);
            else if (direction.x == -1) return new(0, -1);
            return new(-1, 0);
        }
        public static void SetStraightTransform(Transform transform, Vector2Int from, Vector2Int to, float t) {
            Vector2 lerpedCoor = Vector2.Lerp(from, to, t);
            transform.localPosition = new Vector3(lerpedCoor.x, 0, -lerpedCoor.y);
            transform.localRotation = GetRotationFromDirection(to - from);
        }
        public static float SetTurningTransform(Transform transform, Vector2Int last, Vector2Int from, Vector2Int to, Vector2Int next, float t, bool flipRot = false) {
            bool turnBefore = from - last != to - from;
            bool turnAfter = to - from != next - to;
            if (next == to) {
                next = to + (to - from);
            }
            // Mirror.
            if (turnBefore) {
                // TODO: Broken with turns before AND after.
                return SetTurningTransform(transform, next, to, from, last, 1 - t, true);
            }
            // Straight portion of the turn.
            if (t < STRAIGHT_SIZE) {
                SetStraightTransform(transform, from, to, t);
                if (flipRot) transform.Rotate(0, 180, 0);
                return 1;
            }
            // Position.
            Vector2 lerpedCoor = Vector2.Lerp(from, to, t);
            t = (t - STRAIGHT_SIZE) / (1 - STRAIGHT_SIZE);
            float curveRadius = 1 - STRAIGHT_SIZE;
            float radians = 45 * Mathf.Deg2Rad * t;
            // TODO: Horizontal left turns and vertical right turns are messed up.
            float xOffset = curveRadius * (1 - Mathf.Cos(radians)) * (next.x - to.x);
            lerpedCoor.x += xOffset;
            lerpedCoor.y -= xOffset;
            float yOffset = curveRadius * (1 - Mathf.Cos(radians)) * (next.y - to.y);
            lerpedCoor.y += yOffset;
            lerpedCoor.x -= yOffset;
            transform.localPosition = new Vector3(lerpedCoor.x, 0, -lerpedCoor.y);
            // Rotation.
            Quaternion startRotation = GetRotationFromDirection(to - from);
            Quaternion endRotation = GetRotationFromDirection(next - to);
            endRotation = Quaternion.Lerp(startRotation, endRotation, 0.5f);
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            if (flipRot) transform.Rotate(0, 180, 0);
            return 1.0f; // TODO: Slow this down.
        }
    }
}
