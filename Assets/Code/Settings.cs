using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle skipThrow;
    public Toggle shake;

    public void OnEnable()
    {
        skipThrow.isOn = GameSettings.Get<bool>("SkipThrow");
        shake.isOn = GameSettings.Get<bool>("Shake");
    }

    public void SkipDiceThrow(bool value)
    {
        GameSettings.Set("SkipThrow", value);
    }
    public void ShakeThrowDice(bool value)
    {
        GameSettings.Set("Shake", value);
    }
}
