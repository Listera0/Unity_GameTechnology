using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TextConnectCategory
{
    Default = 0,
    Slider,
    TMP_Dropdown
}

public class TextValueChanger : MonoBehaviour
{
    public TextConnectCategory category;
    public GameObject connectTarget;
    private TextMeshProUGUI textMeshPro;
    private Text textTmp;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textTmp = GetComponent<Text>();
        ConnectWithTarget();
    }

    public void ConnectWithTarget()
    {
        switch (category)
        {
            case TextConnectCategory.Slider:
                ChangeTextValue(connectTarget.GetComponent<Slider>().value);
                connectTarget.GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeTextValue(value));
                break;
        }
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
