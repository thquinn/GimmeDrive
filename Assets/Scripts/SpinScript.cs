using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public float rate;

    void Update() {
        transform.localRotation = Quaternion.Euler(0, Time.time * rate, 0);
    }
}
