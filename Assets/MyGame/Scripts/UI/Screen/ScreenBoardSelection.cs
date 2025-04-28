using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ScreenBoardSelection : BaseScreen
{
    [Header("UI References")]
    public Transform contentParent;
    public ScrollRect scrollRect;
    public Button leftButton;
    public Button rightButton;
    public Button pvpButton;
    public Button pveButton;

    [Header("Config")]
    public GlobalConfig globalConfig;

    private List<BoardViewItem> boardViewItems = new List<BoardViewItem>();
    private List<BoardType> boardTypes = new List<BoardType>();
    private BoardType currentBoardType = BoardType.Size3x3;

    private float[] itemPositions;
    private float targetPos;
    private float smoothSpeed = 5f;
    private bool isDragging = false;
    private int currentIndex = 0;
    private bool snapChanged = false;

    public override void Init()
    {
        base.Init();
        InitBoardTypes();
        InitListBoard(currentBoardType);
        InitButtons();
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
        Array enumValues = System.Enum.GetValues(typeof(BoardType));
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
        if (globalConfig == null)
        {
            Debug.LogError("GlobalConfig không được gán trong ScreenBoardSelection.");
            return;
        }

        GameObject prefab = globalConfig.BoardViewItemPrefab;
        if (prefab == null)
        {
            Debug.LogError("Không tìm thấy prefab 'BoardViewItem' trong GlobalConfig.");
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
                boardViewItems.Add(item);
            }
            else
            {
                Debug.LogError("Prefab không có component BoardViewItem!");
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());

        itemPositions = new float[boardTypes.Count];
        for (int i = 0; i < boardTypes.Count; i++)
        {
            itemPositions[i] = (float)i / (boardTypes.Count - 1);
        }

        int startIndex = boardTypes.IndexOf(selectedType);
        SetIndex(startIndex, false);
    }

    private void SetIndex(int index, bool smooth = true)
    {
        if (index < 0 || index >= boardTypes.Count) return;

        currentIndex = index;
        targetPos = itemPositions[index];

        if (smooth)
        {
            DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                       x => scrollRect.horizontalNormalizedPosition = x,
                       targetPos, 0.5f); 
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = targetPos;
        }

        UpdateButtonVisibility(index);
        UpdateSelectedBoard(index);
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

    private new void Update()
    {
        if (boardTypes == null || boardTypes.Count == 0) return;

        if (Input.GetMouseButton(0))
        {
            isDragging = true;
            targetPos = scrollRect.horizontalNormalizedPosition;
        }
        else
        {
            if (isDragging)
            {
                isDragging = false;
                float nearestPos = FindNearest(targetPos);
                int nearestIndex = FindNearestIndex(targetPos);
                if (nearestIndex != currentIndex)
                {
                    snapChanged = true;
                }
                SetIndex(nearestIndex);
            }

            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPos, Time.deltaTime * smoothSpeed);
        }

        ScaleItems();
    }

    private float FindNearest(float currentPos)
    {
        float closest = itemPositions[0];
        float minDist = Mathf.Abs(currentPos - closest);

        for (int i = 1; i < itemPositions.Length; i++)
        {
            float dist = Mathf.Abs(currentPos - itemPositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = itemPositions[i];
            }
        }

        return closest;
    }

    private int FindNearestIndex(float currentPos)
    {
        int nearestIndex = 0;
        float minDist = Mathf.Abs(currentPos - itemPositions[0]);

        for (int i = 1; i < itemPositions.Length; i++)
        {
            float dist = Mathf.Abs(currentPos - itemPositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private void ScaleItems()
    {
        for (int i = 0; i < boardViewItems.Count; i++)
        {
            float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - itemPositions[i]);
            float scale = Mathf.Clamp(1f - distance * 2f, 0.8f, 1f);

            boardViewItems[i].transform.localScale = Vector3.Lerp(boardViewItems[i].transform.localScale, new Vector3(scale, scale, 1f), Time.deltaTime * 10f);
        }
    }
}
