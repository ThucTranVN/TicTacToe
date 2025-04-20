using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    public static List<GameObject> Call(params IEnumerator[] coroutines)
    {
        List<GameObject> goCoroutines = new List<GameObject>();

        if (coroutines == null || coroutines.Length == 0)
        {
            return goCoroutines;
        }

        for (int i = 0; i < coroutines.Length; i++)
        {            
            CoroutineHelper view = CoroutineHelperPooling.Instance.GetCoroutineHelperFromPool();
            view.gameObject.SetActive(true);
            view.Do(coroutines[i]);
            goCoroutines.Add(view.gameObject);
        }

        return goCoroutines;
    }

    private void Do(IEnumerator coroutine)
    {
        StartCoroutine(Wait(coroutine));
    }

    private IEnumerator Wait(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
        gameObject.SetActive(false);
        CoroutineHelperPooling.Instance.ReturnToPool(this);
    }
}