using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUIScript : MonoBehaviour {
    public static PuzzleUIScript instance;
    static Vector3 HIDDEN_POSITION = new Vector3(0, 200, 0);

    public TMP_Text tmpLevelName;
    public Slider speedSlider;

    Vector3 v;

    void Start() {
        instance = this;
        transform.localPosition = HIDDEN_POSITION;
    }
    void Update() {
        bool hidden = PuzzleScript.instance == null;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hidden ? HIDDEN_POSITION : Vector3.zero, ref v, 0.25f);
        if (hidden) return;
        tmpLevelName.text = $"\"{PuzzleScript.instance.puzzleName}\"";
    }

    public void ClickBack() {
        LevelSelectUIScript.instance.QuitPuzzle();
    }
    public void ClickPlay() {
        CarScript.instance.TogglePlay();
    }
    public void ClickUndo() {
        RoadInputScript.instance.Undo();
    }
    public void ClickRedo() {
        RoadInputScript.instance.Redo();
    }
    public void ClickClear() {
        RoadInputScript.instance.Clear();
    }
}
