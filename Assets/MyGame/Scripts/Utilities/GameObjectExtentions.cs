using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtentions
{

    /// <summary>
    /// This is shorcut to GetComponent if gameobject already attached it or automatic adding new one before get if gameobject not attached it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        if (go?.GetComponent<T>() ?? false)
            return go?.GetComponent<T>() ?? null;
        else
            return go?.AddComponent<T>() ?? null;
    }

}
