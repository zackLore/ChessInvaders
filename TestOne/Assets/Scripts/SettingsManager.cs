using UnityEngine;
using System.Collections;

public static class SettingsManager
{
    public static readonly string VOLUME = "volume";

    public static float GetVolume()
    {
        if (PlayerPrefs.HasKey(VOLUME))
        {
            return PlayerPrefs.GetFloat(VOLUME);
        }
        return 0.0f;
    }
    public static void SetVolume(float volume)
    {
        if (volume >= 0 && volume <= 100)
        {
            PlayerPrefs.SetFloat(VOLUME, volume);
        }
        else if (volume < 0)
        {
            PlayerPrefs.SetFloat(VOLUME, 0);
        }
        else if (volume > 100)
        {
            PlayerPrefs.SetFloat(VOLUME, 100);
        }
    }

}
