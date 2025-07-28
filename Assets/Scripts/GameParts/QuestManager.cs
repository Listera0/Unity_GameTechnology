using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotStart = 0,
    Started,
    RequrieFinish,
    Finished
}

public class QuestInfo
{
    public string questName;
    public QuestState questState;
    public List<QuestCondition> questCondition;
    public List<string> questReward;

    public QuestInfo(string name)
    {
        questName = name;
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

    void Start()
    {
        questDatabases = new List<QuestInfo>();

        QuestInfo testQuest = new QuestInfo("TestQuest");
        testQuest.AddQuestCondition(new QuestCondition("SelectTest_SO_0", 1));
        questDatabases.Add(testQuest);

        questDatabases[0].questState = QuestState.Started;
    }

    public void Notify(IObserverTarget obj, string value)
    {
        switch (obj)
        {
            case DialogueManager:
                print("Recieve Finish Dialogue : " + value);
                ChangeQuestParams(value, QuestState.Finished);
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
                questDatabases[i].questState = QuestState.RequrieFinish;
                print("Requrie Finish Quest : " + questDatabases[i].questName);
            }
        }
    }
}
