using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueData
{
    public string dialogueName;
    public DialogueCategory dialogueCategory;
    public DialougeState dialogueState;
    public List<DialogueInfo> dialogues;
    public List<DialogueData> linkDialogue;

    public DialogueData(string name, DialogueCategory category)
    {
        dialogueName = name;
        dialogueCategory = category;
        dialogueState = DialougeState.NotStart;
        dialogues = new List<DialogueInfo>();
        linkDialogue = new List<DialogueData>();
    }

    public void SetBasicDialogue(params DialogueInfo[] values)
    {
        dialogueCategory = DialogueCategory.Dialogue;
        foreach (DialogueInfo i in values)
        {
            dialogues.Add(i);
        }
    }

    public void SetSelectionDiagloue(params (DialogueInfo, DialogueData)[] values)
    {
        dialogueCategory = DialogueCategory.Selection;
        foreach (var (i, j) in values)
        {
            dialogues.Add(i);
            linkDialogue.Add(j);
        }
    }

    public void LinkNextDialogue(DialogueData dialogue)
    {
        linkDialogue.Clear();
        linkDialogue.Add(dialogue);
    }

    public bool CheckFinishDialogue()
    {
        if (linkDialogue.Count != 0) return false;

        dialogueState = DialougeState.Finished;
        return true;
    }
}

public struct DialogueInfo
{
    public string dialogueOwner;
    public string dialogue;
}

public enum DialogueCategory
{
    Dialogue = 0,
    Selection,
    Event
}

public enum DialougeState
{
    NotStart = 0,
    Started,
    Finished
}

public class DialogueManager : Singleton<DialogueManager>
{

}
