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

    [Header("BoardViewItem Settings")]
    public float scaleTweenDuration = 0.3f;
    public float scaleSelected = 1.1f;
    public float scaleNormal = 1f;
    public float scaleMin = 0.8f;
    public float scaleFactor = 2f;

    [Header("BoardViewItem Colors")]
    public Color imageColorSelected = Color.white;
    public Color imageColorUnselected = new Color(1f, 1f, 1f, 0.6f);
    public Color labelColorSelected = Color.yellow;
    public Color labelColorUnselected = Color.gray;

    [Header("Scroll Settings")]
    public float scrollTweenDuration = 0.35f;
}
