using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Firis
{
    public abstract class UI
    {
        public GameObject gameObject { get; set; }

        public abstract GameObject Load();

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }

        public virtual void OnShow() { }
        public virtual void OnUpdate() { }
    }

    public class UIComponent : Component, IAwake, IUpdate
    {
        public GameObject gameObject { get; set; }
        public Transform cache { get; set; }

        private Stack<UI> uiStack { get; set; } = new Stack<UI>();
        private Dictionary<Type, UI> uiDict { get; set; } = new Dictionary<Type, UI>();
        private Dictionary<Type, UI> uiCache { get; set; } = new Dictionary<Type, UI>();

        public static UIComponent Instance { get; set; }

        public void Awake()
        {
            gameObject = GameObject.Find("UIComponent");
            cache = gameObject.transform.Find("Cache");
            GameObject.DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public void Update()
        {
            if (uiStack.Count == 0) return;
            uiStack.Peek().OnUpdate();
        }

        public T Push<T>() where T : UI, new()
        {
            if (uiStack.Count > 0)
            {
                UI pre_ui = uiStack.Peek();
                pre_ui.OnPause();
            }

            if (uiDict.ContainsKey(typeof(T)))
            {
                Log.Error("Push Panel 已存在");
                return null;
            }

            T ui = Load<T>();
            uiStack.Push(ui);

            return ui;
        }

        public void Pop(bool clear = false)
        {
            if (uiStack.Count <= 0)
            {
                return;
            }

            UI cur_ui = uiStack.Pop();
            cur_ui.OnExit();

            Type cur = cur_ui.GetType();

            if (uiDict[cur].gameObject != null)
            {
                if (!clear)
                {
                    uiCache.Add(cur, uiDict[cur]);
                    uiCache[cur].gameObject.transform.SetParent(cache);
                }
                else
                {
                    GameObject.Destroy(uiDict[cur].gameObject);
                }
            }

            uiDict.Remove(cur);

            if (uiStack.Count > 0)
            {
                UI ui = uiStack.Peek();
                ui.OnResume();
            }
        }

        public T Load<T>() where T : UI, new()
        {
            UI ui;
            GameObject go;

            Type cur = typeof(T);
            if (uiCache.ContainsKey(cur))
            {
                ui = uiCache[cur];
                go = uiCache[cur].gameObject;
                go.transform.SetParent(gameObject.transform);
                uiCache.Remove(cur);
                ui.OnShow();
            }
            else
            {
                ui = new T();
                GameObject uiLoad = ui.Load();
                go = GameObject.Instantiate<GameObject>(uiLoad, gameObject.transform);
                go.name = typeof(T).Name;
                ui.gameObject = go;
                ui.OnEnter();
                ui.OnShow();
            }

            uiDict.Add(typeof(T), ui);

            return uiDict[typeof(T)] as T;
        }

        public T Get<T>() where T : UI, new()
        {
            if (uiDict.ContainsKey(typeof(T)))
            {
                return uiDict[typeof(T)] as T;
            }
            return null;
        }

        public void Clear()
        {
            if (uiStack == null) return;
            if (uiStack.Count == 0) return;

            foreach (var ui in uiStack)
            {
                ui.OnExit();
            }

            uiStack.Clear();
            uiDict.Clear();
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();
            Clear();
            GameObject.Destroy(gameObject);
        }
    }
}
