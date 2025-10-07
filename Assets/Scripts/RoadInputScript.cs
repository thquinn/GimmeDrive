using Assets.Code;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoadInputScript : MonoBehaviour {
    public static RoadInputScript instance;

    public GameObject prefabRoad;

    public PuzzleScript puzzleScript;
    public CarScript carScript;
    Dictionary<Vector2Int, RoadScript> roads;
    DrawState drawState;
    Vector2Int lastClickedCoor;
    Stack<List<UndoEvent>> undoHistory, redoHistory;
    List<UndoEvent> currentUndo;

    void Start() {
        instance = this;
        roads = new();
        undoHistory = new();
        redoHistory = new();
    }

    void Update() {
        if (puzzleScript.puzzle == null) return;
        if (Input.GetMouseButton(0)) {
            UpdateClick();
        } else if (Input.GetMouseButtonUp(0)) {
            if (currentUndo != null) {
                undoHistory.Push(currentUndo);
                currentUndo = null;
                redoHistory.Clear();
            }
            drawState = DrawState.Undetermined;
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            Undo();
        } else if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.X)) {
            Redo();
        } else if (Input.GetKeyDown(KeyCode.R)) {
            Clear();
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
    void TryToggle(bool horizontal, Vector2Int coor, bool undoOperation = false) {
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
        if (puzzleScript.IsBlocked(coor, horizontal)) return;
        if (!undoOperation && drawState == DrawState.Undetermined) {
            drawState = roadState ? DrawState.Erasing : DrawState.Drawing;
        }
        bool drawStateMatch = undoOperation || (!roadState && drawState == DrawState.Drawing) || (roadState && drawState == DrawState.Erasing);
        bool success = false;
        if (!roadState && drawStateMatch) {
            roads[coor.x, coor.y] = true;
            if (onEdge) puzzleScript.entryCoors.Add(GetEntryCoor(horizontal, coor));
            success = true;
        } else if (roadState && drawStateMatch) {
            roads[coor.x, coor.y] = false;
            if (onEdge) puzzleScript.entryCoors.Remove(GetEntryCoor(horizontal, coor));
            success = true;
        }
        if (success && !undoOperation) {
            if (currentUndo == null) currentUndo = new();
            currentUndo.Add(new UndoEvent() { horizontal = horizontal, coor = coor });
            if (drawState == DrawState.Drawing) {
                SFXScript.SFXPlace();
            } else {
                SFXScript.SFXRemove();
            }
        }
    }
    Vector2Int GetEntryCoor(bool horizontal, Vector2Int coor) {
        coor.x--;
        coor.y--;
        if (horizontal && coor.x == puzzleScript.puzzle.width - 1) coor.x++;
        if (!horizontal && coor.y == puzzleScript.puzzle.height - 1) coor.y++;
        return coor;
    }

    public bool CanUndo() {
        return undoHistory.Count > 0;
    }
    public bool CanRedo() {
        return redoHistory.Count > 0;
    }
    public void Undo() {
        if (undoHistory.Count == 0) return;
        if (Input.GetMouseButtonDown(0)) return;
        var undos = undoHistory.Pop();
        ApplyUndos(undos);
        redoHistory.Push(undos);
    }
    public void Redo() {
        if (redoHistory.Count == 0) return;
        if (Input.GetMouseButtonDown(0)) return;
        var undos = redoHistory.Pop();
        ApplyUndos(undos);
        undoHistory.Push(undos);
    }
    void ApplyUndos(List<UndoEvent> undos) {
        foreach (UndoEvent undo in undos) {
            TryToggle(undo.horizontal, undo.coor, true);
        }
    }

    public bool CanClear() {
        for (int x = 0; x < puzzleScript.roadsHorizontal.GetLength(0); x++) {
            for (int y = 0; y < puzzleScript.roadsHorizontal.GetLength(1); y++) {
                if (puzzleScript.roadsHorizontal[x, y]) {
                    return true;
                }
            }
        }
        for (int x = 0; x < puzzleScript.roadsVertical.GetLength(0); x++) {
            for (int y = 0; y < puzzleScript.roadsVertical.GetLength(1); y++) {
                if (puzzleScript.roadsVertical[x, y]) {
                    return true;
                }
            }
        }
        return false;
    }
    public void Clear() {
        List<UndoEvent> undos = new();
        for (int x = 0; x < puzzleScript.roadsHorizontal.GetLength(0); x++) {
            for (int y = 0; y < puzzleScript.roadsHorizontal.GetLength(1); y++) {
                if (puzzleScript.roadsHorizontal[x, y]) {
                    TryToggle(true, new Vector2Int(x, y), true);
                    undos.Add(new UndoEvent() { horizontal = true, coor = new Vector2Int(x, y) });
                }
            }
        }
        for (int x = 0; x < puzzleScript.roadsVertical.GetLength(0); x++) {
            for (int y = 0; y < puzzleScript.roadsVertical.GetLength(1); y++) {
                if (puzzleScript.roadsVertical[x, y]) {
                    TryToggle(false, new Vector2Int(x, y), true);
                    undos.Add(new UndoEvent() { horizontal = false, coor = new Vector2Int(x, y) });
                }
            }
        }
        if (undos.Count > 0) {
            undoHistory.Push(undos);
        }
    }
}

enum DrawState {
    Undetermined, Drawing, Erasing
}

struct UndoEvent {
    public bool horizontal;
    public Vector2Int coor;
}