using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScreenHome : BaseScreen
{
    public GameObject m_MainPanel;
    public GameObject m_MultiplayPanel;

    private void OnEnable()
    {
        m_MultiplayPanel.SetActive(false);
    }

    public override void Show(object data)
    {
        base.Show(data);
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnClickPopupSetting()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSetting>();
        }
    }

    public void StartGame()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowNotify<NotifyLoadingGame>();
        }
        this.Hide();
    }

    public void OpenMultiplayPanel()
    {
        m_MultiplayPanel.SetActive(true);
        m_MainPanel.SetActive(false);
    }

    public void ExitMultiplayPanel()
    {
        m_MultiplayPanel.SetActive(false);
        m_MainPanel.SetActive(true);
    }
}
