using Assets.Code;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelCellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image borderOutline, borderFill, pathIcon;
    public TMP_Text tmpName, tmpScore;

    string puzzleName;
    bool hovered;
    float initialFillAlpha, vAlpha;

    void Start() {
        initialFillAlpha = borderFill.color.a;
    }
    public void Init(string puzzleName) {
        this.puzzleName = puzzleName;
        tmpName.text = $"\"{puzzleName}\"";
        int score = LevelSelectUIScript.instance.GetPathScore(puzzleName);
        if (score == -1) {
            RectTransform rt = tmpName.transform as RectTransform;
            Vector2 anchoredPosition = rt.anchoredPosition;
            anchoredPosition.y = 0;
            rt.anchoredPosition = anchoredPosition;
            pathIcon.gameObject.SetActive(false);
            tmpScore.gameObject.SetActive(false);
        } else {
            tmpScore.text = score.ToString();
        }
    }

    void Update() {
        borderOutline.SetAlpha(Mathf.SmoothDamp(borderOutline.color.a, hovered ? 0 : 1, ref vAlpha, 0.05f));
        borderFill.SetAlpha((1 - borderOutline.color.a) * initialFillAlpha);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovered = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        LevelSelectUIScript.instance.StartPuzzle(puzzleName);
    }
}
