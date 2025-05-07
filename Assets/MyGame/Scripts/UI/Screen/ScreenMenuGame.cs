using UnityEngine;
using UnityEngine.UI;

public class ScreenMenuGame : BaseScreen
{
    [SerializeField] private Button m_PlayBtn;

    private void Start()
    {
        m_PlayBtn.onClick.AddListener(OnClickPlayButton);
    }

    private void OnClickPlayButton()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenBoardSelection>();
        }    
    }    
}
