using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GlobalConfig : ScriptableObject
{
    [Header("UI")]
    public float LoadingOverlapTime = 1f;
}
