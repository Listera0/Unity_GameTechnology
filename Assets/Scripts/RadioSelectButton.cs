using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RadioCategory
{
    Unlinked = 0,
    Linked
}

public class RadioSelectButton : MonoBehaviour
{
    public bool initSetting;
    public RadioCategory radioCategory;
    public GameObject[] selectObject;
    public GameObject[] targetObject;

    public Color UnVisibleColor = Color.black;
    public Color VisibleColor = Color.black;

    private bool failedMatchObject;
    private int selectIndex;

    void Awake()
    {
        if (initSetting) RadioSelectButtonInit();
    }

    public void RadioSelectButtonInit()
    {
        if (radioCategory == RadioCategory.Unlinked)
        {
            failedMatchObject = false;
            foreach (GameObject obj in selectObject)
            {
                obj.GetComponent<Button>().onClick.AddListener(() => SelectIndex(obj));
            }
        }
        else if (radioCategory == RadioCategory.Linked)
        {
            if (selectObject.Length != targetObject.Length)
            {
                failedMatchObject = true;
                Debug.LogWarning("There's no same length between selectObject and targetObject!");
            }
            else
            {
                failedMatchObject = false;
                foreach (GameObject obj in selectObject)
                {
                    obj.GetComponent<Button>().onClick.AddListener(() => SelectIndex(obj));
                }
            }
        }

        if (UnVisibleColor == VisibleColor)
            UnVisibleColor.a = UnVisibleColor.a * 0.5f;
    }

    public void SelectIndex(GameObject obj)
    {
        if (!failedMatchObject)
        {
            selectIndex = FindSelectObj(obj);
            UnVisibleSelect(selectIndex);
        }
    }

    public GameObject GetSelectGameObject()
    {
        return targetObject[selectIndex];
    }

    public void UnVisibleSelect(int exception)
    {
        for (int i = 0; i < selectObject.Length; i++)
        {
            if (i == exception)
            {
                selectObject[i].GetComponent<Image>().color = VisibleColor;
                if(radioCategory == RadioCategory.Linked) { targetObject[i].SetActive(true); }
                continue;
            }

            selectObject[i].GetComponent<Image>().color = UnVisibleColor;
            if(radioCategory == RadioCategory.Linked) { targetObject[i].SetActive(false); }
        }
    }

    private int FindSelectObj(GameObject select)
    {
        for (int i = 0; i < selectObject.Length; i++)
        {
            if (select == selectObject[i])
            {
                return i;
            }
        }

        return 0;
    }
}
