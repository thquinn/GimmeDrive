using UnityEngine;

public class PickupScript : MonoBehaviour {
    public Transform transformVisuals;
    public SpriteRenderer srArrows, srForce;

    PuzzleScript puzzleScript;
    int x, y;
    bool flipped;

    public void Init(PuzzleScript puzzleScript, int x, int y) {
        this.puzzleScript = puzzleScript;
        transform.localPosition = new Vector3(x, 0, -y);
        PuzzleSpace pickup = puzzleScript.puzzle.spaces[x, y];
        flipped = pickup == PuzzleSpace.Right || pickup == PuzzleSpace.RightForce;
        srForce.gameObject.SetActive(pickup == PuzzleSpace.LeftForce || pickup == PuzzleSpace.RightForce);
    }

    void Update() {
        srArrows.transform.localRotation = Quaternion.Euler(0, flipped ? 180 : 0, Time.time * 45);
    }
}
