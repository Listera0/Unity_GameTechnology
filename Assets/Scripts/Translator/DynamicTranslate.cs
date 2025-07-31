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
        jsonTranslate = JsonTranslate.instance;
        ownTextMesh = transform.GetComponent<TextMeshProUGUI>();
    }

    public void ChangeText(string text)
    {
        ownTextMesh.text = text;
        jsonTranslate.TranslateText(ownTextMesh);
    }
}
