using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;

public class ScreenBoardSelection : BaseScreen
{
    [Header("UI References")]
    public Transform contentParent;
    public ScrollRect scrollRect;
    public Button leftButton;
    public Button rightButton;
    public Button pvpButton;
    public Button pveButton;

    private List<BoardViewItem> boardViewItems = new List<BoardViewItem>();
    private List<BoardType> boardTypes = new List<BoardType>();
    private BoardType currentBoardType;

    private int currentIndex = 0;
    private bool isDragging = false;

    public override void Init()
    {
        base.Init();
        currentBoardType = DataManager.Instance.GlobalConfig.defaultBoardType;
        InitBoardTypes();
        InitListBoard(currentBoardType);
        InitButtons();
        InitScrollEvents();
    }

    private void InitButtons()
    {
        leftButton.onClick.AddListener(() => MoveBoard(Direction.Left));
        rightButton.onClick.AddListener(() => MoveBoard(Direction.Right));
        pvpButton.onClick.AddListener(() => EnterGameMode(GameMode.PVP));
        pveButton.onClick.AddListener(() => EnterGameMode(GameMode.PVE));
    }

    private void InitBoardTypes()
    {
        Array enumValues = Enum.GetValues(typeof(BoardType));
        for (int i = 1; i < enumValues.Length; i++)
        {
            BoardType type = (BoardType)enumValues.GetValue(i);
            if (type != BoardType.Unknown)
            {
                boardTypes.Add(type);
            }
        }
    }

    private void InitListBoard(BoardType selectedType)
    {
        if (DataManager.Instance.GlobalConfig == null)
        {
            Debug.Log("GlobalConfig không được gán trong DataManager.");
            return;
        }

        GameObject prefab = DataManager.Instance.GlobalConfig.BoardViewItemPrefab;
        if (prefab == null)
        {
            Debug.Log("Không tìm thấy prefab 'BoardViewItem' trong GlobalConfig.");
            return;
        }

        for (int i = 0; i < boardTypes.Count; i++)
        {
            BoardType type = boardTypes[i];
            GameObject itemGO = Instantiate(prefab, contentParent);
            itemGO.name = $"Board {type}";
            BoardViewItem item = itemGO.GetComponent<BoardViewItem>();

            if (item != null)
            {
                item.Init(type);
                item.NormalizedPosition = (float)i / (boardTypes.Count - 1);
                boardViewItems.Add(item);
            }
            else
            {
                Debug.LogWarning("Prefab không có component BoardViewItem!");
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        int startIndex = boardTypes.IndexOf(selectedType);
        SetIndex(startIndex, false);
    }

    private void SetIndex(int index, bool smooth = true)
    {
        if (index < 0 || index >= boardTypes.Count) return;

        currentIndex = index;
        float targetPos = boardViewItems[index].NormalizedPosition;
        float duration = DataManager.Instance.GlobalConfig.scrollTweenDuration;

        if (smooth)
        {
            scrollRect.DOHorizontalNormalizedPos(targetPos, duration)
                      .SetEase(Ease.OutQuad)
                      .OnComplete(() => UpdateSelectedBoard(index));
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = targetPos;
            UpdateSelectedBoard(index);
        }

        UpdateButtonVisibility(index);
    }

    private void MoveBoard(Direction direction)
    {
        if (direction == Direction.Left && currentIndex > 0)
            currentIndex--;
        else if (direction == Direction.Right && currentIndex < boardTypes.Count - 1)
            currentIndex++;

        SetIndex(currentIndex);
    }

    private void UpdateSelectedBoard(int index)
    {
        if (index < 0 || index >= boardTypes.Count) return;

        currentBoardType = boardTypes[index];

        for (int i = 0; i < boardViewItems.Count; i++)
        {
            boardViewItems[i].SetSelected(i == index);
        }

        Debug.Log($"[Selection Changed] Đang chọn board: {currentBoardType}");
    }

    private void EnterGameMode(GameMode mode)
    {
        Debug.Log($"[EnterGameMode] Game Mode: {mode}, Board Type: {currentBoardType}");
        this.Hide();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.StartGame(mode, currentBoardType);
        }
    }

    private void UpdateButtonVisibility(int index)
    {
        leftButton.gameObject.SetActive(index > 0);
        rightButton.gameObject.SetActive(index < boardTypes.Count - 1);
    }

    private void InitScrollEvents()
    {
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

        EventTrigger trigger = scrollRect.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = scrollRect.gameObject.AddComponent<EventTrigger>();

        var entryBegin = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
        entryBegin.callback.AddListener((data) => { isDragging = true; });
        trigger.triggers.Add(entryBegin);

        var entryEnd = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
        entryEnd.callback.AddListener((data) =>
        {
            isDragging = false;
            SnapToClosestBoard();
        });
        trigger.triggers.Add(entryEnd);
    }

    private void OnScrollValueChanged(Vector2 value)
    {
        ScaleItems();
    }

    private void SnapToClosestBoard()
    {
        int closestIndex = FindClosestBoardIndex();
        if (closestIndex != -1)
        {
            SetIndex(closestIndex, smooth: true);
        }
    }

    private void ScaleItems()
    {
        for (int i = 0; i < boardViewItems.Count; i++)
        {
            boardViewItems[i].UpdateScale(scrollRect.horizontalNormalizedPosition);
        }
    }

    private int FindClosestBoardIndex()
    {
        float currentPos = scrollRect.horizontalNormalizedPosition;
        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < boardViewItems.Count; i++)
        {
            float distance = Mathf.Abs(boardViewItems[i].NormalizedPosition - currentPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
