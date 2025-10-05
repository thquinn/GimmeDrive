using UnityEngine;

public class VFXPulseScript : MonoBehaviour {
    public SpriteRenderer sr;

    public float targetScale, lifetime, growSpeed, fadeTime;
    float time, vAlpha;
    float vScale;

    void Start() {
        Color c = sr.color;
        c.a = 0;
        sr.color = c;
    }

    void Update() {
        time += Time.deltaTime;
        float scale = Mathf.SmoothDamp(transform.localScale.x, targetScale, ref vScale, lifetime / growSpeed);
        transform.localScale = new Vector3(scale, scale, 1);
        Color c = sr.color;
        c.a = Mathf.SmoothDamp(c.a, time < lifetime ? 1 : 0, ref vAlpha, fadeTime);
        if (time > lifetime && c.a < .01f) {
            Destroy(gameObject);
            return;
        }
        sr.color = c;
    }
}
