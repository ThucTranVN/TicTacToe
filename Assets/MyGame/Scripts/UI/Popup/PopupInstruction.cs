using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupInstruction : BasePopup
{
    [Header("Parameters")]
    [SerializeField] private Button m_ExitButton;
    [SerializeField] private TextMeshProUGUI m_ContentTxt;
    [SerializeField] private GlobalConfig globalConfig;

    private void Start()
    {
        m_ContentTxt.text = globalConfig.instructionInfo;
        m_ExitButton.onClick.AddListener(OnClickExitButton);
    }
    private void OnClickExitButton()
    {
        this.Hide();
    }


}
