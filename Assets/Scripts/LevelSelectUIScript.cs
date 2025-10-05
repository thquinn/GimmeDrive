using System.Collections.Generic;
using UnityEngine;

public class LevelSelectUIScript : MonoBehaviour
{
    public static LevelSelectUIScript instance;
    static Vector3 POSITION_HIDDEN = new Vector3(0, -1200, 0);

    public GameObject prefabLevelCell, prefabPuzzle;

    public Puzzles puzzles;
    public Transform levelGrid;

    bool hidden;
    Dictionary<string, int> pathScores;
    Vector3 v;

    void Start() {
        instance = this;
        pathScores = new();
        PopulateLevels();
    }

    void Update() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hidden ? POSITION_HIDDEN : Vector3.zero, ref v, 0.25f);
    }

    void PopulateLevels() {
        foreach (Transform child in levelGrid) {
            if (child.GetSiblingIndex() == 0) continue;
            Destroy(child.gameObject);
        }
        foreach (PuzzleEntry puzzleEntry in puzzles.puzzleStrings) {
            string puzzleName = puzzleEntry.name;
            Puzzle puzzle = puzzles.GetPuzzleWithName(puzzleName);
            if (puzzle == null) {
                Debug.Log($"Missing puzzle \"{puzzleName}\".");
                continue;
            }
            Instantiate(prefabLevelCell, levelGrid).GetComponent<LevelCellScript>().Init(puzzleName);
        }
    }

    public void StartPuzzle(string puzzleName) {
        if (hidden) return;
        Instantiate(prefabPuzzle).GetComponent<PuzzleScript>().Init(puzzleName);
        hidden = true;
    }
    public void QuitPuzzle() {
        PuzzleScript.instance.Finish();
        hidden = false;
        PopulateLevels();
    }

    public int GetPathScore(string puzzleName) {
        if (pathScores.ContainsKey(puzzleName)) return pathScores[puzzleName];
        return -1;
    }
    public void SetPathScore(string puzzleName, int score) {
        if (!pathScores.ContainsKey(puzzleName)) pathScores[puzzleName] = score;
        pathScores[puzzleName] = Mathf.Min(pathScores[puzzleName], score);
    }
}
