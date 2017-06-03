using System;
using System.Collections.Generic;
using System.Linq;
using UnitedSolution;using UnityEngine;

namespace UnitedSolution
{ 
    public class HotkeyManager : MonoBehaviour
    {
        public static HotkeyManager Instance { get; private set; }

        public event Action OnAnykey_Down;

        /// <summary>
        /// Action bool:
        ///     - false: continue trigger
        ///     - true: means cancel following actions
        /// </summary>
        private List<KeyValuePair<KeyCode[], Action>> keydownEvents = new List<KeyValuePair<KeyCode[], Action>>();
        private List<KeyValuePair<KeyCode[], Action>> keyupEvents = new List<KeyValuePair<KeyCode[], Action>>();
        private List<KeyValuePair<KeyCode[], Action>> keyEvents = new List<KeyValuePair<KeyCode[], Action>>();

        private List<Action> exclusiveEvents = new List<Action>();

        private bool isMatched;

        /// <summary>
        /// Clear all events for : key down, key hold, key up
        /// </summary>
        public void ClearAll()
        {
            ClearKeyHold();
            ClearKeyDown();
            CleareKeyUp();
        }

        /// <summary>
        /// Clear all events for key down
        /// </summary>
        public void ClearKeyDown()
        {
            lock (keydownEvents)
            {
                keydownEvents.Clear();
            }
        }

        /// <summary>
        /// Clear all events for key up
        /// </summary>
        public void CleareKeyUp()
        {
            lock (keyupEvents)
            {
                keydownEvents.Clear();
            }
        }

        /// <summary>
        /// Clear all events for key hold
        /// </summary>
        public void ClearKeyHold()
        {
            lock (keyEvents)
            {
                keydownEvents.Clear();
            }
        }

        /// <summary>
        /// Clear all events for exclusive
        /// </summary>
        public void ClearExclusives()
        {
            lock(exclusiveEvents)
            {
                exclusiveEvents.Clear();
            }
        }

        /// <summary>
        /// Register down event with keys
        /// Can mark exclusive
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keyCodes"></param>
        public void RegisterKeydown(Action action, params KeyCode[] keyCodes)
        {
            keydownEvents.Add(new KeyValuePair<KeyCode[], Action>(keyCodes, action));
        }


        /// <summary>
        /// Register up event with keys
        /// Can mark exclusive
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keyCodes"></param>
        public void RegisterKeyup(Action action, params KeyCode[] keyCodes)
        {
            keyupEvents.Add(new KeyValuePair<KeyCode[], Action>(keyCodes, action));
        }


        /// <summary>
        /// Register hold event with keys
        /// Can mark exclusive
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keyCodes"></param>
        public void RegisterKeyhold(Action action, params KeyCode[] keyCodes)
        {
            keyEvents.Add(new KeyValuePair<KeyCode[], Action>(keyCodes, action));
        }


        /// <summary>
        /// After this event fired will cancel all other events
        /// </summary>
        /// <param name="action"></param>
        /// <param name="keyCodes"></param>
        public void MarkExclusive(Action action)
        {
            exclusiveEvents.Add(action);
        }

        public void ClearExclusive(Action action)
        {
            exclusiveEvents.Remove(action);
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        void Update()
        {
            // Any key down first first
            if (Input.anyKeyDown && OnAnykey_Down != null)
            {
                OnAnykey_Down();
                if (exclusiveEvents.Contains(OnAnykey_Down)) return;
            }

            // Key down
            lock (keydownEvents)
            {
                foreach (var keyEvent in keydownEvents)
                {
                    isMatched = true;
                    foreach (var key in keyEvent.Key)
                    {
                        if (!(isMatched = Input.GetKeyDown(key))) break;
                    }
                    if (isMatched)
                    {
                        keyEvent.Value();
                        if (exclusiveEvents.Contains(keyEvent.Value)) return;
                    }
                }
            }

            // Key hold
            lock (keyEvents)
            {
                foreach (var keyEvent in keyEvents)
                {
                    isMatched = true;
                    foreach (var key in keyEvent.Key)
                    {
                        if (!(isMatched = Input.GetKey(key))) break;
                    }
                    if (isMatched)
                    {
                        keyEvent.Value();
                        if (exclusiveEvents.Contains(keyEvent.Value)) return;
                    }
                }
            }

            // Key up
            lock (keyupEvents)
            {
                foreach (var keyEvent in keyupEvents)
                {
                    isMatched = true;
                    foreach (var key in keyEvent.Key)
                    {
                        if (!(isMatched = Input.GetKeyUp(key))) break;
                    }
                    if (isMatched)
                    {
                        keyEvent.Value();
                        if (exclusiveEvents.Contains(keyEvent.Value)) return;
                    }
                }
            }
        }
    }
}
