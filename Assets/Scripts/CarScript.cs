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

    public float speed;

    bool going;
    Vector2Int direction;
    Vector2Int from, to, next;
    float t;
    PuzzleSpace heldPickup;

    void Start() {
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!going) Go();
            else Stop();
        }
        if (puzzleScript.entryCoors.Count == 0) {
            visuals.SetActive(false);
        } else if (going) {
            Going();
        } else {
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
        going = true;
        to = puzzleScript.entryCoors[0];
        if (to.x == -1) direction = new(1, 0);
        else if (to.y == -1) direction = new(0, 1);
        else if (to.x == puzzleScript.puzzle.width) direction = new(-1, 0);
        else if (to.y == puzzleScript.puzzle.height) direction = new(0, -1);
        else throw new System.Exception("Entry coor not on edge.");
        t = 0;
        ArriveAtCoor();
    }
    public void Stop() {
        going = false;
    }

    void Going() {
        visuals.SetActive(true);
        t += Time.deltaTime * speed;
        while (t >= 1) {
            ArriveAtCoor();
            t--;
        }
        Vector2 lerpedCoor = Vector2.Lerp(from, to, t);
        transform.localPosition = new Vector3(lerpedCoor.x, 0, -lerpedCoor.y);
    }
    void ArriveAtCoor() {
        if (from == to) return;
        if (to == next) {
            from = to;
            to = next;
            return;
        }
        PuzzleSpace spacePickup = puzzleScript.GetSpace(to);
        if (spacePickup != PuzzleSpace.Empty) {
            heldPickup = spacePickup;
        }
        from = to;
        PuzzleSpace fromPickup = heldPickup;
        direction = GetNewDirection(from, fromPickup);
        to = from + direction;
        // Determine pickup leaving the "to" space.
        PuzzleSpace toPickup = puzzleScript.GetSpace(to);
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
}
