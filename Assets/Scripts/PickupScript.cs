using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PickupScript : MonoBehaviour {
    public GameObject prefabVFXPulse;

    public Transform transformVisuals;
    public MeshRenderer meshRenderer;
    public SpriteRenderer srArrows, srForce;

    public Material materialLeft, materialRight, materialUsed;
    public Color colorForceLeft, colorForceRight, colorForceUsed;

    PuzzleScript puzzleScript;
    Vector2Int coor;
    bool right, picked, used;
    Vector3 v;
    float pickedT;

    public void Init(PuzzleScript puzzleScript, Vector2Int coor) {
        this.puzzleScript = puzzleScript;
        this.coor = coor;
        PuzzleSpace pickup = puzzleScript.GetSpace(coor);
        right = pickup == PuzzleSpace.Right || pickup == PuzzleSpace.RightForce;
        InitMaterials();
        srForce.gameObject.SetActive(pickup == PuzzleSpace.LeftForce || pickup == PuzzleSpace.RightForce);
    }
    void InitMaterials() {
        if (right) {
            meshRenderer.material = materialRight;
            srForce.color = colorForceRight;
        } else {
            meshRenderer.material = materialLeft;
            srForce.color = colorForceLeft;
        }
    }

    void Update() {
        CarScript carScript = puzzleScript.carScript;
        transformVisuals.gameObject.SetActive(!carScript.PickupGone(coor));
        if (carScript.PickingUp(coor)) {
            transform.position = Vector3.SmoothDamp(transform.position, carScript.pickupAnchor.position, ref v, 0.1f / (1 + pickedT));
            if (!picked) {
                v = new Vector3(0, 10, 0);
                picked = true;
            }
            if (!used && carScript.PickupActive(coor) && carScript.UsedActivePickup()) {
                Instantiate(prefabVFXPulse, transformVisuals).GetComponent<SpriteRenderer>().color = meshRenderer.material.color;
                meshRenderer.material = materialUsed;
                srForce.color = colorForceUsed;
                used = true;
            }
        } else {
            transform.localPosition = new Vector3(coor.x, 0, -coor.y);
            picked = false;
            used = false;
            v = Vector3.zero;
            InitMaterials();
        }
        pickedT = picked ? pickedT + Time.deltaTime : 0;
        srArrows.transform.localRotation = Quaternion.Euler(0, right ? 180 : 0, Time.time * 45);
    }
}
