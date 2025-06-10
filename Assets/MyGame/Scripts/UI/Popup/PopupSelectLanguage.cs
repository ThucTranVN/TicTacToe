using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupSelectLanguage : BasePopup
{
    [Header("Language Buttons")]
    [SerializeField] private Button m_EnglishButton;
    [SerializeField] private Button m_VietnameseButton;

    private void Start()
    {
        m_EnglishButton.onClick.AddListener(() => SelectLanguage(Language.English));
        m_VietnameseButton.onClick.AddListener(() => SelectLanguage(Language.Vietnamese));
    }
    private void SelectLanguage(Language lang)
    {
        LocalizationManager.Instance.SetLanguage(lang);
        gameObject.SetActive(false);
    }
}
