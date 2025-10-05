using UnityEngine;

public class PickupScript : MonoBehaviour {
    public Transform transformVisuals;
    public MeshRenderer meshRenderer;
    public SpriteRenderer srArrows, srForce;

    public Material materialLeft, materialRight;
    public Color colorForceLeft, colorForceRight;

    PuzzleScript puzzleScript;
    Vector2Int coor;
    bool right, picked;
    Vector3 v;

    public void Init(PuzzleScript puzzleScript, Vector2Int coor) {
        this.puzzleScript = puzzleScript;
        this.coor = coor;
        PuzzleSpace pickup = puzzleScript.GetSpace(coor);
        right = pickup == PuzzleSpace.Right || pickup == PuzzleSpace.RightForce;
        if (right) {
            meshRenderer.material = materialRight;
            srForce.color = colorForceRight;
        }
        srForce.gameObject.SetActive(pickup == PuzzleSpace.LeftForce || pickup == PuzzleSpace.RightForce);
    }

    void Update() {
        CarScript carScript = puzzleScript.carScript;
        transformVisuals.gameObject.SetActive(!carScript.PickupGone(coor));
        if (carScript.PickupActive(coor)) {
            transform.position = Vector3.SmoothDamp(transform.position, carScript.pickupAnchor.position, ref v, 0.1f);
            if (!picked) {
                v = new Vector3(0, 10, 0);
                picked = true;
            }
        } else {
            transform.localPosition = new Vector3(coor.x, 0, -coor.y);
            picked = false;
            v = Vector3.zero;
        }
        srArrows.transform.localRotation = Quaternion.Euler(0, right ? 180 : 0, Time.time * 45);
    }
}
