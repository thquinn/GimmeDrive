using Assets.Code;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class CarScript : MonoBehaviour {
    public PuzzleScript puzzleScript;
    public GameObject visuals;
    public Transform pickupAnchor;

    public float speed;

    CarState state;
    Vector2Int direction;
    Vector2Int last, from, to, next;
    float t;
    List<Vector2Int> pickupCoors;
    PuzzleSpace activePickup;
    bool usedActivePickup;

    void Start() {
        pickupCoors = new List<Vector2Int>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (state != CarState.Waiting) Stop();
            else Go();
        }
        if (puzzleScript.entryCoors.Count == 0) {
            visuals.SetActive(false);
        } else if (state == CarState.Going) {
            Going();
        } else if (state == CarState.Waiting) {
            visuals.SetActive(true);
            Vector2Int coor = puzzleScript.entryCoors[0];
            transform.localPosition = new Vector3(coor.x, 0, -coor.y);
            if (coor.x == -1) transform.localRotation = Quaternion.identity;
            else if (coor.y == -1) transform.localRotation = Quaternion.Euler(0, 90, 0);
            else if (coor.x == puzzleScript.puzzle.width) transform.localRotation = Quaternion.Euler(0, 180, 0);
            else if (coor.y == puzzleScript.puzzle.height) transform.localRotation = Quaternion.Euler(0, 270, 0);
            else throw new System.Exception("Entry coor not on edge.");
        }
    }

    public void Go() {
        if (state != CarState.Waiting) {
            Stop();
        }
        if (puzzleScript.entryCoors.Count == 0) return;
        state = CarState.Going;
        from = Util.INVALID_COOR;
        to = puzzleScript.entryCoors[0];
        next = Util.INVALID_COOR;
        if (to.x == -1) direction = new(1, 0);
        else if (to.y == -1) direction = new(0, 1);
        else if (to.x == puzzleScript.puzzle.width) direction = new(-1, 0);
        else if (to.y == puzzleScript.puzzle.height) direction = new(0, -1);
        else throw new System.Exception("Entry coor not on edge.");
        ArriveAtCoor();
    }
    public void Stop() {
        state = CarState.Waiting;
        t = 0;
        activePickup = PuzzleSpace.Empty;
        pickupCoors.Clear();
    }

    void Going() {
        visuals.SetActive(true);
        while (t >= 1) {
            ArriveAtCoor();
            t--;
        }
        // Set transform.
        bool turning = Util.IsTurn(last, from, to) || Util.IsTurn(from, to, next);
        if (to == from) turning = false;
        if (turning) {
            float tMult = Util.SetTurningTransform(transform, last, from, to, next, t);
            t += Time.deltaTime * speed * tMult;
        } else {
            Util.SetStraightTransform(transform, from, to, t);
            t += Time.deltaTime * speed;
        }
    }
    void ArriveAtCoor() {
        if (from == to) {
            state = CarState.Crashed;
            return;
        }
        if (from == Util.INVALID_COOR) {
            last = to - direction;
        } else {
            last = from;
        }
        PuzzleSpace spacePickup = GetSpaceWithPath(to);
        if (spacePickup != PuzzleSpace.Empty) {
            if (activePickup != PuzzleSpace.Empty && !usedActivePickup) {
                state = CarState.DidntUsePickup;
                return;
            }
            activePickup = spacePickup;
            pickupCoors.Add(to);
            usedActivePickup = false;
        }
        from = to;
        if (to == next) {
            from = to;
            to = next;
            return;
        }
        PuzzleSpace fromPickup = activePickup;
        direction = GetNewDirection(from, fromPickup);
        to = from + direction;
        // Mark pickup as used.
        if (Util.IsTurn(last, from, to)) {
            usedActivePickup = true;
        }
        // Determine pickup leaving the "to" space.
        PuzzleSpace toPickup = GetSpaceWithPath(to);
        if (toPickup == PuzzleSpace.Empty) {
            toPickup = fromPickup;
        }
        Vector2Int nextDirection = GetNewDirection(to, toPickup);
        if (puzzleScript.CanMoveInDirection(to, nextDirection)) {
            next = to + nextDirection;
        } else {
            next = to;
        }
    }
    PuzzleSpace GetSpaceWithPath(Vector2Int coor) {
        if (pickupCoors.Contains(coor)) return PuzzleSpace.Empty;
        return puzzleScript.GetSpace(coor);
    }
    Vector2Int GetNewDirection(Vector2Int arriveCoor, PuzzleSpace pickup) {
        if (pickup == PuzzleSpace.Empty) return direction;
        int turn = (pickup == PuzzleSpace.Left || pickup == PuzzleSpace.LeftForce) ? -1 : 1;
        bool force = pickup == PuzzleSpace.LeftForce || pickup == PuzzleSpace.RightForce;
        // Check if we can go straight.
        if (puzzleScript.CanMoveInDirection(arriveCoor, direction) && !force) {
            return direction;
        }
        // Check if we can make the turn.
        Vector2Int turnDirection = Util.Turn(direction, turn);
        if (puzzleScript.CanMoveInDirection(arriveCoor, turnDirection)) {
            return turnDirection;
        }
        // Otherwise, just go straight.
        return direction;
    }

    public bool IsGoing() {
        return state != CarState.Waiting;
    }
    public bool PickingUp(Vector2Int coor) {
        if (!IsGoing()) return false;
        if (to == coor && t > .5f) return true;
        return PickupActive(coor);
    }
    public bool PickupActive(Vector2Int coor) {
        if (!IsGoing()) return false;
        return pickupCoors.Count > 0 && pickupCoors[pickupCoors.Count - 1] == coor;
    }
    public bool PickupGone(Vector2Int coor) {
        if (!IsGoing()) return false;
        int index = pickupCoors.IndexOf(coor);
        return index >= 0 && index < pickupCoors.Count - 1;
    }
    public bool UsedActivePickup() {
        return usedActivePickup;
    }
}

public enum CarState {
    Waiting, Going,
    Crashed,
    DidntUsePickup,
}