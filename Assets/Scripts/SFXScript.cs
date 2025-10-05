using UnityEngine;

public class SFXScript : MonoBehaviour
{
    public static SFXScript instance;

    public AudioSource sourceEngine, sourceSFX;
    public AudioClip sfxButton, sfxPlace, sfxRemove;

    float vEngine;

    void Start() {
        instance = this;
        sourceEngine.volume = 0;
    }

    void Update() {
        sourceEngine.volume = Mathf.SmoothDamp(sourceEngine.volume, CarScript.instance?.IsGoing(true) == true ? 1 : 0, ref vEngine, .1f);
    }

    public static void SFXButton() {
        instance.sourceSFX.PlayOneShot(instance.sfxButton, 0.1f);
    }
    public static void SFXPlace() {
        instance.sourceSFX.PlayOneShot(instance.sfxPlace, 0.1f);
    }
    public static void SFXRemove() {
        instance.sourceSFX.PlayOneShot(instance.sfxRemove, 0.1f);
    }
}
