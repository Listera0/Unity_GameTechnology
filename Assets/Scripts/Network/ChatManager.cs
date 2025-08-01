using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IInitializeInter
{
    public NetworkManager networkManager;
    public TextMeshProUGUI chatLog;
    public TMP_InputField inputField;
    public Button sendButton;

    private List<string> chatList;

    public void Initialize()
    {
        networkManager = NetworkManager.instance;
        chatLog = transform.Find("Chat Log").GetComponent<TextMeshProUGUI>();
        inputField = transform.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        sendButton = transform.Find("Send Button").GetComponent<Button>();
        sendButton.onClick.AddListener(() => SendMessage());

        chatList = new List<string>();
    }

    public void AddChatLog(string chat)
    {
        if (chatList.Count >= 20)
        {
            chatList.RemoveAt(0);
        }

        chatList.Add(chat);

        chatLog.text = "";
        foreach (string ch in chatList)
        {
            chatLog.text += ch + "\n";
        }
    }

    public void ClearChatLog()
    {
        chatList.Clear();
        chatLog.text = "";
    }

    public void SendMessage()
    {
        if (inputField.text == "") return;

        networkManager.SendMessageInRoom(string.Format("{0} : {1}", networkManager.userName.text, inputField.text));
        inputField.text = "";
    }
}
