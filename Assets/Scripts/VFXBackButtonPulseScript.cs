using Assets.Code;
using UnityEngine;
using UnityEngine.UI;

public class VFXBackButtonPulseScript : MonoBehaviour
{
    public Image image;

    public float targetScale, dampTime, fadeStartScale;
    float v;

    void Update() {
        float scale = Mathf.SmoothDamp(transform.localScale.x, targetScale, ref v, dampTime);
        transform.localScale = new Vector3(scale, scale, 1);
        image.SetAlpha(Mathf.InverseLerp(targetScale, fadeStartScale, scale));
        if (image.color.a <= 0) {
            Destroy(gameObject);
        }
    }
}
