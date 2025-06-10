using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InputKeyboardControl : MonoBehaviour
{
    public GameObject[] keyButtonObjs;
    public GameObject selectKeyPanel;

    private string[] keyActionArray;
    private Dictionary<string, KeyCode> keyBinding;
    private Dictionary<string, KeyCode> userChanges;
    private List<KeyCode> duplicateKeys;
    private string selectAction;
    private bool changeKey;
    private Color normalColor;
    private Color duplicatedColor;

    void Awake()
    {
        keyActionArray = new string[] { "Move_Forward", "Move_Back", "Move_Left", "Move_Right", "Jump", "Inventory", "Item1", "Item2", "Item3", "Item4" };
        if (keyActionArray.Length != keyButtonObjs.Length)
        {
            UnityEngine.Debug.LogWarning("Not Enough KeyButtonObjs");
        }
        else
        {
            for (int i = 0; i < keyButtonObjs.Length; i++)
            {
                int index = i;
                keyButtonObjs[i].GetComponent<Button>().onClick.AddListener(() => SelectChangeKey(keyActionArray[index]));
            }
        }

        selectKeyPanel.SetActive(false);

        keyBinding = new Dictionary<string, KeyCode>();
        keyBinding.Add("Move_Forward", KeyCode.W);
        keyBinding.Add("Move_Back", KeyCode.S);
        keyBinding.Add("Move_Left", KeyCode.A);
        keyBinding.Add("Move_Right", KeyCode.D);
        keyBinding.Add("Jump", KeyCode.Space);
        keyBinding.Add("Inventory", KeyCode.Tab);
        keyBinding.Add("Item1", KeyCode.Alpha1);
        keyBinding.Add("Item2", KeyCode.Alpha2);
        keyBinding.Add("Item3", KeyCode.Alpha3);
        keyBinding.Add("Item4", KeyCode.Alpha4);

        userChanges = new Dictionary<string, KeyCode>(keyBinding);
        duplicateKeys = new List<KeyCode>();
        changeKey = false;
        normalColor = new Color32(255, 255, 255, 255);
        duplicatedColor = new Color32(255, 175, 175, 255);
    }

    void Update()
    {
        if (changeKey) { ChangeActiveKey(); }
        else { ActiveSelectKey(); }
    }

    public void ActiveSelectKey()
    {
        string selectFunction = "Null";
        bool hasAction = false;

        if (Input.anyKeyDown)
        {
            KeyCode key = KeyCode.None;

            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    key = keycode;
                    break;
                }
            }

            selectFunction = FindBindingFromArray(key);
            hasAction = true;
        }

        if (!hasAction) return;

        switch (selectFunction)
        {
            case "Move_Forward": MoveForward(); break;
            case "Move_Back": MoveBack(); break;
            case "Move_Left": MoveLeft(); break;
            case "Move_Right": MoveRight(); break;
            case "Jump": Jump(); break;
            case "Inventory": Inventory(); break;
            case "Item1": UseItem1(); break;
            case "Item2": UseItem2(); break;
            case "Item3": UseItem3(); break;
            case "Item4": UseItem4(); break;
        }
    }

    public string FindBindingFromArray(KeyCode key)
    {
        foreach (var pair in keyBinding)
        {
            if (pair.Value == key) return pair.Key;
        }

        return "Null";
    }

    public void SelectChangeKey(string action)
    {
        changeKey = true;
        selectAction = action;
        selectKeyPanel.SetActive(true);
    }

    public void ChangeActiveKey()
    {
        if (Input.anyKeyDown)
        {
            KeyCode key = KeyCode.None;

            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    key = keycode;
                    break;
                }
            }

            for (int i = 0; i < keyActionArray.Length; i++)
            {
                if (keyActionArray[i] == selectAction)
                {
                    keyButtonObjs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = key.ToString();
                }
            }

            userChanges[selectAction] = key;
            changeKey = false;
            selectKeyPanel.SetActive(false);
            CheckDuplicationKeyBinding();
        }
    }

    public void CheckDuplicationKeyBinding()
    {
        duplicateKeys.Clear();
        HashSet<KeyCode> seen = new HashSet<KeyCode>();

        foreach (var diction in userChanges)
        {
            if (seen.Contains(diction.Value))
            {
                if (!duplicateKeys.Contains(diction.Value))
                {
                    duplicateKeys.Add(diction.Value);
                }
            }
            else
            {
                seen.Add(diction.Value);
            }
        }

        foreach (GameObject obj in keyButtonObjs)
        {
            obj.GetComponent<Image>().color = normalColor;
        }

        if (duplicateKeys.Count != 0)
        {
            for (int i = 0; i < duplicateKeys.Count; i++)
            {
                int j = 0;
                foreach (var diction in userChanges)
                {
                    if (diction.Value == duplicateKeys[i])
                    {
                        keyButtonObjs[j].GetComponent<Image>().color = duplicatedColor;
                    }
                    j++;
                }
            }
        }
    }

    public void ApplyChangeSetting()
    {
        if (duplicateKeys.Count == 0)
        {
            keyBinding = new Dictionary<string, KeyCode>(userChanges);
        }
        else
        {
            // warning message duplicated key
        }
    }

    public void MoveForward()
    {

    }

    public void MoveBack()
    {

    }

    public void MoveLeft()
    {

    }

    public void MoveRight()
    {

    }

    public void Jump()
    {

    }

    public void Inventory()
    {

    }

    public void UseItem1()
    {

    }

    public void UseItem2()
    {

    }

    public void UseItem3()
    {

    }

    public void UseItem4()
    {
        
    }
}
