using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
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
    }
}
