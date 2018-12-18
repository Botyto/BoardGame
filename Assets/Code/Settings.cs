using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle skipThrow;
    public Toggle shake;

    public void OnEnable()
    {
        // TODO: Extract it somewhere that will be executed only on first launch
        if (!PlayerPrefs.HasKey("SkipThrow")) PlayerPrefs.SetInt("SkipThrow", 1);
        if (!PlayerPrefs.HasKey("Shake")) PlayerPrefs.SetInt("Shake", 1);

        this.skipThrow.isOn = (PlayerPrefs.GetInt("SkipThrow") != 0);
        this.shake.isOn = (PlayerPrefs.GetInt("Shake") != 0);
    }

    public void SkipDiceThrow(bool value)
    {
        PlayerPrefs.SetInt("SkipThrow", Convert.ToInt32(value));
    }
    public void ShakeThrowDice(bool value)
    {
        PlayerPrefs.SetInt("Shake", Convert.ToInt32(value));
    }


}

