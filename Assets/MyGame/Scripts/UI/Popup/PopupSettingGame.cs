using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupSettingGame : BasePopup
{
    [Header("Button List")]
    [SerializeField] private Button m_ExitButton;
    [SerializeField] private Button m_ShopButton;
    [SerializeField] private Button m_MusicButton;
    [SerializeField] private Button m_LanguageButton;
    [SerializeField] private Toggle m_BgmToggle;
    [SerializeField] private Toggle m_SfxToggle;
    [SerializeField] private Toggle m_VibrationToggle;

    [Header("Text List")]
    [SerializeField] private TextMeshProUGUI m_TitleText;

    private void Start()
    {
        m_ExitButton.onClick.AddListener(OnClickExitButton);
        m_MusicButton.onClick.AddListener(OnClickMusicButton);
    }
    public override void Show(object data)
    {
        base.Show(data);
        if(data !=null)
        {
            if (data is string title)
            {
                m_TitleText.text = title;
            }
        } 
            
    }

    private void OnClickExitButton()
    {
        this.Hide();
    }
    private void OnClickMusicButton()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingMusic>();
        }    
    }    
}
