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

    [Header("Win Line Settings")]
    public Color winLineColor = Color.green;
    public float winLineWidth = 0.2f;
    public string winLineSortingLayerName = "UI";
    public int winLineSortingOrder = 10;
    public float winLineDrawDuration = 0.5f;
    public int winLinePositionCount = 2;

    [Header("NotifyGameLoadProgress Setting")]
    public float timeFakeLoading;
    public float timeHideCanvasGroup;
    public float timeDoAnimationText;

    [Header("String Intruction")]
    [TextArea(20,20)]
    public string instructionInfo;

    [Header("List Music")]
    public List<AudioClip> soundMusics;

    [Header("Color Alpha")]
    public Color colorAlphaOn = new(1f, 1f, 1f, 1f);
    public Color colorAlphaOff = new(1f, 1f, 1f, 0f);

    [Header("MinimaxAi Setting")]
    [Tooltip("Amount of turn that player and AI click at start game")]
    public int amountMove;
    public int radiusMove;
    public int maxCandidates;
    public int nearbyMove;
}
