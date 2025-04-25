using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectionPanel : BaseScreen
{
    [Header("UI References")]
    public Transform contentParent;
    public ScrollRect scrollRect;
    public Button leftButton;
    public Button rightButton;
    public Button pvpButton;
    public Button pveButton;

    [Header("Snap Scroll")]
    public SnapScroll snapScroll;

    private List<BoardViewItem> boardViewItems = new List<BoardViewItem>();
    private List<BoardType> boardTypes = new List<BoardType>();
    private BoardType currentBoardType = BoardType.Size3x3;

    private void Start()
    {
        InitBoardTypes();
        InitListBoard(currentBoardType);
        OnClickButton();

        // Gán callback khi SnapScroll đổi index
        snapScroll.onSnapChanged = OnSnapIndexChanged;
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
            if (type != BoardType.Unknown)
                boardTypes.Add(type);
        }
    }

    private void InitListBoard(BoardType selectedType)
    {
        boardViewItems.Clear();

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

        foreach (BoardType type in boardTypes)
        {
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

        int index = boardTypes.IndexOf(selectedType);
        snapScroll.SetIndex(index); // Đặt index ban đầu
        OnSnapIndexChanged(index); // Cập nhật UI ban đầu
    }

    private void MoveBoard(Direction direction)
    {
        int currentIndex = boardTypes.IndexOf(currentBoardType);

        if (direction == Direction.Left && currentIndex > 0)
            currentIndex--;
        else if (direction == Direction.Right && currentIndex < boardTypes.Count - 1)
            currentIndex++;

        currentBoardType = boardTypes[currentIndex];

        snapScroll.SetIndex(currentIndex);
        OnSnapIndexChanged(currentIndex);
    }

    private void OnSnapIndexChanged(int index)
    {
        if (index < 0 || index >= boardTypes.Count) return;

        currentBoardType = boardTypes[index];

        for (int i = 0; i < boardViewItems.Count; i++)
        {
            boardViewItems[i].SetSelected(i == index);
        }

        Debug.Log($"[SnapScroll] Đang chọn board: {currentBoardType}");
    }

    private void EnterGameMode(GameMode mode)
    {
        Debug.Log($"[EnterGameMode] Game Mode: {mode}, Board Type: {currentBoardType}");
        this.Hide();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.StartGame(mode, currentBoardType);
        }
        // Tùy theo logic game, có thể gọi:
        // GameManager.Instance.StartGame(mode, currentBoardType);
        // hoặc SceneManager.LoadScene(...);
    }
}
