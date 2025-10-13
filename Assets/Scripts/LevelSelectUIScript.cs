using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectUIScript : MonoBehaviour
{
    public static LevelSelectUIScript instance;
    static Vector3 POSITION_HIDDEN = new Vector3(0, -500, 0);

    public GameObject prefabLevelCell, prefabPuzzle;

    public Puzzles puzzles;
    public CanvasGroup canvasGroup;
    public Transform levelGrid;
    public GameObject totalScoreIcon;
    public TMP_Text totalScoreLabel;

    bool hidden;
    Vector3 v;
    float vAlpha;

    void Start() {
        instance = this;
        PopulateLevels();
        transform.localPosition = POSITION_HIDDEN;
        canvasGroup.alpha = 0;
    }

    void Update() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hidden ? POSITION_HIDDEN : Vector3.zero, ref v, 0.2f);
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, hidden ? 0 : 1, ref vAlpha, 0.1f);
        canvasGroup.interactable = !hidden;
        // Update total score.
        int totalScore = GetTotalScore();
        totalScoreIcon.SetActive(totalScore > 0);
        totalScoreLabel.gameObject.SetActive(totalScore > 0);
        totalScoreLabel.text = totalScore.ToString();
    }
    int GetTotalScore() {
        int total = 0;
        foreach (PuzzleEntry puzzleEntry in puzzles.puzzleStrings) {
            string puzzleName = puzzleEntry.name;
            if (GetPathScore(puzzleName) == -1) return -1;
            total += GetPathScore(puzzleName);
        }
        return total;
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
        if (PuzzleScript.instance == null) return;
        PuzzleScript.instance.Finish();
        hidden = false;
        PopulateLevels();
    }

    public int GetPathScore(string puzzleName) {
        string key = puzzleName + "_score";
        if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetInt(key);
        return -1;
    }
    public bool SetPathScore(string puzzleName, int score) {
        int previousScore = GetPathScore(puzzleName);
        if (previousScore == -1 || score <= previousScore) {
            string key = puzzleName + "_score";
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
