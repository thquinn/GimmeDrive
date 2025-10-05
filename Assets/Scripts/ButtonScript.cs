using Assets.Code;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public RectTransform rtCircle;
    public Image icon;
    public ButtonCondition condition;

    bool hovered;
    float vScale, vAlpha;

    void Update() {
        // Dimming.
        bool clickable = true;
        if (condition == ButtonCondition.Play) {
            clickable = PuzzleScript.instance.entryCoors.Count > 0;
        } else if (condition == ButtonCondition.Undo) {
            clickable = RoadInputScript.instance.CanUndo();
        } else if (condition == ButtonCondition.Redo) {
            clickable = RoadInputScript.instance.CanRedo();
        } else if (condition == ButtonCondition.Clear) {
            clickable = RoadInputScript.instance.CanClear();
        }
        icon.SetAlpha(Mathf.SmoothDamp(icon.color.a, clickable ? 1 : 0.5f, ref vAlpha, 0.1f));
        // Scale.
        float targetScale = 1;
        if (hovered && clickable) {
            targetScale = Input.GetMouseButton(0) ? .95f : 1.05f;
        }
        float scale = Mathf.SmoothDamp(rtCircle.localScale.x, targetScale, ref vScale, 0.05f);
        rtCircle.localScale = new Vector3(scale, scale, 1);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hovered = true;
    }
    public void OnPointerExit(PointerEventData eventData) {
        hovered = false;
    }
}

public enum ButtonCondition {
    None, Play, Undo, Redo, Clear
}