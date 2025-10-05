using UnityEngine;

public class PickupScript : MonoBehaviour {
    public Transform transformVisuals;
    public MeshRenderer meshRenderer;
    public SpriteRenderer srArrows, srForce;

    public Material materialRight;
    public Color colorForceRight;

    PuzzleScript puzzleScript;
    Vector2Int coor;
    bool right;
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
        } else {
            transform.localPosition = new Vector3(coor.x, 0, -coor.y);
            v = Vector3.zero;
        }
        srArrows.transform.localRotation = Quaternion.Euler(0, right ? 180 : 0, Time.time * 45);
    }
}
