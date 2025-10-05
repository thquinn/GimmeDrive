using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour {
    static float EDGE_SCALE = 5;

    public Material materialRoadFade;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Mesh meshCross, meshCurve, meshEnd, meshStraight, meshT;

    PuzzleScript puzzleScript;
    Vector2Int coor;
    bool edge;
    Vector3 initialPosition;
    float edgeScale, vScale;

    public void Init(PuzzleScript puzzleScript, Vector2Int coor) {
        this.puzzleScript = puzzleScript;
        this.coor = coor;
        initialPosition = new Vector3(coor.x, 0, -coor.y);
        transform.localPosition = initialPosition;
        edge = coor.x == -1 || coor.y == -1 || coor.x == puzzleScript.puzzle.width || coor.y == puzzleScript.puzzle.height;
        if (edge) {
            meshRenderer.material = materialRoadFade;
            edgeScale = 1;
        }
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
            if (right) transform.localRotation = Quaternion.identity;
            else if (bottom) transform.localRotation = Quaternion.Euler(0, 90, 0);
            else if (left) transform.localRotation = Quaternion.Euler(0, 180, 0);
            else transform.localRotation = Quaternion.Euler(0, 270, 0);
            if (edge) {
                meshFilter.mesh = meshStraight;
                edgeScale = Mathf.SmoothDamp(edgeScale, EDGE_SCALE, ref vScale, 0.1f);
                transform.localScale = new Vector3(edgeScale, 1, 1);
                transform.localPosition = initialPosition - transform.right * ((edgeScale - 1) / 2);
            } else {
                meshFilter.mesh = meshEnd;
            }
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
