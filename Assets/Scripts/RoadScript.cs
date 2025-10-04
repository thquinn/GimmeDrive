using UnityEngine;

public class RoadScript : MonoBehaviour {
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Mesh meshCross, meshCurve, meshEnd, meshStraight, meshT;

    PuzzleScript puzzleScript;
    Vector2Int coor;

    public void Init(PuzzleScript puzzleScript, Vector2Int coor) {
        this.puzzleScript = puzzleScript;
        this.coor = coor;
        transform.localPosition = new Vector3(coor.x, 0, -coor.y);
        Update();
    }
    void Update() {
        int numConnections = 0;
        bool left = puzzleScript.HasLeftConnection(coor);
        bool right = puzzleScript.HasRightConnection(coor);
        bool top = puzzleScript.HasTopConnection(coor);
        bool bottom = puzzleScript.HasBottomConnection(coor);
        if (left) numConnections++;
        if (right) numConnections++;
        if (top) numConnections++;
        if (bottom) numConnections++;
        if (numConnections == 0) {
            Destroy(gameObject);
            return;
        }
        if (numConnections == 1) {
            meshFilter.mesh = meshEnd;
            if (right) transform.localRotation = Quaternion.identity;
            else if (bottom) transform.localRotation = Quaternion.Euler(0, 90, 0);
            else if (left) transform.localRotation = Quaternion.Euler(0, 180, 0);
            else transform.localRotation = Quaternion.Euler(0, 270, 0);
        } else if (numConnections == 2) {
            if ((left && right) || (top && bottom)) {
                meshFilter.mesh = meshStraight;
                transform.localRotation = left ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
            } else {
                meshFilter.mesh = meshCurve;
                if (right && bottom) transform.localRotation = Quaternion.identity;
                else if (left && bottom) transform.localRotation = Quaternion.Euler(0, 90, 0);
                else if (left && top) transform.localRotation = Quaternion.Euler(0, 180, 0);
                else transform.localRotation = Quaternion.Euler(0, 270, 0);
            }
        } else if (numConnections == 3) {
            meshFilter.mesh = meshT;
            if (!left) transform.localRotation = Quaternion.identity;
            else if (!top) transform.localRotation = Quaternion.Euler(0, 90, 0);
            else if (!right) transform.localRotation = Quaternion.Euler(0, 180, 0);
            else transform.localRotation = Quaternion.Euler(0, 270, 0);
        } else {
            meshFilter.mesh = meshCross;
        }
    }
}
