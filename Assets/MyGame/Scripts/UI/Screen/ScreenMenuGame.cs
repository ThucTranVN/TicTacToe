using UnityEngine;
using UnityEngine.UI;

public class ScreenMenuGame : BaseScreen
{
    [Header("Button List")]
    [SerializeField] private Button m_PlayBtn;
    [SerializeField] private Button m_MenuBtn;
    [SerializeField] private Button m_InfoBtn;

    private void Start()
    {
        m_PlayBtn.onClick.AddListener(OnClickPlayButton);
        m_MenuBtn.onClick.AddListener(OnClickMenuButton);
        m_InfoBtn.onClick.AddListener(OnClickInfoButton);
    }

    private void OnClickPlayButton()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenBoardSelection>();
            this.Hide();
        }    
    }    
    private void OnClickMenuButton()
    {
       if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingGame>();
        }
    }
    private void OnClickInfoButton()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupInstruction>();
        }    
    }    
}
