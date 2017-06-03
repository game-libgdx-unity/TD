using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{
    public struct Setting<T>
    {
        public Action<T> Setter;
        public Func<T> Getter;
        public Setting(Func<T> Getter, Action<T> Setter)
        {
            this.Getter = Getter;
            this.Setter = Setter;
        }
    }



    public class GameSetting : SingletonBehaviour<GameSetting>
    {
        Dictionary<string, Setting<string>> stringPrefs = new Dictionary<string, Setting<string>>();
        Dictionary<string, Setting<int>> intPrefs = new Dictionary<string, Setting<int>>();
        Dictionary<string, Setting<bool>> boolPrefs = new Dictionary<string, Setting<bool>>();
        Dictionary<string, Setting<float>> floatPrefs = new Dictionary<string, Setting<float>>();

        public static void Register(string key, Func<string> getter, Action<string> setter) { Instance._Register(key, getter, setter); }
        private void _Register(string key, Func<string> getter, Action<string> setter)
        {
            stringPrefs.Add(key, new Setting<string>(getter, setter));
            Set<string>(key, getter());
        }

        public static void Register(string key, Func<int> getter, Action<int> setter) { Instance._Register(key, getter, setter); }
        private void _Register(string key, Func<int> getter, Action<int> setter)
        {
            intPrefs.Add(key, new Setting<int>(getter, setter));
            Set<int>(key, getter());

        }

        public static void Register(string key, Func<float> getter, Action<float> setter) { Instance._Register(key, getter, setter); }
        private void _Register(string key, Func<float> getter, Action<float> setter)
        {
            floatPrefs.Add(key, new Setting<float>(getter, setter));
        }

        public static void Register(string key, Func<bool> getter, Action<bool> setter) { Instance._Register(key, getter, setter); }
        private void _Register(string key, Func<bool> getter, Action<bool> setter)
        {
            boolPrefs.Add(key, new Setting<bool>(getter, setter));
        }

        protected override void Awake()
        {
            base.Awake();

            Load();
        }

        void OnDestroy()
        {
            Save();
        }

        public void Load()
        {
            LoadDictionary<string>(stringPrefs);
            LoadDictionary<int>(intPrefs);
            LoadDictionary<float>(floatPrefs);
            LoadDictionary<bool>(boolPrefs);
        }

        private void LoadDictionary<T>(Dictionary<string, Setting<T>> dic)
        {
            foreach (string key in stringPrefs.Keys)
            {
                T data = Get<T>(key);
                dic[key].Setter(data);
            }
        }

        public void Save()
        {
            SaveDictionary<string>(stringPrefs);
            SaveDictionary<int>(intPrefs);
            SaveDictionary<float>(floatPrefs);
            SaveDictionary<bool>(boolPrefs);
            PlayerPrefs.Save();
        }

        private void SaveDictionary<T>(Dictionary<string, Setting<T>> dic)
        {
            foreach (string key in stringPrefs.Keys)
            {
                T data = dic[key].Getter();
                Set<T>(key, data);
            }
        }

        public void DeleteAll()
        {
            stringPrefs.Clear();
            boolPrefs.Clear();
            intPrefs.Clear();
            floatPrefs.Clear();
            PlayerPrefs.DeleteAll();
        }
        public void DeleteKey(string key)
        {
            stringPrefs.Remove(key);
            boolPrefs.Remove(key);
            intPrefs.Remove(key);
            floatPrefs.Remove(key);
            PlayerPrefs.DeleteKey(key);
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public void Set(string key, object value)
        {
            Set<object>(key, value);
        }

        public T Get<T>(string key)
        {
            object value = null;
            Type type = typeof(T);
            if (type == typeof(int))
            {
                value = GetInt(key);
            }
            else if (type == typeof(float))
            {
                value = GetFloat(key);
            }
            else if (type == typeof(bool))
            {
                value = GetBool(key);
            }
            else if (type == typeof(string))
            {
                value = GetString(key);
            }
            return (T)value;
        }

        public void Set<T>(string key, T valueT)
        {
            Type type = typeof(T);
            if (type == typeof(int))
            {
                string valueS = valueT.ToString();
                SetInt(key, int.Parse(valueS));
            }
            else if (type == typeof(float))
            {
                string valueS = valueT.ToString();
                SetFloat(key, float.Parse(valueS));
            }
            else if (type == typeof(bool))
            {
                string valueS = valueT.ToString();
                SetBool(key, bool.Parse(valueS));
            }
            else if (type == typeof(string))
            {
                string valueS = valueT.ToString();
                SetString(key, valueS);
            }
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key) == 1;
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}
