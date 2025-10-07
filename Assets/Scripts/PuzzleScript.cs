using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class PuzzleScript : MonoBehaviour {
    public static PuzzleScript instance;

    public GameObject prefabPickup, prefabBlock;
    public Material materialGrid;
    public Puzzles puzzles;

    public CarScript carScript;
    public GameObject grid;

    [HideInInspector] public string puzzleName;
    [HideInInspector] public Puzzle puzzle;
    [HideInInspector] public bool[,] roadsHorizontal, roadsVertical;
    [HideInInspector] public List<Vector2Int> entryCoors;
    [HideInInspector] public bool won;

    void Start() {
        instance = this;
    }
    public void Init(string puzzleName) {
        this.puzzleName = puzzleName;
        puzzle = puzzles.GetPuzzleWithName(puzzleName);
        transform.localPosition = new Vector3((puzzle.width - 1) / -2f, 0, (puzzle.height - 1) / 2f);
        roadsHorizontal = new bool[puzzle.width + 1, puzzle.height + 2];
        roadsVertical = new bool[puzzle.width + 2, puzzle.height + 1];
        entryCoors = new List<Vector2Int>();
        for (int x = 0; x < puzzle.width; x++) {
            for (int y = 0; y < puzzle.height; y++) {
                PuzzleSpace space = puzzle.spaces[x, y];
                if (space != PuzzleSpace.Empty) {
                    if (space == PuzzleSpace.Block) {
                        Instantiate(prefabBlock, transform).transform.localPosition = new Vector3(x, 0, -y);
                    } else {
                        Instantiate(prefabPickup, transform).GetComponent<PickupScript>().Init(this, new Vector2Int(x, y));
                    }
                }
            }
        }
        grid.transform.localScale = new Vector3(puzzle.width + 1.5f, puzzle.height + 1.5f, 1);
        grid.transform.position = Vector3.zero;
        materialGrid.SetVector("_GridOffset", new Vector2(puzzle.width % 2 == 1 ? 0 : 0.5f, puzzle.height % 2 == 1 ? 0 : 0.5f));
        LoadSolution();
    }
    public bool HasLeftConnection(Vector2Int coor) { return HasConnection(roadsHorizontal, coor.x, coor.y + 1); }
    public bool HasRightConnection(Vector2Int coor) { return HasConnection(roadsHorizontal, coor.x + 1, coor.y + 1); }
    public bool HasTopConnection(Vector2Int coor) { return HasConnection(roadsVertical,coor.x + 1, coor.y); }
    public bool HasBottomConnection(Vector2Int coor) { return HasConnection(roadsVertical, coor.x + 1, coor.y + 1); }
    bool HasConnection(bool[,] roads, int x, int y) {
        if (x < 0 || y < 0 || x >= roads.GetLength(0) || y >= roads.GetLength(1)) return false;
        return roads[x, y];
    }
    public int PathCount() {
        int count = 0;
        foreach (bool road in roadsHorizontal) {
            if (road) count++;
        }
        foreach (bool road in roadsVertical) {
            if (road) count++;
        }
        return count;
    }
    public bool IsBlocked(Vector2Int coor, bool horizontal) {
        coor.x--;
        coor.y--;
        if (GetSpace(coor) == PuzzleSpace.Block) return true;
        coor += horizontal ? Vector2Int.right : Vector2Int.up;
        return GetSpace(coor) == PuzzleSpace.Block;
    }

    public PuzzleSpace GetSpace(Vector2Int coor) {
        if (coor.x < 0 || coor.x >= puzzle.width || coor.y < 0 || coor.y >= puzzle.height) {
            return PuzzleSpace.Empty;
        }
        return puzzle.spaces[coor.x, coor.y];
    }
    public bool CanMoveInDirection(Vector2Int coor, Vector2Int direction) {
        if (direction.x < 0) coor.x--;
        if (direction.y < 0) coor.y--;
        return direction.y == 0 ? HasRightConnection(coor) : HasBottomConnection(coor);
    }

    public void SaveSolution() {
        SaveData data = new SaveData(roadsHorizontal, roadsVertical, entryCoors);
        string key = puzzleName + "_state";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }
    public void LoadSolution() {
        string key = puzzleName + "_state";
        if (!PlayerPrefs.HasKey(key)) return;
        SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(key));
        roadsHorizontal = data.GetRoadsHorizontal();
        roadsVertical = data.GetRoadsVertical();
        entryCoors = data.entryCoors;
    }

    public void Finish() {
        instance = null;
        Destroy(gameObject);
    }
}

[Serializable]
class SaveData {
    public int rhWidth, rvWidth;
    public bool[] rh, rv;
    public List<Vector2Int> entryCoors;

    public SaveData(bool[,] roadsHorizontal, bool[,] roadsVertical, List<Vector2Int> entryCoors) {
        rhWidth = roadsHorizontal.GetLength(0);
        rh = new bool[roadsHorizontal.Length];
        for (int i = 0; i < rh.Length; i++) rh[i] = roadsHorizontal[i % rhWidth, i / rhWidth];
        rvWidth = roadsVertical.GetLength(0);
        rv = new bool[roadsVertical.Length];
        for (int i = 0; i < rv.Length; i++) rv[i] = roadsVertical[i % rvWidth, i / rvWidth];
        this.entryCoors = entryCoors;
    }
    public bool[,] GetRoadsHorizontal() {
        return GetRoads(rh, rhWidth);
    }
    public bool[,] GetRoadsVertical() {
        return GetRoads(rv, rvWidth);
    }
    bool[,] GetRoads(bool[] arr, int width) {
        int height = arr.Length / width;
        bool[,] ret = new bool[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                ret[x, y] = arr[y * width + x];
            }
        }
        return ret;
    }
}
