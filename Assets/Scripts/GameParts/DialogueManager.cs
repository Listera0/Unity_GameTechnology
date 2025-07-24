using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueCategory
{
    Dialogue = 0,
    Select,
    Event
}

public enum DialougeState
{
    Locked = 0,
    NotStart,
    Started,
    Finished
}

public struct DialogueInfo
{
    public string dialogueOwner;
    public string dialogue;

    public DialogueInfo(string owner, string dia)
    {
        dialogueOwner = owner;
        dialogue = dia;
    }
}

public class DialogueData
{
    public string dialogueName;
    public DialogueCategory dialogueCategory;
    public DialougeState dialogueState;
    public List<DialogueData> requireDialogue;
    public List<DialogueInfo> dialogues;

    // SelectDialogueOption
    public int selected;
    public List<string> selectOptions;
    public List<DialogueData> linkDialogues;
    public List<DialogueData> prevDialogues;

    public DialogueData(string name)
    {
        dialogueName = name;
        dialogueCategory = DialogueCategory.Dialogue;
        dialogueState = DialougeState.Locked;
        requireDialogue = new List<DialogueData>();
        dialogues = new List<DialogueInfo>();
        selectOptions = new List<string>();
        linkDialogues = new List<DialogueData>();
        prevDialogues = new List<DialogueData>();
    }

    public void SetBasicDialogue(params DialogueInfo[] values)
    {
        dialogueCategory = DialogueCategory.Dialogue;
        foreach (DialogueInfo i in values)
        {
            dialogues.Add(i);
        }
    }

    public void SetSelectDiagloue(params (string, DialogueData)[] values)
    {
        dialogueCategory = DialogueCategory.Select;
        foreach (var (i, j) in values)
        {
            selectOptions.Add(i);
            linkDialogues.Add(j);
        }
    }

    public void AddRequireDialogue(params DialogueData[] values)
    {
        foreach (DialogueData i in values)
        {
            requireDialogue.Add(i);
        }
    }
}

public class DialogueManager : Singleton<DialogueManager>
{
    private List<DialogueData> dialogueDatabase;
    private DialogueData currentDia;
    private bool startedDialouge;
    private int currentIndex;
    private int currentProgress;

    public Transform dialogueListPanel;
    public GameObject dialoguePrefab;
    public TextMeshProUGUI dialougeText;
    public Button dialougePanel;
    public Transform dialogueSelectPanel;

    void Start()
    {
        dialogueDatabase = new List<DialogueData>();

        DialogueData welcome = new DialogueData("Welcome!");
        welcome.SetBasicDialogue(new DialogueInfo("System", "Hello! This part is dialouge. Click dialouge panel to continue"), new DialogueInfo("System", "Now add other dialouge for test"));
        dialogueDatabase.Add(welcome);

        DialogueData selectTest_SO_0 = new DialogueData("SelectTest_SO_0");
        selectTest_SO_0.SetBasicDialogue(new DialogueInfo("System", "This is Option 0"));

        DialogueData selectTest_SO_1 = new DialogueData("SelectTest_SO_1");
        selectTest_SO_1.SetBasicDialogue(new DialogueInfo("System", "This is Option 1"));

        DialogueData selectTest_SO_2 = new DialogueData("SelectTest_SO_2");
        selectTest_SO_2.SetBasicDialogue(new DialogueInfo("System", "This is Option 2"));

        DialogueData selectTest = new DialogueData("Select Test");
        selectTest.SetBasicDialogue(new DialogueInfo("System", "This dialogue is test for select option"));
        selectTest.SetSelectDiagloue(("option0", selectTest_SO_0), ("option1", selectTest_SO_1), ("option2", selectTest_SO_2));
        selectTest.AddRequireDialogue(welcome);
        dialogueDatabase.Add(selectTest);

        dialougePanel.gameObject.SetActive(false);
        dialogueSelectPanel.gameObject.SetActive(false);

        dialougePanel.onClick.AddListener(() => ActDialouge());
        dialogueSelectPanel.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SelectOption(0));
        dialogueSelectPanel.GetChild(1).GetComponent<Button>().onClick.AddListener(() => SelectOption(1));
        dialogueSelectPanel.GetChild(2).GetComponent<Button>().onClick.AddListener(() => SelectOption(2));

        UpdateDialogueList();
    }

    public void StartDialouge(int index)
    {
        if (dialogueDatabase[index].dialogueState == DialougeState.Finished) return;

        currentIndex = index;
        currentProgress = -1;
        startedDialouge = true;
        ActDialouge();
    }

    public void ActDialouge()
    {
        if (!startedDialouge) return;
        currentProgress++;

        currentDia = dialogueDatabase[currentIndex];

        while (currentDia.dialogueState == DialougeState.Started)
        {
            currentDia = currentDia.linkDialogues[currentDia.selected];
        }

        dialougePanel.gameObject.SetActive(false);
        dialogueSelectPanel.gameObject.SetActive(false);

        if (currentDia.dialogueCategory == DialogueCategory.Dialogue)
        {
            DialogueAction();
        }
        else if (currentDia.dialogueCategory == DialogueCategory.Select)
        {
            SelectAction();
        }
    }

    public void DialogueAction()
    {
        if (currentDia.dialogues.Count == currentProgress)
        {
            foreach (DialogueData prev in currentDia.prevDialogues)
                prev.dialogueState = DialougeState.Finished;
            currentDia.dialogueState = DialougeState.Finished;
            startedDialouge = false;
            UpdateDialogueList();
        }
        else
        {
            dialougePanel.gameObject.SetActive(true);
            dialougeText.text = string.Format("{0} : {1}", currentDia.dialogues[currentProgress].dialogueOwner, currentDia.dialogues[currentProgress].dialogue);
        }
    }

    public void SelectAction()
    {
        if (currentDia.dialogues.Count == currentProgress)
        {
            dialogueSelectPanel.gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                if (i < currentDia.selectOptions.Count)
                {
                    dialogueSelectPanel.GetChild(i).gameObject.SetActive(true);
                    dialogueSelectPanel.GetChild(i).Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = currentDia.selectOptions[i];
                }
                else
                {
                    dialogueSelectPanel.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            dialougePanel.gameObject.SetActive(true);
            dialougeText.text = string.Format("{0} : {1}", currentDia.dialogues[currentProgress].dialogueOwner, currentDia.dialogues[currentProgress].dialogue);
        }
    }

    public void SelectOption(int index)
    {
        currentProgress = -1;
        currentDia.selected = index;
        currentDia.linkDialogues[index].prevDialogues.AddRange(currentDia.prevDialogues);
        currentDia.linkDialogues[index].prevDialogues.Add(currentDia);
        foreach (DialogueData prev in currentDia.linkDialogues[index].prevDialogues)
            prev.dialogueState = DialougeState.Started;
        ActDialouge();
    }

    // need object pool
    public void UpdateDialogueList()
    {
        foreach (Transform child in dialogueListPanel)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        foreach (DialogueData data in dialogueDatabase)
        {
            if (data.dialogueState == DialougeState.Locked)
            {
                bool check = true;
                foreach (DialogueData require in data.requireDialogue)
                {
                    if (require.dialogueState != DialougeState.Finished) check = false; break;
                }

                if (check) data.dialogueState = DialougeState.NotStart;
            }
            
            if(data.dialogueState != DialougeState.Locked)
            {
                GameObject newDialogue = Instantiate(dialoguePrefab, dialogueListPanel);
                int i = index;
                newDialogue.GetComponent<Button>().onClick.AddListener(() => StartDialouge(i));
                index++;
                TextMeshProUGUI newDialogueName = newDialogue.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                newDialogueName.text = data.dialogueName;

                if (data.dialogueState == DialougeState.Finished)
                {
                    // 중앙라인 추가
                    // newDialogueName.text = ;
                    newDialogueName.fontStyle = FontStyles.Strikethrough;
                    newDialogueName.color = new Color32(150, 150, 150, 255);
                }
            }
        }
    }

    public bool IsContainDialogue(string name)
    {
        foreach (DialogueData playerDialougeData in dialogueDatabase)
        {
            if (playerDialougeData.dialogueName == name) return true;
        }
        return false;
    }

    public DialogueData FindDialogueFromName(string name)
    {
        foreach (DialogueData data in dialogueDatabase)
        {
            if (data.dialogueName == name) return data;
        }

        return null;
    }
}
