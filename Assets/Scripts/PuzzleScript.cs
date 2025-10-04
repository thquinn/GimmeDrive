using System.Drawing;
using UnityEngine;

public class PuzzleScript : MonoBehaviour {
    public GameObject prefabPickup;
    public Puzzles puzzles;

    [HideInInspector] public Puzzle puzzle;
    [HideInInspector] public bool[,] roadsHorizontal, roadsVertical;

    void Start() {
        Init(0);
    }
    public void Init(int puzzleIndex) {
        puzzle = new Puzzle(puzzles.puzzleStrings[puzzleIndex]);
        roadsHorizontal = new bool[puzzle.width + 1, puzzle.height + 2];
        roadsVertical = new bool[puzzle.width + 2, puzzle.height + 1];
        for (int x = 0; x < puzzle.width; x++) {
            for (int y = 0; y < puzzle.height; y++) {
                PuzzleSpace space = puzzle.spaces[x, y];
                if (space != PuzzleSpace.Empty) {
                    Instantiate(prefabPickup, transform).GetComponent<PickupScript>().Init(this, x, y);
                }
            }
        }
    }
    public bool HasLeftConnection(Vector2Int coor) { return HasConnection(roadsHorizontal, coor.x, coor.y + 1); }
    public bool HasRightConnection(Vector2Int coor) { return HasConnection(roadsHorizontal, coor.x + 1, coor.y + 1); }
    public bool HasTopConnection(Vector2Int coor) { return HasConnection(roadsVertical,coor.x + 1, coor.y); }
    public bool HasBottomConnection(Vector2Int coor) { return HasConnection(roadsVertical, coor.x + 1, coor.y + 1); }
    bool HasConnection(bool[,] roads, int x, int y) {
        if (x < 0 || y < 0 || x >= roads.GetLength(0) || y >= roads.GetLength(1)) return false;
        return roads[x, y];
    }

    void Update() {
        
    }
}
