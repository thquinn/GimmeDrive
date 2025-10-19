using Assets.Code;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleUIScript : MonoBehaviour {
    public static PuzzleUIScript instance;
    static float HOTKEY_REPEAT_DELAY = 0.5f;
    static float HOTKEY_REPEAT_TIME = 0.1f;
    static Vector3 HIDDEN_POSITION = new Vector3(0, 200, 0);

    public TMP_Text tmpLevelName, tmpPathCount, tmpSliderLabel;
    public Slider speedSlider;
    public CanvasGroup cgPathCount;

    float undoTimer, redoTimer;
    Vector3 v;
    float vPathCountAlpha, vSliderLabelAlpha;

    void Start() {
        instance = this;
        transform.localPosition = HIDDEN_POSITION;
        if (PlayerPrefs.HasKey("PlaybackSpeed")) {
            speedSlider.value = PlayerPrefs.GetFloat("PlaybackSpeed");
        }
    }
    void Update() {
        // Hotkeys.
        undoTimer = Input.GetKey(KeyCode.Z) ? undoTimer + Time.deltaTime : 0;
        redoTimer = Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.X) ? redoTimer + Time.deltaTime : 0;
        if (HotkeyRepeat(undoTimer)) {
            RoadInputScript.instance?.Undo();
        } else if (HotkeyRepeat(redoTimer)) {
            RoadInputScript.instance?.Redo();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            RoadInputScript.instance?.Clear();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            LevelSelectUIScript.instance.QuitPuzzle();
        }
        // Slider.
        tmpSliderLabel.text = $"{speedSlider.value}<voffset=.1em>x";
        bool sliderActive = EventSystem.current.currentSelectedGameObject == speedSlider.gameObject && Input.GetMouseButton(0);
        tmpSliderLabel.SetAlpha(Mathf.SmoothDamp(tmpSliderLabel.color.a, sliderActive ? 1 : 0, ref vSliderLabelAlpha, 0.1f));
        // State.
        bool hidden = PuzzleScript.instance == null;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hidden ? HIDDEN_POSITION : Vector3.zero, ref v, 0.2f);
        if (hidden) return;
        tmpLevelName.text = $"\"{PuzzleScript.instance.puzzleName}\"";
        int pathCount = PuzzleScript.instance.PathCount();
        tmpPathCount.text = pathCount.ToString();
        cgPathCount.alpha = Mathf.SmoothDamp(cgPathCount.alpha, pathCount == 0 ? 0 : 1, ref vPathCountAlpha, 0.1f);
    }
    bool HotkeyRepeat(float timer) {
        if (timer == Time.deltaTime) return true;
        timer -= HOTKEY_REPEAT_DELAY;
        if (timer < 0) return false;
        float before = timer - Time.deltaTime;
        return timer % HOTKEY_REPEAT_TIME < before % HOTKEY_REPEAT_TIME;
    }

    public void ClickBack() {
        LevelSelectUIScript.instance.QuitPuzzle();
        SFXScript.SFXButton();
    }
    public void ClickPlay() {
        CarScript.instance.TogglePlay();
        SFXScript.SFXButton();
    }
    public void ClickUndo() {
        RoadInputScript.instance.Undo();
        SFXScript.SFXButton();
    }
    public void ClickRedo() {
        RoadInputScript.instance.Redo();
        SFXScript.SFXButton();
    }
    public void ClickClear() {
        RoadInputScript.instance.Clear();
        SFXScript.SFXButton();
    }
    public void ChangeSpeed() {
        PlayerPrefs.SetFloat("PlaybackSpeed", speedSlider.value);
        PlayerPrefs.Save();
    }
}
