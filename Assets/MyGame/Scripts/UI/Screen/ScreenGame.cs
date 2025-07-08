using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGame : BaseScreen
{
    [SerializeField] private TMP_Dropdown difficultDropdown;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button settingBtn;
    
    private List<AIDepthLevel> depthOptions;

    private void Start()
    {
        backBtn.onClick.AddListener(OnClickBackButton);
        settingBtn.onClick.AddListener(OnClickSettingButton);
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
                            if(canvasGroup != null)
                            {
                                canvasGroup.alpha = 0f; // Hide groupdown when in PVP mode
                                canvasGroup.interactable = false;
                            }
                        }
                        break;
                    case GameMode.PVE:
                        {
                            InitDropdown();
                            if (canvasGroup != null)
                            {
                                canvasGroup.alpha = 1f; // Show dropdown when in PVE mode
                                canvasGroup.interactable = true;
                            }
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
            string title = "SETTING";
            UIManager.Instance.ShowPopup<PopupSettingGame>(title);
        }

    }
}
