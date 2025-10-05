using UnityEngine;
using UnityEngine.UI;

public class PuzzleUIScript : MonoBehaviour {
    public static PuzzleUIScript instance;

    public Slider speedSlider;

    void Start() {
        instance = this;
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
