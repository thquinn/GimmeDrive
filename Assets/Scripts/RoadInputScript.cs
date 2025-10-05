using Assets.Code;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoadInputScript : MonoBehaviour {
    public GameObject prefabRoad;

    public PuzzleScript puzzleScript;
    public CarScript carScript;
    Dictionary<Vector2Int, RoadScript> roads;
    DrawState drawState;
    Vector2Int lastClickedCoor;

    void Start() {
        roads = new();
    }

    void Update() {
        if (puzzleScript.puzzle == null) return;
        if (Input.GetMouseButton(0)) {
            UpdateClick();
        } if (Input.GetMouseButtonUp(0)) {
            drawState = DrawState.Undetermined;
        }
        for (int x = -1; x < puzzleScript.puzzle.width + 1; x++) {
            for (int y = -1; y < puzzleScript.puzzle.height + 1; y++) {
                Vector2Int coor = new Vector2Int(x, y);
                if (roads.GetValueOrDefault(coor) != null) continue;
                if (puzzleScript.HasLeftConnection(coor) || puzzleScript.HasRightConnection(coor) || puzzleScript.HasTopConnection(coor) || puzzleScript.HasBottomConnection(coor)) {
                    RoadScript road = Instantiate(prefabRoad, transform).GetComponent<RoadScript>();
                    road.Init(puzzleScript, coor);
                    roads[coor] = road;
                }
            }
        }
    }
    void UpdateClick() {
        Vector2Int clickedCoor = GetClickedCoor();
        if (clickedCoor == Util.INVALID_COOR) return;
        if (Input.GetMouseButtonDown(0)) lastClickedCoor = clickedCoor;
        while (clickedCoor.x < lastClickedCoor.x) {
            lastClickedCoor.x--;
            TryToggle(true, lastClickedCoor);
        }
        while (clickedCoor.x > lastClickedCoor.x) {
            TryToggle(true, lastClickedCoor);
            lastClickedCoor.x++;
        }
        while (clickedCoor.y < lastClickedCoor.y) {
            lastClickedCoor.y--;
            TryToggle(false, lastClickedCoor);
        }
        while (clickedCoor.y > lastClickedCoor.y) {
            TryToggle(false, lastClickedCoor);
            lastClickedCoor.y++;
        }
        lastClickedCoor = clickedCoor;
    }
    Camera cam;
    Vector2Int GetClickedCoor() {
        if (cam == null) cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distance = 0;
        if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out distance)) {
            Vector3 position = ray.GetPoint(distance);
            Debug.DrawLine(position, position + new Vector3(0, 1, 0));
            position = transform.InverseTransformPoint(position);
            return new Vector2Int(Mathf.FloorToInt(position.x + 1.5f), Mathf.FloorToInt(-position.z + 1.5f));
        }
        return Util.INVALID_COOR;
    }
    void TryToggle(bool horizontal, Vector2Int coor) {
        if (carScript.IsGoing()) return;
        bool[,] roads = horizontal ? puzzleScript.roadsHorizontal : puzzleScript.roadsVertical;
        if (horizontal) {
            if (coor.y == 0 || coor.y == roads.GetLength(1) - 1) return;
        } else {
            if (coor.x == 0 || coor.x == roads.GetLength(0) - 1) return;
        }
        if (coor.x < 0 || coor.y < 0 || coor.x >= roads.GetLength(0) || coor.y >= roads.GetLength(1)) return;
        bool roadState = roads[coor.x, coor.y];
        bool onEdge = coor.x == 0 || coor.y == 0 || coor.x == roads.GetLength(0) - 1 || coor.y == roads.GetLength(1) - 1;
        if (onEdge && !roadState && puzzleScript.entryCoors.Count >= 2) return;
        if (drawState == DrawState.Undetermined) {
            drawState = roadState ? DrawState.Erasing : DrawState.Drawing;
        }
        if (!roadState && drawState == DrawState.Drawing) {
            roads[coor.x, coor.y] = true;
            if (onEdge) puzzleScript.entryCoors.Add(GetEntryCoor(horizontal, coor));
        } else if (roadState && drawState == DrawState.Erasing) {
            roads[coor.x, coor.y] = false;
            if (onEdge) puzzleScript.entryCoors.Remove(GetEntryCoor(horizontal, coor));
        }
    }
    Vector2Int GetEntryCoor(bool horizontal, Vector2Int coor) {
        coor.x--;
        coor.y--;
        if (horizontal && coor.x == puzzleScript.puzzle.width - 1) coor.x++;
        if (!horizontal && coor.y == puzzleScript.puzzle.height - 1) coor.y++;
        return coor;
    }
}

enum DrawState {
    Undetermined, Drawing, Erasing
}
