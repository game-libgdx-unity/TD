using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class UnityExtendedShortKeys : ScriptableObject
{
    [MenuItem("HotKey/Run _F5")]
   public  static void PlayGame()
    {
        if (!EditorApplication.isPlaying)
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    [MenuItem("HotKey/Clear console _F4")]
   public static void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
}