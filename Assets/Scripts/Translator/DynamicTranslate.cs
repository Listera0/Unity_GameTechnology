using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicTranslate : MonoBehaviour
{
    public JsonTranslate jsonTranslate;
    public TextMeshProUGUI ownTextMesh;

    void Start()
    {
        InitDynamicTranslate();
    }

    public void InitDynamicTranslate()
    {
        if (jsonTranslate == null) jsonTranslate = JsonTranslate.instance;
        if (ownTextMesh == null) ownTextMesh = transform.GetComponent<TextMeshProUGUI>();
    }

    public void ChangeText(string text)
    {
        ownTextMesh.text = text;
        jsonTranslate.TranslateText(ownTextMesh);
    }

    public void InitAndChange(string text)
    {
        InitDynamicTranslate();
        ChangeText(text);
    }
}
