using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectionPanel : BaseScreen
{
    public Transform contentParent;
    public ScrollRect scrollRect;
    public Button leftButton;
    public Button rightButton;
    public Button pvpButton;
    public Button pveButton;

    private List<BoardViewItem> boardViewItems = new List<BoardViewItem>();
    private List<BoardType> boardTypes = new List<BoardType>();
    private BoardType currentBoardType = BoardType.Size3x3; 

    private void Start()
    {
        InitBoardTypes();
        InitListBoard(currentBoardType);
        OnClickButton();
    }

    public override void Init()
    {
        base.Init();
    }

    private void OnClickButton()
    {
        leftButton.onClick.AddListener(() => MoveBoard(Direction.Left));
        rightButton.onClick.AddListener(() => MoveBoard(Direction.Right));
        pvpButton.onClick.AddListener(() => EnterGameMode(GameMode.PVP));
        pveButton.onClick.AddListener(() => EnterGameMode(GameMode.PVE));
    }    
    private void InitBoardTypes()
    {
        boardTypes.Clear();
        foreach (BoardType type in System.Enum.GetValues(typeof(BoardType)))
        {
            if (type != BoardType.Default)
                boardTypes.Add(type);
        }
    }

    private void InitListBoard(BoardType type)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        GameObject prefab = Resources.Load<GameObject>("Prefabs/BoardViewItem");
        if (prefab == null)
        {
            Debug.LogError("Không tìm thấy prefab 'BoardViewItem' trong Resources/Prefabs/");
            return;
        }
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }

    private void MoveBoard(Direction direction)
    {
        if (boardViewItems.Count == 0) return;

        switch (direction)
        {
            case Direction.Left:
                currentBoardType = (BoardType)Mathf.Max((int)BoardType.Size3x3, (int)currentBoardType - 1);
                break;
            case Direction.Right:
                currentBoardType = (BoardType)Mathf.Min((int)BoardType.Size11x11, (int)currentBoardType + 1);
                break;
        }

        UpdateView();
    }

    private void UpdateView()
    {
        if (boardViewItems.Count > 0)
        {
            BoardViewItem item = boardViewItems[0]; 
            item.Init(currentBoardType); 
        }
    }

    private void EnterGameMode(GameMode mode)
    {
        Debug.Log($"[EnterGameMode] Game Mode: {mode}, Board Type: {currentBoardType}");
    }
}
