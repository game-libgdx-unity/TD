using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace UnitedSolution
{
    public enum LogLevel
    {
        NONE = 0,
        DEBUG = 1,
        INFO = 2,
        TRACE = 4,
        WARN = 8,
        ERROR = 16,
        ALL = DEBUG | INFO | TRACE | WARN | ERROR
    }
    public enum LogDetails
    {
        NONE = 0,
        FLAG = 1,
        GAME_OBJECT = 2,
        SCRIPT_NAME = 4,
        FULL_SCRIPT_NAME = 8,
        ALL = FLAG | GAME_OBJECT | SCRIPT_NAME | FULL_SCRIPT_NAME
    }
    [System.Serializable]
    public class DebugFlag
    {
        /// <summary>
        /// Store for Name property
        /// </summary>
        public Object gameObject;
        /// <summary>
        /// Store for Value property
        /// </summary>
        public bool allowLogging;
        /// <summary>
        /// Flag name
        /// </summary>
        public string Name { get { return gameObject.name; } }
        /// <summary>
        /// Flag value
        /// </summary>
        public bool Value { get { return allowLogging; } }
    }
    public class SmartLogger : SingletonBehaviour<SmartLogger>
    {
        public List<DebugFlag> debugFlags = new List<DebugFlag>();

        public LogLevel logLevel = LogLevel.NONE;
        public LogDetails logDetails = LogDetails.NONE;

        public void SetLevel(LogLevel level)
        {
            logLevel = level;
        }

        public void SetLogDetails(LogDetails level)
        {
            logDetails = level;
        }

        public static void Initialize(LogLevel level, LogDetails details)
        {
            Instance.logLevel = level;
            Instance.logDetails = details;
        }

        private void DoLog(MonoBehaviour script, object text, System.Action<object> logAction, string label)
        {
            string className = script.GetType().ToString();
            if (logDetails == LogDetails.NONE)
            {
                logAction(text);
            }
            else if (logDetails == LogDetails.ALL)
            {
                logAction(label + script.gameObject.name + " --- " + className + ": " + text);
            }
            else if ((logDetails & LogDetails.FLAG) > 0)
            {
                logAction(text);
            }
            else if ((logDetails & LogDetails.GAME_OBJECT) > 0)
            {
                logAction(label + script.gameObject.name + " --- " + text);
            }
            else if ((logDetails & LogDetails.SCRIPT_NAME) > 0)
            {
                logAction(label + className.Substring(className.LastIndexOf('.') + 1) + " --- " + text);
            }
            else if ((logDetails & LogDetails.FULL_SCRIPT_NAME) > 0)
            {
                logAction(label + className + " --- " + text);
            }
        }

        public void LogDebug(MonoBehaviour script, object text)
        {
#if UNITY_EDITOR
            if ((logLevel & LogLevel.DEBUG) > 0)
            {
                if (Validate(script))
                {
                    DoLog(script, text, obj => Debug.Log(obj), "DEBUG --- ");
                }
            }
#endif
        }

        public void LogError(MonoBehaviour script, object text)
        {
#if UNITY_EDITOR
            if ((logLevel & LogLevel.ERROR) > 0)
            {
                if (Validate(script))
                {
                    DoLog(script, text, obj => Debug.LogError(obj), "ERR --- ");
                }
            }
#endif
        }

        public void LogInfo(MonoBehaviour script, object text)
        {
#if UNITY_EDITOR
            if ((logLevel & LogLevel.INFO) > 0)
            {
                if (Validate(script))
                {
                    DoLog(script, text, obj => Debug.Log(obj), "INFO --- ");
                }
            }
#endif
        }

        public void LogTrace(MonoBehaviour script, object text)
        {
#if UNITY_EDITOR
            if ((logLevel & LogLevel.TRACE) > 0)
            {
                if (Validate(script))
                {
                    DoLog(script, text, obj => Debug.Log(obj), "TRACE --- ");
                }
            }
#endif
        }

        public void LogWarning(MonoBehaviour script, object text)
        {
#if UNITY_EDITOR
            if ((logLevel & LogLevel.WARN) > 0)
            {
                if (Validate(script))
                {
                    DoLog(script, text, obj => Debug.LogWarning(obj), "WARN --- ");
                }
            }
#endif
        }

        private bool Validate(MonoBehaviour obj)
        {
            if (debugFlags.Count == 0)
            {
                return true;
            }
            foreach (DebugFlag flag in debugFlags)
            {
                if (flag.gameObject == obj.gameObject && flag.Value)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static class Logger
    {
        /// <summary>
        /// Logs a message to the Unity Console if the program is running from the Unity Editor and the object is validated to log.
        /// </summary>
        /// <param name="message">Message to log to the console</param>
        public static void debug(this MonoBehaviour mono, object message)
        {
            SmartLogger.Instance.LogDebug(mono, message);
        }

        public static void info(this MonoBehaviour mono, object message)
        {
            SmartLogger.Instance.LogInfo(mono, message);
        }

        public static void trace(this MonoBehaviour mono, object message)
        {
            SmartLogger.Instance.LogTrace(mono, message);
        }

        public static void warn(this MonoBehaviour mono, object message)
        {
            SmartLogger.Instance.LogWarning(mono, message);
        }

        public static void error(this MonoBehaviour mono, object message)
        {
            SmartLogger.Instance.LogError(mono, message);
        }

        public static void debug(object message)
        {
            SmartLogger.Instance.LogDebug(SmartLogger.Instance, message);
        }

        public static void info(object message)
        {
            SmartLogger.Instance.LogInfo(SmartLogger.Instance, message);
        }

        public static void trace(object message)
        {
            SmartLogger.Instance.LogTrace(SmartLogger.Instance, message);
        }

        public static void warn(object message)
        {
            SmartLogger.Instance.LogWarning(SmartLogger.Instance, message);
        }

        public static void error(object message)
        {
            SmartLogger.Instance.LogError(SmartLogger.Instance, message);
        }

        /// <summary>
        /// Logs a message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="message">Message to log to the console</param>
        /// <param name="context">Object to which the message applies</param>
        public static void Log(DebugFlag debugFlag, object message, Object context)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.Log("[DebugFlag: " + debugFlag.Name + "] " + message, context);
            }
#endif
        }

        /// <summary>
        /// Logs a formatted message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">Format arguments</param>
        public static void logFormat(DebugFlag debugFlag, string format, params object[] args)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.LogFormat("[DebugFlag: " + debugFlag.Name + "] " + format, args);
            }
#endif
        }

        /// <summary>
        /// Logs a formatted message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="context">Object to which the message applies</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">Format arguments</param>
        public static void LogFormat(DebugFlag debugFlag, Object context, string format, params object[] args)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.LogFormat(context, "[DebugFlag: " + debugFlag.Name + "] " + format, args);
            }
#endif
        }

        /// <summary>
        /// Logs a warning message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="message">Message to log to the console</param>
        public static void LogWarning(DebugFlag debugFlag, object message)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.LogWarning("[DebugFlag: " + debugFlag.Name + "] " + message);
            }
#endif
        }

        /// <summary>
        /// Logs a warning message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="message">Message to log to the console</param>
        /// <param name="context">Object to which the message applies</param>
        public static void LogWarning(DebugFlag debugFlag, object message, Object context)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.LogWarning("[DebugFlag: " + debugFlag.Name + "] " + message, context);
            }
#endif
        }

        /// <summary>
        /// Logs a formatted warning message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">Format arguments</param>
        public static void LogWarningFormat(DebugFlag debugFlag, string format, params object[] args)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.LogWarningFormat("[DebugFlag: " + debugFlag.Name + "] " + format, args);
            }
#endif
        }

        /// <summary>
        /// Logs a formatted warning message to the Unity Console if the program is running from the Unity Editor and the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="context">Object to which the message applies</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">Format arguments</param>
        public static void LogWarningFormat(DebugFlag debugFlag, Object context, string format, params object[] args)
        {
#if UNITY_EDITOR
            if (debugFlag.Value)
            {
                Debug.LogWarningFormat(context, "[DebugFlag: " + debugFlag.Name + "] " + format, args);
            }
#endif
        }

        /// <summary>
        /// Logs an error message to the Unity Console if the program is running from the Unity Editor
        /// and either AlwaysLogErrors is true or the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="message">Message to log to the console</param>
        public static void LogError(DebugFlag debugFlag, object message)
        {
            Debug.LogError("[DebugFlag: " + debugFlag.Name + "] " + message);
        }

        /// <summary>
        /// Logs an error message to the Unity Console if the program is running from the Unity Editor
        /// and either AlwaysLogErrors is true or the value of debugFlag is true..
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="message">Message to log to the console</param>
        /// <param name="context">Object to which the message applies</param>
        public static void LogError(DebugFlag debugFlag, object message, Object context)
        {
            Debug.LogError("[DebugFlag: " + debugFlag.Name + "] " + message, context);
        }

        /// <summary>
        /// Logs a formatted error message to the Unity Console if the program is running from the Unity Editor
        /// and either AlwaysLogErrors is true or the value of debugFlag is true.
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">Format arguments</param>
        public static void LogErrorFormat(DebugFlag debugFlag, string format, params object[] args)
        {
            Debug.LogErrorFormat("[DebugFlag: " + debugFlag.Name + "] " + format, args);
        }

        /// <summary>
        /// Logs a formatted error message to the Unity Console if the program is running from the Unity Editor
        /// and either AlwaysLogErrors is true or the value of debugFlag is true..
        /// </summary>
        /// <param name="debugFlag">Flag for whether the message should be logged to the console</param>
        /// <param name="context">Object to which the message applies</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">Format arguments</param>
        public static void LogErrorFormat(DebugFlag debugFlag, Object context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, "[DebugFlag: " + debugFlag.Name + "] " + format, args);
        }
    }
}