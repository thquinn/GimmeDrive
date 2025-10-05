using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleEntry {
    public string name;
    [TextArea(3, 10)] public string content;
}

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject {
    public List<PuzzleEntry> puzzleStrings;

    public Puzzle GetPuzzleWithName(string key) {
        foreach (var kvp in puzzleStrings) {
            if (kvp.name == key) {
                return new Puzzle(kvp.content);
            }
        }
        return null;
    }
}

public class Puzzle {
    public int width, height;
    public PuzzleSpace[,] spaces;
    public Puzzle(string puzzleString) {
        string[] lines = puzzleString.Split(new string[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
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