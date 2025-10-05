using UnityEngine;

public class VFXBackButtonPulseEmitterScript : MonoBehaviour
{
    public GameObject prefabBackButtonPulse;

    public float rate;
    float timer;

    void Update() {
        if (PuzzleScript.instance?.won == true) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                Instantiate(prefabBackButtonPulse, transform);
                timer += rate;
            }
        }
    }
}
