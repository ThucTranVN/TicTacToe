using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGame : BaseScreen
{
    [SerializeField] private TMP_Dropdown difficultDropdown;
    [SerializeField] private CanvasGroup cvgdifficultDropdown;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button hintBtn;
    [SerializeField] private Button undoBtn;
    
    private List<AIDepthLevel> depthOptions;

    private void Start()
    {
        InitButton();
    }
    public override void Show(object data)
    {
        base.Show(data);
        if(data !=null)
        {
            if (data is GameMode gameMode)
            {
                CanvasGroup canvasGroup = difficultDropdown.GetComponentInParent<CanvasGroup>();
                switch (gameMode)
                {
                    case GameMode.PVP:
                        {
                            ShowUIDifficult(false); // Hide dropdown when in PVP mode
                        }
                        break;
                    case GameMode.PVE:
                        {
                            InitDropdown();
                            ShowUIDifficult(true); // Show dropdown when in PVE mode
                        }
                        break;
                }
            }
        }
      
      

    }
    public override void Init()
    {
        base.Init();
    }
    private void ShowUIDifficult(bool isShow)
    {
        if (cvgdifficultDropdown != null)
        {
            cvgdifficultDropdown.alpha = isShow ? 1f : 0f;
            cvgdifficultDropdown.interactable = isShow;
            cvgdifficultDropdown.blocksRaycasts = isShow;
        }
    }
    private void InitButton()
    {
        backBtn.onClick.AddListener(OnClickBackButton);
        settingBtn.onClick.AddListener(OnClickSettingButton);
        undoBtn.onClick.AddListener(OnClickUndoButton);
    }

    private void InitDropdown()
    {
        if (difficultDropdown != null)
        {
            depthOptions = Enum.GetValues(typeof(AIDepthLevel))
                      .Cast<AIDepthLevel>()
                      .Skip(1)
                      .ToList();

            List<string> optionNames = depthOptions.Select(e => e.ToString()).ToList();
            difficultDropdown.ClearOptions();
            difficultDropdown.AddOptions(optionNames);
        }
        difficultDropdown.onValueChanged.AddListener(OnClickSelectDifficule);
    }
    private void OnClickSelectDifficule(int index)
    {
        AIDepthLevel selected = depthOptions[index];
        Debug.Log($"Đã chọn: {selected} - Giá trị : {(int)selected}");
        if (GameManager.HasInstance)
        {
            GameManager.Instance.SetAIDepthLevel(selected);
        }
    }
    private void OnClickBackButton()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenBoardSelection>();
        }
        if(BoardController.HasInstance)
        {
            BoardController.Instance.ResetBoard(); // Reset board state if needed
        }    
        this.Hide();
    }
    private void OnClickSettingButton()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingGame>();
        }

    }
    private void OnClickUndoButton()
    {
        if (BoardController.HasInstance)
        {
            BoardController.Instance.UndoMove();
        }
    }
}
