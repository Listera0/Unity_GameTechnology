using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

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

public class JsonTranslate : MonoBehaviour
{
    public GameObject languageDropBox;

    private TextMeshProUGUI[] targetObjects;
    private string[] targetTexts;
    private Dictionary<string, string> localizedText;

    void Awake()
    {
        if (!languageDropBox)
        {
            Debug.LogWarning("LanguageDropBox is Null");
        }
        
        targetObjects = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        targetTexts = new string[targetObjects.Length];

        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetTexts[i] = targetObjects[i].text;
        }

        SetLanguageInDropBox();
        languageDropBox.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(TranslateAllTexts);
    }

    public void SetLanguageInDropBox()
    {
        if (!languageDropBox) return;

        TMP_Dropdown dropdown = languageDropBox.GetComponent<TMP_Dropdown>();

        string path = Application.dataPath + "/../Language/";
        string[] filesName = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

        foreach (string name in filesName)
        {
            string fileName = Path.GetFileNameWithoutExtension(name);
            dropdown.options.Add(new TMP_Dropdown.OptionData(fileName));
        }

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == "English")
            {
                dropdown.value = i;
                dropdown.RefreshShownValue();
                break;
            }
        }
    }

    public void TranslateAllTexts(int index)
    {
        TMP_Dropdown dropdown = languageDropBox.GetComponent<TMP_Dropdown>();

        string language = dropdown.options[index].text;
        string path = Application.dataPath + "/../Language/" + language + ".json";

        if (!File.Exists(path))
        {
            Debug.LogError("No File: " + path);
            return;
        }

        localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));

        for (int i = 0; i < targetObjects.Length; i++)
        {
            bool findedFlag = false;
            foreach (var data in localizedText)
            {
                if (data.Key == targetTexts[i])
                {
                    targetObjects[i].text = data.Value;
                    findedFlag = true;
                    break;
                }
            }

            if (!findedFlag)
            {
                targetObjects[i].text = targetTexts[i];
            }
        }
        
        dropdown.value = index;
        dropdown.RefreshShownValue();
    }
}
