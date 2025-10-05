using Assets.Code;
using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour {
    public TMP_Text tmp;

    float vAlpha;

    void Update() {
        PuzzleTutorial tutorial = PuzzleScript.instance?.puzzle.tutorial ?? PuzzleTutorial.None;
        tmp.SetAlpha(Mathf.SmoothDamp(tmp.color.a, tutorial == PuzzleTutorial.None ? 0 : 1, ref vAlpha, 0.1f));
        tmp.text = GetTutorialText(tutorial);
    }

    #pragma warning disable 8524
    static string GetTutorialText(PuzzleTutorial tutorial) => tutorial switch {
        PuzzleTutorial.None => "",
        PuzzleTutorial.Draw => "Click and drag to draw a road that goes straight from one edge to the other.",
        PuzzleTutorial.Turn => "Pick up a star to turn in the indicated direction.",
        PuzzleTutorial.Wait => "You can turn as soon as you grab a star,\nor you can wait until later.",
        PuzzleTutorial.Multi => "You can use a single star to turn many times.",
        PuzzleTutorial.Loop => "You'll often need to draw\na road that crosses itself.",
        PuzzleTutorial.Force => "Stars with a \"!\" will always turn if they can.",
    };
}

public enum PuzzleTutorial {
    None, Draw, Turn, Wait, Multi, Loop, Force
}