using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Dictionary<string, string> localizedMap = new();
    private Language currentLanguage = Language.English;
    public static event System.Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentLanguage = (Language)PlayerPrefs.GetInt("Language", (int)Language.English);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => DataManager.HasInstance && DataManager.Instance.GlobalConfig != null);

        SetLanguage(currentLanguage);
    }

    public void SetLanguage(Language lang)
    {
        currentLanguage = lang;
        PlayerPrefs.SetInt("Language", (int)lang);
        PlayerPrefs.Save();

        // Update custom key-value mapping
        localizedMap.Clear();
        foreach (var entry in DataManager.Instance.GlobalConfig.localizationEntries)
        {
            string value = lang switch
            {
                Language.English => entry.EN,
                Language.Vietnamese => entry.VN,
                _ => entry.EN
            };
            localizedMap[entry.Key] = value;
        }

        // Update Unity Localization System
        StartCoroutine(SetUnityLocale(lang));
        OnLanguageChanged?.Invoke();
    }

    private IEnumerator SetUnityLocale(Language lang)
    {
        yield return LocalizationSettings.InitializationOperation;

        string localeCode = lang switch
        {
            Language.English => "en",
            Language.Vietnamese => "vi",
            _ => "en"
        };

        var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (selectedLocale != null)
            LocalizationSettings.SelectedLocale = selectedLocale;
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedMap.TryGetValue(key, out var value))
        {
            return UnescapeRichText(value);
        }
        return $"[Key:{key}]";
    }

    public Language GetCurrentLanguage() => currentLanguage;

    public string GetLocalizedFormattedValue(string key, params object[] args)
    {
        string raw = GetLocalizedValue(key);
        if (string.IsNullOrEmpty(raw)) return key;
        return string.Format(raw, args);
    }
    public TMP_FontAsset GetFontForCurrentLanguage()
    {
        return currentLanguage switch
        {
            Language.English => DataManager.Instance.GlobalConfig.englishFont,
            Language.Vietnamese => DataManager.Instance.GlobalConfig.vietnameseFont,
            _ => DataManager.Instance.GlobalConfig.englishFont
        };
    }

    public static string UnescapeRichText(string input)
    {
        return input
            .Replace("\\n", "\n")
            .Replace("\\t", "\t")
            .Replace("\\\"", "\"")
            .Replace("\\\\", "\\");
    }
}
