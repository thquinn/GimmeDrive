using System;
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
    public PuzzleTutorial tutorial;

    public Puzzle(string puzzleString) {
        List<string> lines = new List<string>(puzzleString.Split(new string[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None));
        while (lines[lines.Count - 1].Contains(' ')) {
            string[] tokens = lines[lines.Count - 1].Split(' ');
            lines.RemoveAt(lines.Count - 1);
            if (tokens[0] == "tut") tutorial = (PuzzleTutorial) Enum.Parse(typeof(PuzzleTutorial), tokens[1]);
        }
        width = lines[0].Length;
        height = lines.Count;
        spaces = new PuzzleSpace[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                char c = lines[y][x];
                if (c == 'l') spaces[x, y] = PuzzleSpace.Left;
                if (c == 'L') spaces[x, y] = PuzzleSpace.LeftForce;
                if (c == 'r') spaces[x, y] = PuzzleSpace.Right;
                if (c == 'R') spaces[x, y] = PuzzleSpace.RightForce;
                if (c == 'X') spaces[x, y] = PuzzleSpace.Block;
            }
        }
    }
}

public enum PuzzleSpace {
    Empty, Left, LeftForce, Right, RightForce, Block
}