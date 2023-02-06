using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError(typeof(T).ToString() + " is NULL");
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = (T)this;

        Initialize();
    }

    public virtual void Initialize()
    {

    }
}
