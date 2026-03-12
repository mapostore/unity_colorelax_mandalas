using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Unity6AndroidBuildStability
{
    private const string GradlePropertiesPath = "Assets/Plugins/Android/gradleTemplate.properties";

    [MenuItem("Tools/Fix/Apply Android Build Stability (Unity 6)")]
    public static void Apply()
    {
        bool changed = false;
        changed |= SetAndroidSdkPins();
        changed |= NormalizeGradleProperties();

        if (changed)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Android build stability settings applied.");
        }
        else
        {
            Debug.Log("Android build stability settings already up-to-date.");
        }
    }

    private static bool SetAndroidSdkPins()
    {
        bool changed = false;

        // Keep AGP 8.7.2 + Gradle 8.11 on a tested compile/target level for this Unity setup.
        changed |= SetAndroidSdkLevelProperty("targetSdkVersion", "AndroidApiLevel35");
        changed |= SetAndroidSdkLevelProperty("minSdkVersion", "AndroidApiLevel24");

        try
        {
            var current = PlayerSettings.Android.targetArchitectures;
            var arm64 = AndroidArchitecture.ARM64;
            if ((current & arm64) == 0)
            {
                PlayerSettings.Android.targetArchitectures = current | arm64;
                changed = true;
            }
        }
        catch
        {
            // Ignore if API changes in future Unity versions.
        }

        return changed;
    }

    private static bool SetAndroidSdkLevelProperty(string propertyName, string enumName)
    {
        var androidType = typeof(PlayerSettings.Android);
        var prop = androidType.GetProperty(propertyName);
        if (prop == null || !prop.CanWrite)
            return false;

        Type enumType = prop.PropertyType;
        if (!enumType.IsEnum)
            return false;

        object targetEnumValue;
        try
        {
            targetEnumValue = Enum.Parse(enumType, enumName);
        }
        catch
        {
            return false;
        }

        try
        {
            object current = prop.GetValue(null, null);
            if (!Equals(current, targetEnumValue))
            {
                prop.SetValue(null, targetEnumValue, null);
                return true;
            }
        }
        catch
        {
            // Ignore API incompatibility.
        }

        return false;
    }

    private static bool NormalizeGradleProperties()
    {
        if (!File.Exists(GradlePropertiesPath))
            return false;

        string[] lines = File.ReadAllLines(GradlePropertiesPath);
        bool changed = false;

        changed |= Upsert(ref lines, "android.useAndroidX", "true");
        changed |= Upsert(ref lines, "android.enableJetifier", "true");
        changed |= Upsert(ref lines, "android.suppressUnsupportedCompileSdk", "35");

        if (changed)
        {
            File.WriteAllLines(GradlePropertiesPath, lines);
        }

        return changed;
    }

    private static bool Upsert(ref string[] lines, string key, string value)
    {
        string prefix = key + "=";
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(prefix, StringComparison.Ordinal))
            {
                string desired = prefix + value;
                if (!string.Equals(lines[i], desired, StringComparison.Ordinal))
                {
                    lines[i] = desired;
                    return true;
                }
                return false;
            }
        }

        Array.Resize(ref lines, lines.Length + 1);
        lines[lines.Length - 1] = prefix + value;
        return true;
    }
}
