using System;
using UnitedSolution;using UnityEngine;

namespace Mecury.Core.Unity.Utilities
{
    public class UnityLogger 
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

        private LogLevel logLevel = LogLevel.NONE;

        public void SetLevel(LogLevel level)
        {
            logLevel = level;
        }

        public void LogDebug(Func<string> text)
        {
            if ((logLevel & LogLevel.DEBUG) > 0)
            {
                Debug.Log("DEBUG ==== " + text());
            }
        }

        public void LogError(Func<string> text)
        {
            if ((logLevel & LogLevel.ERROR) > 0)
            {
                Debug.LogError("ERR ==== " + text());
            }
        }

        public void LogInfo(Func<string> text)
        {
            if ((logLevel & LogLevel.INFO) > 0)
            {
                Debug.Log("INFO ==== " + text());
            }
        }

        public void LogTrace(Func<string> text)
        {
            if ((logLevel & LogLevel.TRACE) > 0)
            {
                Debug.Log("TRACE ==== " + text());
            }
        }

        public void LogWarning(Func<string> text)
        {
            if ((logLevel & LogLevel.WARN) > 0)
            {
                Debug.LogWarning("WARN ==== " + text());
            }
        }
    }
}
