using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

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

public class JsonTranslate : Singleton<JsonTranslate>, IInitializeInter
{
    public GameObject languageDropBox;

    private List<TextMeshProUGUI> targetObjects;
    private List<string> targetTexts;
    private Dictionary<string, string> localizedText;
    private bool translated;

    public void Initialize()
    {
        if (!languageDropBox)
        {
            Debug.LogWarning("LanguageDropBox is Null");
        }

        SetLanguageInDropBox();
        languageDropBox.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(TranslateAllTexts);

        targetObjects = new List<TextMeshProUGUI>();
        targetTexts = new List<string>();
        FindAllTextMeshProUGUI();
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

    public void FindAllTextMeshProUGUI()
    {
        if (targetObjects.Count != 0)
        {
            int index = 0;
            foreach (TextMeshProUGUI textMesh in targetObjects)
            {
                textMesh.text = targetTexts[index];
                index++;
            }
        }

        targetObjects = new List<TextMeshProUGUI>();
        targetTexts = new List<string>();
        targetObjects = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Where(t => t.gameObject.scene.IsValid() && t.gameObject.hideFlags == HideFlags.None).ToList();

        foreach (TextMeshProUGUI textMesh in targetObjects)
        {
            targetTexts.Add(textMesh.text);
        }
    }

    public void TranslateAllTexts(int index)
    {
        FindAllTextMeshProUGUI();
        TMP_Dropdown dropdown = languageDropBox.GetComponent<TMP_Dropdown>();

        string language = dropdown.options[index].text;
        string path = Application.dataPath + "/../Language/" + language + ".json";

        if (!File.Exists(path))
        {
            Debug.LogError("No File: " + path);
            return;
        }
        
        translated = true;
        localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));

        for (int i = 0; i < targetObjects.Count; i++)
        {
            targetObjects[i].text = FindTranslateText(targetTexts[i]);
        }

        dropdown.value = index;
        dropdown.RefreshShownValue();
    }

    public void TranslateText(TextMeshProUGUI textMesh)
    {
        textMesh.text = FindTranslateText(textMesh.text);
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
}
