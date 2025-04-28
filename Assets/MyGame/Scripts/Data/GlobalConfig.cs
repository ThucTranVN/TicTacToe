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
    public List<Sprite> boardSprites;

    [Header("Prefabs")]
    public GameObject BoardViewItemPrefab;
}
