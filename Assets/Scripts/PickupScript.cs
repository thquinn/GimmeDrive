using Assets.Code;
using UnityEngine;

public class PickupScript : MonoBehaviour {
    public GameObject prefabVFXPulse;

    public Transform transformVisuals;
    public MeshRenderer meshRenderer;
    public SpriteRenderer srArrows, srForce;

    public Color colorLeft, colorRight, colorUsed;
    public Color colorForceLeft, colorForceRight, colorForceUsed;

    PuzzleScript puzzleScript;
    Vector2Int coor;
    bool right, picked, used;
    Vector3 v;
    float pickedT, vAlpha;

    public void Init(PuzzleScript puzzleScript, Vector2Int coor) {
        this.puzzleScript = puzzleScript;
        this.coor = coor;
        PuzzleSpace pickup = puzzleScript.GetSpace(coor);
        right = pickup == PuzzleSpace.Right || pickup == PuzzleSpace.RightForce;
        InitMaterials(true);
        srForce.gameObject.SetActive(pickup == PuzzleSpace.LeftForce || pickup == PuzzleSpace.RightForce);
    }
    void InitMaterials(bool setAlpha = false) {
        if (right) {
            SetColor(colorRight, setAlpha);
            srForce.color = colorForceRight;
        } else {
            SetColor(colorLeft, setAlpha);
            srForce.color = colorForceLeft;
        }
    }

    void Update() {
        CarScript carScript = puzzleScript.carScript;
        float targetAlpha = carScript.PickupGone(coor) ? 0 : 1;
        srArrows.SetAlpha(Mathf.SmoothDamp(srArrows.color.a, targetAlpha, ref vAlpha, 0.05f));
        srForce.SetAlpha(srArrows.color.a);
        SetAlpha(srArrows.color.a);
        if (carScript.PickingUp(coor) || carScript.PickupGone(coor)) {
            transform.position = Vector3.SmoothDamp(transform.position, carScript.pickupAnchor.position, ref v, 0.1f / (1 + pickedT));
            if (!picked) {
                v = new Vector3(0, 10, 0);
                picked = true;
            }
            if (!used && carScript.PickupActive(coor) && carScript.UsedActivePickup()) {
                Instantiate(prefabVFXPulse, transformVisuals).GetComponent<SpriteRenderer>().color = GetColor();
                SFXScript.SFXTurnPulse(right);
                SetColor(colorUsed);
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

    Color GetColor() {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);
        Color c = block.GetColor("_Tint");
        c.a = 1;
        return c;
    }
    void SetColor(Color newC, bool setAlpha = false) {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);
        Color c = block.GetColor("_Tint");
        c.r = newC.r;
        c.g = newC.g;
        c.b = newC.b;
        if (setAlpha) {
            c.a = newC.a;
        }
        block.SetColor("_Tint", c);
        meshRenderer.SetPropertyBlock(block);
    }
    void SetAlpha(float a) {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);
        Color c = block.GetColor("_Tint");
        c.a = a;
        block.SetColor("_Tint", c);
        meshRenderer.SetPropertyBlock(block);
    }
}
