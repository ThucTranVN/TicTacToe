using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CSVLocalizationImporter : EditorWindow
{
    private TextAsset csvFile;
    private GlobalConfig globalConfig;

    [MenuItem("Tools/Import Localization CSV")]
    public static void ShowWindow()
    {
        GetWindow<CSVLocalizationImporter>("Import Localization CSV");
    }

    private void OnGUI()
    {
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
        globalConfig = (GlobalConfig)EditorGUILayout.ObjectField("GlobalConfig", globalConfig, typeof(GlobalConfig), false);

        if (GUILayout.Button("Import") && csvFile != null && globalConfig != null)
        {
            ImportCSV();
        }
    }

    private void ImportCSV()
    {
        var lines = csvFile.text.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        var header = lines[0].Split(',').Select(h => h.Trim()).ToList();

        List<LocalizationEntry> entries = new();

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',').Select(v => v.Trim()).ToList();
            if (values.Count != header.Count) continue;

            var entry = new LocalizationEntry
            {
                Key = values[0],
                EN = values.Count > 1 ? values[1] : "",
                VN = values.Count > 2 ? values[2] : ""
            };

            entries.Add(entry);
        }

        globalConfig.localizationEntries = entries;
        EditorUtility.SetDirty(globalConfig);
        AssetDatabase.SaveAssets();

        Debug.Log("?ã import localization CSV vào GlobalConfig.");
    }
}