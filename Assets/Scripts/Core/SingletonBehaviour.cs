using System;
using System.Collections.Generic;
using UnitedSolution;
using UnityEngine;

namespace UnitedSolution
{
    public class PhotonSingleton<T> : Photon.PunBehaviour where T : Photon.PunBehaviour
    {
        static T _instance;
        static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            Debug.Log("An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = GetComponent<T>(); // the same as instance = this;
            }
            else
            {
                DestroyImmediate(gameObject); //this means the "Start" method will not be called.
            }
        }

        void Enable()
        {

        }

        void Disable()
        {

        }

        Dictionary<Event, Action> eventHandlers = new Dictionary<Event, Action>();



        static bool applicationIsQuitting = false;

        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }

    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance;
        static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (Application.isPlaying && applicationIsQuitting)
                {
                    Debug.LogWarning("Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            if (Application.isPlaying)
                            {
                                DontDestroyOnLoad(singleton);
                            }

                            Debug.Log("An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(this);
                }
                _instance = GetComponent<T>(); // the same as instance = this;
            }
            else
            {
                DestroyImmediate(gameObject); //this means the "Start" method will not be called.
            }
        }

        void Enable()
        {

        }

        void Disable()
        {

        }

        Dictionary<Event, Action> eventHandlers = new Dictionary<Event, Action>();

        static bool applicationIsQuitting = false;

        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}
