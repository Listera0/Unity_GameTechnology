using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioSelectButton : MonoBehaviour
{
    public GameObject[] selectObject;
    public GameObject[] targetObject;

    public Color UnVisibleColor;
    public Color VisibleColor;

    private bool failedMatchObject;
    private int selectIndex;

    void Awake()
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

    private void UnVisibleSelect(int exception)
    {
        for (int i = 0; i < selectObject.Length; i++)
        {
            if (i == exception)
            {
                selectObject[i].GetComponent<Image>().color = VisibleColor;
                targetObject[i].SetActive(true);
                continue;
            }

            selectObject[i].GetComponent<Image>().color = UnVisibleColor;
            targetObject[i].SetActive(false);
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
