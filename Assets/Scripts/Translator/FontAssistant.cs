using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FontAssistant : MonoBehaviour
{
    private bool changed;
    private TextMeshProUGUI fontAssistantText;
    private TMP_FontAsset changeFont;
    private float fontChangeValue;

    public void FontChange(TextMeshProUGUI mesh)
    {
        if (changed == false) return;

        mesh.font = changeFont;
        mesh.fontSize *= fontChangeValue;
    }

    public void FontChangeAssistant(string path)
    {
        if (fontAssistantText == null)
        {
            fontAssistantText = transform.AddComponent<TextMeshProUGUI>();
            fontAssistantText.text = "Ag";
        }

        changeFont = GetFontFromPath(path);
        if (changeFont == null)
        {
            changed = false;
            changeFont = fontAssistantText.font;
        }
        else
        {
            fontChangeValue = FontSizeAssistant(changeFont);
        }
    }

    public float FontSizeAssistant(TMP_FontAsset changeFont)
    {
        if (changeFont == null) return 0.0f;

        fontAssistantText.ForceMeshUpdate();
        float beforeSize = fontAssistantText.preferredHeight;

        fontAssistantText.font = changeFont;
        fontAssistantText.ForceMeshUpdate();
        float afterSize = fontAssistantText.preferredHeight;

        changed = true;
        return beforeSize / afterSize;
    }

    public TMP_FontAsset GetFontFromPath(string path)
    {
        List<TMP_FontAsset> fonts = new List<TMP_FontAsset>();
        string[] filesName = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

        foreach (string name in filesName)
        {
            if (Path.GetExtension(name) == "")
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(name);
                if (bundle == null) continue;
                TMP_FontAsset font = bundle.LoadAsset<TMP_FontAsset>(Path.GetFileNameWithoutExtension(name));
                if (font != null) fonts.Add(font);
                bundle.Unload(false);
            }
        }

        return fonts.Count > 0 ? fonts[0] : null;
    }
}
