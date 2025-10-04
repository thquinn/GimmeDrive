using UnityEngine;

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject {
    public string[] puzzleStrings;
}

public class Puzzle {
    public int width, height;
    public PuzzleSpace[,] spaces;
    public Puzzle(string puzzleString) {
        string[] lines = puzzleString.Split('\n');
        width = lines[0].Length;
        height = lines.Length;
        spaces = new PuzzleSpace[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                char c = lines[y][x];
                if (c == 'l') spaces[x, y] = PuzzleSpace.Left;
                if (c == 'L') spaces[x, y] = PuzzleSpace.LeftForce;
                if (c == 'r') spaces[x, y] = PuzzleSpace.Right;
                if (c == 'R') spaces[x, y] = PuzzleSpace.RightForce;
            }
        }
    }
}

public enum PuzzleSpace {
    Empty, Left, LeftForce, Right, RightForce
}