using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GlobalConfig : ScriptableObject
{
    [Header("UI")]
    public float LoadingOverlapTime = 1f;
    public BoardType defaultBoardType = BoardType.Size3x3;

    [Header("Board Images")]
    public Sprite spriteSize3x3;
    public Sprite spriteSize6x6;
    public Sprite spriteSize9x9;
    public Sprite spriteSize11x11;

    [Header("Prefabs")]
    public GameObject BoardViewItemPrefab;
}
