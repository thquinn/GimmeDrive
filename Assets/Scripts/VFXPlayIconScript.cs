using UnityEngine;
using UnityEngine.UI;

public class VFXPlayIconScript : MonoBehaviour
{
    public Image image;
    public Sprite iconPlay, iconStop;

    void Update() {
        image.sprite = CarScript.instance?.IsGoing() == false ? iconPlay : iconStop;
    }
}
