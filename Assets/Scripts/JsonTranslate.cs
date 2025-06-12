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
    private TextMeshProUGUI[] targetObjects;
    private string[] targetTexts;
    private Dictionary<string, string> localizedText;

    void Awake()
    {
        targetObjects = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        targetTexts = new string[targetObjects.Length];

        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetTexts[i] = targetObjects[i].text;
        }

        TranslateAllTexts();
    }

    public void TranslateAllTexts()
    {
        LoadTranslateData("English");

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
    }

    public void LoadTranslateData(string fileName)
    {
        string path = Application.dataPath + "/../Language/" + fileName + ".json";

        if (!File.Exists(path))
        {
            Debug.LogError("No File: " + path);
            return;
        }
        else
        { 
            Debug.Log(path + " is Loaded!");
        }

        localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
    }
}
