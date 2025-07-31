using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Unity.VisualScripting;

[System.Serializable]
public class TranslateItem
{
    public string target;
    public string value;
}

[System.Serializable]
public class TranslateData
{
    public List<TranslateItem> items;

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> reuturnValue = new Dictionary<string, string>();

        foreach (TranslateItem item in items)
        {
            reuturnValue.Add(item.target, item.value);
        }

        return reuturnValue;
    }
}

public enum PathCategory
{
    Custom = 0,
    DataPath,
    StreamingAssetPath,
    PersistentDataPath,
    TemporaryCachePath,
    ConsoleLogPath
}

public class JsonTranslate : Singleton<JsonTranslate>, IInitializeInter
{
    public PathCategory pathCategory;
    public string pathLocation;

    public bool useLanguageDropBox;
    public TMP_Dropdown languageDropBox;
    public string selectLanguage;

    private FontAssistant fontAssistant;
    private Dictionary<TextMeshProUGUI, string> staticTranslateObjs;
    private Dictionary<TextMeshProUGUI, string> dynamicTranslateObjs;
    private Dictionary<string, string> localizedText;
    private List<string> languageList;
    private bool translated;

    public void Initialize()
    {
        fontAssistant = transform.AddComponent<FontAssistant>();
        CheckJsonTranslate();
    }

    public void CheckJsonTranslate()
    {
        staticTranslateObjs = new Dictionary<TextMeshProUGUI, string>();
        dynamicTranslateObjs = new Dictionary<TextMeshProUGUI, string>();
        localizedText = new Dictionary<string, string>();

        GetLanguageList();
        FindAllTextMeshProUGUI();

        if (useLanguageDropBox)
        {
            if (languageDropBox)
            {
                SetLanguageInDropBox();
                languageDropBox.onValueChanged.AddListener(TranslateAllTexts);
            }
            else
            {
                Debug.LogWarning("Language DropBox is Null");
            }
        }
        else
        {
            TranslateAllTexts(0);
        }
    }

    public void GetLanguageList()
    {
        languageList = new List<string>();
        string[] filesName = Directory.GetFiles(CombinePath() + pathLocation, "*.json", SearchOption.AllDirectories);

        foreach (string name in filesName)
        {
            string fileName = Path.GetFileNameWithoutExtension(name);
            languageList.Add(fileName);
        }
    }

    public void SetLanguageInDropBox()
    {
        if (!languageDropBox) return;

        foreach (string name in languageList)
        {
            languageDropBox.options.Add(new TMP_Dropdown.OptionData(name));
        }

        for (int i = 0; i < languageDropBox.options.Count; i++)
        {
            if (languageDropBox.options[i].text == "English")
            {
                languageDropBox.value = i;
                languageDropBox.RefreshShownValue();
                break;
            }
        }
    }

    public void TranslateAllTexts(int index)
    {
        if (useLanguageDropBox)
        {
            languageDropBox.value = index;
            selectLanguage = languageDropBox.options[index].text;
        }

        GetTranslateText(selectLanguage);
        fontAssistant.FontChangeAssistant(CombinePath() + pathLocation);
        TranslateAllStaticText();
        TranslateAllDynamicText();

        if (useLanguageDropBox)
        {
            languageDropBox.GetComponent<TMP_Dropdown>().RefreshShownValue();
            TranslateText(languageDropBox.transform.Find("Label").GetComponent<TextMeshProUGUI>());
        }
    }

    public void FindAllTextMeshProUGUI()
    {
        // already translated text return origin
        if (staticTranslateObjs.Count != 0)
        {
            foreach (KeyValuePair<TextMeshProUGUI, string> pair in staticTranslateObjs)
            {
                if(pair.Key != null)
                    pair.Key.text = pair.Value;
            }
        }

        staticTranslateObjs = new Dictionary<TextMeshProUGUI, string>();
        List<TextMeshProUGUI> targetObjects = new List<TextMeshProUGUI>();
        targetObjects = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Where(T => T.gameObject.scene.IsValid() && T.gameObject.hideFlags == HideFlags.None).ToList();

        foreach (TextMeshProUGUI textMesh in targetObjects)
        {
            staticTranslateObjs.Add(textMesh, textMesh.text);
        }
    }

    public void GetTranslateText(string language)
    {
        if (languageList.Contains(language))
        {
            // Application.dataPath + "/../Language/"
            string path = CombinePath() + pathLocation + language + ".json";

            if (!File.Exists(path))
            {
                Debug.LogWarningFormat("No File [{0}] ", path);
                return;
            }
            translated = true;
            localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
        }
        else
        {
            Debug.LogWarningFormat("No language File [{0}]", language);
        }
    }

    public void TranslateAllStaticText()
    {
        foreach (KeyValuePair<TextMeshProUGUI, string> pair in staticTranslateObjs)
        {
            if (pair.Key != null)
            {
                pair.Key.text = FindTranslateText(pair.Value);
                fontAssistant.FontChange(pair.Key);
            }  
        }
    }

    public void TranslateAllDynamicText()
    {
        foreach (KeyValuePair<TextMeshProUGUI, string> pair in dynamicTranslateObjs)
        {
            if (pair.Key != null)
            {
                pair.Key.text = FindTranslateText(pair.Value);
                fontAssistant.FontChange(pair.Key);
            }
        }
    }

    public void TranslateText(TextMeshProUGUI textMesh)
    {
        if (dynamicTranslateObjs.ContainsKey(textMesh))
            dynamicTranslateObjs[textMesh] = textMesh.text;
        else
            dynamicTranslateObjs.Add(textMesh, textMesh.text);
        
        textMesh.text = FindTranslateText(textMesh.text);
        fontAssistant.FontChange(textMesh);
    }

    private string FindTranslateText(string text)
    {
        if (translated)
        {
            foreach (var data in localizedText)
            {
                if (data.Key == text)
                {
                    return data.Value;
                }
            }
        }
        return text;
    }

    private string CombinePath()
    {
        string path = "";
        switch (pathCategory)
        {
            case PathCategory.Custom:
                break;
            case PathCategory.DataPath:
                path = Application.dataPath;
                break;
            case PathCategory.StreamingAssetPath:
                path = Application.streamingAssetsPath;
                break;
            case PathCategory.PersistentDataPath:
                path = Application.persistentDataPath;
                break;
            case PathCategory.TemporaryCachePath:
                path = Application.temporaryCachePath;
                break;
            case PathCategory.ConsoleLogPath:
                path = Application.consoleLogPath;
                break;
        }
        return path;
    }
}
