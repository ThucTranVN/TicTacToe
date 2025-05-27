using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections;

public class PopupSelectLanguage : BasePopup
{
    [Header("Language Buttons")]
    [SerializeField] private Button m_EnglishButton;
    [SerializeField] private Button m_VietnameseButton;

    private void Start()
    {
        m_EnglishButton.onClick.AddListener(OnClickEnglishButton);
        m_VietnameseButton.onClick.AddListener(OnClickVietnameseButton);
    }

    private void OnClickEnglishButton()
    {
        StartCoroutine(SetLocale("en"));
    }

    private void OnClickVietnameseButton()
    {
        StartCoroutine(SetLocale("vi"));
    }

    private IEnumerator SetLocale(string localeCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;

        for (int i = 0; i < locales.Count; i++)
        {
            if (locales[i].Identifier.Code == localeCode)
            {
                LocalizationSettings.SelectedLocale = locales[i];
                break;
            }
        }
        if (LocalizationSettings.SelectedLocale != null)
        {
            yield return null;
            this.Hide();
        }
        else
        {
            Debug.LogWarning($"Không tìm th?y ngôn ng? '{localeCode}'!");
        }
    }
}
