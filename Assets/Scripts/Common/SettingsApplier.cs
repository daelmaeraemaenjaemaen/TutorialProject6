using UnityEngine;
public class SettingsApplier : MonoBehaviour
{
    void Awake()
    {
        PlayerSettings.Load();
        PlayerSettings.ApplyToRuntime();
    }
}