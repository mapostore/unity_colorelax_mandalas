using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CompilerDefineSanitizer
{
    [MenuItem("Tools/Fix/Normalize Compiler Defines")]
    public static void NormalizeNow()
    {
        bool changed = false;
        changed |= NormalizeLegacyBuildTargetGroups();
        changed |= NormalizeNamedBuildTargets();

        if (changed)
        {
            AssetDatabase.SaveAssets();
            Debug.Log("CompilerDefineSanitizer: normalized invalid define/compiler-arg entries.");
        }
        else
        {
            Debug.Log("CompilerDefineSanitizer: no invalid define/compiler-arg entries found.");
        }
    }

    private static bool NormalizeLegacyBuildTargetGroups()
    {
        bool changed = false;
        foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
        {
            if (group == BuildTargetGroup.Unknown) continue;

            try
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                string normalized = NormalizeDefineString(symbols);
                if (!string.Equals(symbols, normalized, StringComparison.Ordinal))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, normalized);
                    changed = true;
                }
            }
            catch
            {
                // Ignore unsupported/deprecated groups on this Unity version.
            }
        }

        return changed;
    }

    private static bool NormalizeNamedBuildTargets()
    {
        bool changed = false;

        Assembly editorAssembly = typeof(PlayerSettings).Assembly;
        Type namedBuildTargetType = editorAssembly.GetType("UnityEditor.Build.NamedBuildTarget");
        if (namedBuildTargetType == null) return false;

        MethodInfo getDefines = typeof(PlayerSettings).GetMethod("GetScriptingDefineSymbols",
            BindingFlags.Public | BindingFlags.Static, null, new[] { namedBuildTargetType }, null);
        MethodInfo setDefines = typeof(PlayerSettings).GetMethod("SetScriptingDefineSymbols",
            BindingFlags.Public | BindingFlags.Static, null, new[] { namedBuildTargetType, typeof(string) }, null);

        MethodInfo getArgs = typeof(PlayerSettings).GetMethod("GetAdditionalCompilerArguments",
            BindingFlags.Public | BindingFlags.Static, null, new[] { namedBuildTargetType }, null);
        MethodInfo setArgs = typeof(PlayerSettings).GetMethod("SetAdditionalCompilerArguments",
            BindingFlags.Public | BindingFlags.Static, null, new[] { namedBuildTargetType, typeof(string[]) }, null);

        if (getDefines == null || setDefines == null) return false;

        var targets = new List<object>();
        foreach (PropertyInfo p in namedBuildTargetType.GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            if (p.PropertyType == namedBuildTargetType)
            {
                object value = p.GetValue(null, null);
                if (value != null) targets.Add(value);
            }
        }

        foreach (object target in targets)
        {
            try
            {
                string symbols = (string)getDefines.Invoke(null, new[] { target });
                string normalized = NormalizeDefineString(symbols);
                if (!string.Equals(symbols, normalized, StringComparison.Ordinal))
                {
                    setDefines.Invoke(null, new[] { target, normalized });
                    changed = true;
                }

                if (getArgs != null && setArgs != null)
                {
                    string[] args = (string[])getArgs.Invoke(null, new[] { target }) ?? Array.Empty<string>();
                    string[] normalizedArgs = args
                        .Where(a => !string.IsNullOrWhiteSpace(a))
                        .Where(a => !string.Equals(a.Trim(), "-define:", StringComparison.Ordinal))
                        .ToArray();

                    if (!args.SequenceEqual(normalizedArgs))
                    {
                        setArgs.Invoke(null, new object[] { target, normalizedArgs });
                        changed = true;
                    }
                }
            }
            catch
            {
                // Ignore targets unsupported by this project.
            }
        }

        return changed;
    }

    private static string NormalizeDefineString(string symbols)
    {
        if (symbols == null) return string.Empty;
        string[] parts = symbols.Split(new[] { ';' }, StringSplitOptions.None);
        IEnumerable<string> cleaned = parts
            .Select(s => (s ?? string.Empty).Trim())
            .Where(s => s.Length > 0);
        return string.Join(";", cleaned);
    }
}
