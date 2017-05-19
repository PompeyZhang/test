using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> where T :class ,new()
{
    protected static T _Instance = null;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new T();
            return _Instance;
        }
    }

    protected Singleton()
    {
        Init();
    }

    public virtual void Init()
    {
        
    }
}


public abstract class SingletonMono<T> : MonoBehaviour where T :SingletonMono<T>
{
    protected static T _Instance = null;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject go = GameObject.Find(typeof(T).Name);
                if (go == null)
                {
                    go = new GameObject();
                    _Instance = go.AddComponent<T>();
                }
                else
                {
                    if (go.GetComponent<T>() == null)
                    {
                        go.AddComponent<T>();
                    }
                    _Instance = go.GetComponent<T>();
                }

                DontDestroyOnLoad(go);
            }
            return _Instance;
        }
    }
}
