using UnityEngine;

[CreateAssetMenu(fileName = "Puzzles", menuName = "Scriptable Objects/Puzzles")]
public class Puzzles : ScriptableObject {
    public string[] puzzleStrings;
}

public class Puzzle {
    public PuzzleSpace[,] spaces;
}

public enum PuzzleSpace {
    Empty, Left, LeftForce, Right, RightForce
}