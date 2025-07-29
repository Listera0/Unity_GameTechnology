using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum QuestState
{
    NotStart = 0,
    Started,
    RequireFinish,
    Finished
}

public class QuestInfo
{
    public string questName;
    public string questDetail;
    public QuestState questState;
    public List<QuestCondition> questCondition;
    public List<string> questReward;

    public QuestInfo(string name)
    {
        questName = name;
        questDetail = "";
        questState = QuestState.NotStart;
        questCondition = new List<QuestCondition>();
        questReward = new List<string>();
    }

    public void AddQuestCondition(QuestCondition condition)
    {
        int index = HasAlreadyCondition(condition);
        if (index == -1)
        {
            questCondition.Add(condition);
        }
        else
        {
            questCondition[index].requireCount += condition.requireCount;
        }
    }

    public int HasAlreadyCondition(QuestCondition condition)
    {
        for (int i = 0; i < questCondition.Count; i++)
        {
            if (questCondition[i].conditionName == condition.conditionName)
            {
                return i;
            }
        }
        return -1;
    }
}

public class QuestCondition
{
    public string conditionName;
    public QuestState conditionState;
    public int requireCount;
    public int currentCount;

    public QuestCondition(string name, int require)
    {
        conditionName = name;
        conditionState = QuestState.NotStart;
        requireCount = require;
        currentCount = 0;
    }
}

public class QuestManager : Singleton<QuestManager>, IObserver
{
    private List<QuestInfo> questDatabases;

    public Transform questListPanel;
    public GameObject questInfoPanel;
    private int selectQuest;

    private TextMeshProUGUI detail_Tittle;
    private TextMeshProUGUI detail_Detail;
    private TextMeshProUGUI detail_Condition;
    private TextMeshProUGUI detail_ActingButton;

    void Start()
    {
        SetQuestDatabase();

        detail_Tittle = questInfoPanel.transform.Find("Tittle Text").GetComponent<TextMeshProUGUI>();
        detail_Detail = questInfoPanel.transform.Find("Detail Text").GetComponent<TextMeshProUGUI>();
        detail_Condition = questInfoPanel.transform.Find("Condition Text").GetComponent<TextMeshProUGUI>();
        detail_ActingButton = questInfoPanel.transform.Find("Action Button").GetChild(0).GetComponent<TextMeshProUGUI>();
        questInfoPanel.transform.Find("Action Button").GetComponent<Button>().onClick.AddListener(() => OnClickActionButton());

        UpdateQuestList();
        ResetQuestInfoPanel();
    }

    public void SetQuestDatabase()
    {
        questDatabases = new List<QuestInfo>();

        QuestInfo testQuest = new QuestInfo("TestQuest");
        testQuest.AddQuestCondition(new QuestCondition("SelectTest_SO_0", 1));
        questDatabases.Add(testQuest);
    }

    public void SetQuestInfoPanel(int index)
    {
        if (index == -1) return;

        selectQuest = index;
        QuestInfo quest = questDatabases[index];

        detail_Tittle.text = quest.questName;
        detail_Detail.text = quest.questDetail;

        string conditionText = "";
        foreach (QuestCondition con in quest.questCondition)
        {
            conditionText += con.conditionName + (con.conditionState == QuestState.Finished ? " (Clear)" : string.Format(" ({0} / {1})", con.currentCount, con.requireCount)) + "\n";
        }
        if (conditionText.Length > 3) conditionText = conditionText.Substring(0, conditionText.Length - 1);

        detail_Condition.text = conditionText;

        if (quest.questState == QuestState.NotStart) { detail_ActingButton.text = "Start"; }
        else if (quest.questState == QuestState.Started) { detail_ActingButton.text = "Progressing"; }
        else if (quest.questState == QuestState.RequireFinish) { detail_ActingButton.text = "Clear"; }
        else if (quest.questState == QuestState.Finished) { detail_ActingButton.text = "Finished"; }
    }

    public void ResetQuestInfoPanel()
    {
        selectQuest = -1;
        detail_Tittle.text = "";
        detail_Detail.text = "";
        detail_Condition.text = "";
        detail_ActingButton.text = "Choose Quest";
    }

    public void OnClickActionButton()
    {
        if (selectQuest == -1) return;

        if (questDatabases[selectQuest].questState == QuestState.NotStart)
        {
            questDatabases[selectQuest].questState = QuestState.Started;
            UpdateQuestList();
            SetQuestInfoPanel(selectQuest);
        }
        else if (questDatabases[selectQuest].questState == QuestState.RequireFinish)
        {
            questDatabases[selectQuest].questState = QuestState.Finished;
            // get quest reward;
            ResetQuestInfoPanel();
            UpdateQuestList();
        }
    }

    public void UpdateQuestList()
    {
        for (int i = questListPanel.childCount - 1; i >= 0; i--)
        {
            ObjectPoolManager.instance.ReturnObjectToPool(questListPanel.GetChild(i).gameObject);
        }

        int index = 0;
        foreach (QuestInfo data in questDatabases)
        {
            GameObject newQuest = ObjectPoolManager.instance.GetObjectFromPool("Quest");
            newQuest.transform.SetParent(questListPanel);
            int i = index;
            newQuest.GetComponent<Button>().onClick.AddListener(() => SetQuestInfoPanel(i));
            index++;
            TextMeshProUGUI newQuestName = newQuest.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

            string questObjName = data.questName;

            newQuestName.fontStyle = FontStyles.Normal;
            newQuestName.color = new Color32(0, 0, 0, 255);

            if (data.questState == QuestState.NotStart) { questObjName += " (Not Start)"; }
            else if (data.questState == QuestState.Started) { questObjName += " (Progressing)"; }
            else if (data.questState == QuestState.RequireFinish) { questObjName += " (Clear)"; }
            else if (data.questState == QuestState.Finished)
            {
                questObjName += " (Finish)";
                newQuestName.fontStyle = FontStyles.Strikethrough;
                newQuestName.color = new Color32(150, 150, 150, 255);
            }
            
            newQuestName.text = questObjName;
        }
    }

    public void Notify(IObserverTarget obj, string value)
    {
        switch (obj)
        {
            case DialogueManager:
                print("Recieve Finish Dialogue : " + value);
                ChangeQuestParams(value, QuestState.Finished);
                UpdateQuestList();
                SetQuestInfoPanel(selectQuest);
                break;
        }
    }

    public void ChangeQuestParams(string name, QuestState value)
    {
        for (int i = 0; i < questDatabases.Count; i++)
        {
            if (questDatabases[i].questState != QuestState.Started) continue;

            for (int j = 0; j < questDatabases[i].questCondition.Count; j++)
            {
                if (questDatabases[i].questCondition[j].conditionName == name)
                {
                    questDatabases[i].questCondition[j].conditionState = value;
                    break;
                }
            }
        }
        CheckFinishQuests();
    }

    public void CheckFinishQuests()
    {
        for (int i = 0; i < questDatabases.Count; i++)
        {
            if (questDatabases[i].questState != QuestState.Started) continue;

            bool flag = true;
            for (int j = 0; j < questDatabases[i].questCondition.Count; j++)
            {
                QuestCondition condition = questDatabases[i].questCondition[j];
                if (condition.conditionState != QuestState.Finished && condition.requireCount > condition.currentCount)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                questDatabases[i].questState = QuestState.RequireFinish;
                print("Requrie Finish Quest : " + questDatabases[i].questName);
            }
        }
    }
}
