using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        LocalizationManager.OnLanguageChanged += UpdateTextAndFont;
    }

    private void Start()
    {
        UpdateTextAndFont();
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChanged -= UpdateTextAndFont;
    }

    private void UpdateTextAndFont()
    {
        if (text == null || LocalizationManager.Instance == null) return;

        // Set localized text
        string value = LocalizationManager.Instance.GetLocalizedValue(localizationKey);
        text.text = value;

        // Set correct font per language
        var currentLang = LocalizationManager.Instance.GetCurrentLanguage();
        text.font = currentLang switch
        {
            Language.English => DataManager.Instance.GlobalConfig.englishFont,
            Language.Vietnamese => DataManager.Instance.GlobalConfig.vietnameseFont,
            _ => DataManager.Instance.GlobalConfig.englishFont 
        };

        Debug.Log($"[LocalizedText] Updated '{localizationKey}' with text: '{value}' and font: '{text.font.name}'");
    }
}