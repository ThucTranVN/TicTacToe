using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextDynamic : MonoBehaviour
{
    [SerializeField] private string localizationKey;
    [SerializeField] private string[] arguments;

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

    public void SetArguments(params string[] args)
    {
        arguments = args;
        UpdateTextAndFont();
    }

    private void UpdateTextAndFont()
    {
        if (text == null || LocalizationManager.Instance == null) return;

        string localized = LocalizationManager.Instance.GetLocalizedFormattedValue(localizationKey, arguments);

        Debug.Log($"[LocalizedTextDynamic] key = {localizationKey}, value = {localized}");

        text.text = localized;
        text.font = LocalizationManager.Instance.GetFontForCurrentLanguage();
    }
    public void UpdateTextManually()
    {
        UpdateTextAndFont();
    }
}
