using UnityEngine;
using UnityEngine.UI;

public class ScreenMenuGame : BaseScreen
{
    [Header("Button List")]
    [SerializeField] private Button m_PlayBtn;
    [SerializeField] private Button m_MenuBtn;

    private void Start()
    {
        m_PlayBtn.onClick.AddListener(OnClickPlayButton);
        m_MenuBtn.onClick.AddListener(OnClickMenuBtn);
    }

    private void OnClickPlayButton()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenBoardSelection>();
            this.Hide();
        }    
    }    
    private void OnClickMenuBtn()
    {
       if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingGame>();
        }
    }
}
