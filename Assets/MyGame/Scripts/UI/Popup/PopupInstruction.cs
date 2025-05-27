using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInstruction : BasePopup
{
    [Header("Parameters")]
    [SerializeField] private Button m_ExitButton;
    [SerializeField] private TextMeshProUGUI m_ContentTxt;
    [SerializeField] private GlobalConfig globalConfig;
    [SerializeField] private RectTransform m_ContentRect;

    private void Start()
    {   
        m_ContentTxt.text = globalConfig.instructionInfo;
        m_ExitButton.onClick.AddListener(OnClickExitButton);
    }
    private void OnClickExitButton()
    {
        this.Hide();
    }
    public override void Show(object data)
    {
        base.Show(data);

        StartCoroutine(ReadSizeNextFrame());
    }
    private IEnumerator ReadSizeNextFrame()
    {
        // Chờ đến cuối frame mới lấy được sizedelta.y ContentSizeFitter đã set layout xong
        yield return null;
        float hDelta = m_ContentRect.sizeDelta.y;
        Vector2 startPos = m_ContentRect.anchoredPosition;
        startPos.y = -(hDelta/2);
        m_ContentRect.anchoredPosition = startPos;
    }


}
