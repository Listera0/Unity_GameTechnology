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

    private string[] keyActionArray;
    private Dictionary<string, KeyCode> keyBinding;
    private Dictionary<string, KeyCode> userChanges;
    private List<KeyCode> duplicateKeys;
    private string selectAction;
    private bool changeKey;

    void Awake()
    {
        keyActionArray = new string[] { "Move_Forward", "Move_Back", "Move_Left", "Move_Right", "Jump" };
        if (keyActionArray.Length != keyButtonObjs.Length)
        {
            UnityEngine.Debug.LogWarning("Not Enough KeyButtonObjs");
        }
        else
        { 
            for (int i = 0; i < keyButtonObjs.Length; i++)
            {
                int index = i;
                keyButtonObjs[i].GetComponent<Button>().onClick.AddListener(()=>SelectChangeKey(keyActionArray[index]));
            }
        }

        keyBinding = new Dictionary<string, KeyCode>();
        keyBinding.Add("Move_Forward", KeyCode.W);
        keyBinding.Add("Move_Back", KeyCode.S);
        keyBinding.Add("Move_Left", KeyCode.A);
        keyBinding.Add("Move_Right", KeyCode.D);
        keyBinding.Add("Jump", KeyCode.Space);

        userChanges = new Dictionary<string, KeyCode>(keyBinding);
        duplicateKeys = new List<KeyCode>();
        changeKey = false;
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
        // print ui 'select change key'
        UnityEngine.Debug.Log("select change key");
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
                duplicateKeys.Add(diction.Value);
            }
            else
            { 
                seen.Add(diction.Value);
            }
        }
        
        if (duplicateKeys.Count != 0)
        {
            
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
}
