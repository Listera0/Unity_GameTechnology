using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField userName;

    private NetworkManager networkManager;
    private TextMeshProUGUI chatLog;
    private TMP_InputField inputField;
    private Button sendButton;

    private List<string> chatList;

    void Awake()
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

    public void SendMessage()
    {
        if (inputField.text == "") return;

        networkManager.SendMessageInRoom(string.Format("{0} : {1}", userName.text, inputField.text));
        inputField.text = "";
    }
}
