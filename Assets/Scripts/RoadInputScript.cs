using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoadInputScript : MonoBehaviour {
    static Vector2Int INVALID_COOR = new Vector2Int(-999, -999);

    public GameObject prefabRoad;

    public PuzzleScript puzzleScript;
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
        for (int x = -1; x < puzzleScript.puzzle.width; x++) {
            for (int y = -1; y < puzzleScript.puzzle.height; y++) {
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
        if (clickedCoor == INVALID_COOR) return;
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
    void TryToggle(bool horizontal, Vector2Int coor) {
        bool[,] roads = horizontal ? puzzleScript.roadsHorizontal : puzzleScript.roadsVertical;
        if (coor.x < 0 || coor.y < 0 || coor.x >= roads.GetLength(0) || coor.y >= roads.GetLength(1)) return;
        bool roadState = roads[coor.x, coor.y];
        if (drawState == DrawState.Undetermined) {
            drawState = roadState ? DrawState.Erasing : DrawState.Drawing;
        }
        if (!roadState && drawState == DrawState.Drawing) roads[coor.x, coor.y] = true;
        if (roadState && drawState == DrawState.Erasing) roads[coor.x, coor.y] = false;
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
        return INVALID_COOR;
    }
}

enum DrawState {
    Undetermined, Drawing, Erasing
}
