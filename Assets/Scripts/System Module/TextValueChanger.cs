using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextValueChanger : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private Text textTmp;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textTmp = GetComponent<Text>();
    }

    public void ChangeTextValue(float value)
    {
        if (textMeshPro)
        {
            textMeshPro.SetText(((int)value).ToString());
            return;
        }

        if (textTmp)
        {
            textTmp.text = ((int)value).ToString();
            return;
        }
    }
}
