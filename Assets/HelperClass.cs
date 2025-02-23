using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public static class HelperClass
{
    #region Editor Specific Functions
    public static void DebugMessage(object message)
    {
        #if UNITY_EDITOR
        Debug.Log(message);
        #endif
    }

    public static void DebugWarning(object message)
    {
    #if UNITY_EDITOR
        Debug.LogWarning(message);
    #endif
    }

    public static void DebugError(object message)
    {
    #if UNITY_EDITOR
        Debug.LogError(message);
    #endif
    }

    public static void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }
    #endregion
    #region Non-Editor Specific Functions
    public static string GetActionName(Action functionToConvert)
    {
        return functionToConvert.Method.Name;
    }
    #endregion
}