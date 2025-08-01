using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks, IInitializeInter
{
    private static NetworkManager Instance;
    public static NetworkManager instance { get { return Instance; } }

    public Button createButton;
    public Button joinButton;
    public Button exitButton;
    public TextMeshProUGUI roomInfoText;
    public Transform hostPanel;
    public GameObject chatPanel;
    public TMP_InputField userName;
    public ChatManager chatManager;
    public bool Active;

    private string selectRoomName;
    private bool isJoinRoom;
    private bool alreadyConnecting;
    private List<string> openedRoomList;

    public void Initialize()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(FindRootParent());

        if (createButton) createButton.onClick.AddListener(() => OnClickCreateButton());
        if (joinButton) joinButton.onClick.AddListener(() => OnClickJoinButton());
        if (exitButton) exitButton.onClick.AddListener(() => OnClickExitButton());
        openedRoomList = new List<string>();
        selectRoomName = "None";
        ControlPanelSetting();

        if (Active) ConnectingNetwork();
    }

    public void ConnectingNetwork()
    {
        if (alreadyConnecting == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        alreadyConnecting = true;
        Debug.Log("Successfully joined the server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Successfully joined the lobby.");
        ControlPanelSetting();
    }

    public override void OnJoinedRoom()
    {
        isJoinRoom = true;
        ControlPanelSetting();
        SendMessageInRoom(string.Format("----------- [{0}] is joined -----------", userName.text));
        Debug.Log(string.Format("Successfully joined the room ({0}).", selectRoomName));
    }

    public override void OnLeftRoom()
    {
        isJoinRoom = false;
        selectRoomName = "None";
        chatManager.ClearChatLog();
        ControlPanelSetting();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in hostPanel)
        {
            ObjectPoolManager.instance.ReturnObjectToPool(child.gameObject);
        }

        openedRoomList = new List<string>();
        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList)
            {
                openedRoomList.Add(room.Name);
                GameObject newRoomObj = ObjectPoolManager.instance.GetObjectFromPool("RoomObject");
                newRoomObj.transform.SetParent(hostPanel.transform);
                newRoomObj.transform.Find("RoomNameText(TMP)").GetComponent<TextMeshProUGUI>().text = room.Name;
                newRoomObj.GetComponent<Button>().onClick.AddListener(() => SelectRoom(room.Name));
                newRoomObj.GetComponent<Button>().onClick.AddListener(() => ControlPanelSetting());
            }
        }
        ControlPanelSetting();
    }

    private void OnClickCreateButton()
    {
        selectRoomName = GenerateRandomString(10);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.EmptyRoomTtl = 0;
        PhotonNetwork.JoinOrCreateRoom(selectRoomName, roomOptions, TypedLobby.Default);
        openedRoomList.Add(selectRoomName);
    }

    private void OnClickJoinButton()
    {
        if (hostPanel && openedRoomList.Count != 0 && selectRoomName != "None")
        {
            PhotonNetwork.JoinRoom(selectRoomName);
        }
    }

    private void OnClickExitButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void SelectRoom(string roomName)
    {
        selectRoomName = roomName;
    }

    private void ControlPanelSetting()
    {
        if (isJoinRoom)
        {
            chatPanel.SetActive(true);
            createButton.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(true);
            userName.interactable = false;
            userName.transform.GetChild(0).Find("Text").GetComponent<TextMeshProUGUI>().color = new Color32(200, 200, 200, 255);
            roomInfoText.text = selectRoomName;
        }
        else
        {
            chatPanel.SetActive(false);
            createButton.gameObject.SetActive(true);
            joinButton.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(false);
            userName.interactable = true;
            userName.transform.GetChild(0).Find("Text").GetComponent<TextMeshProUGUI>().color = new Color32(50, 50, 50, 255);
            roomInfoText.text = selectRoomName;
        }
    }

    public void SendMessageInRoom(string message)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("RecieveSendMessage", RpcTarget.All, message);
    }

    [PunRPC]
    public void RecieveSendMessage(string message)
    {
        if (!chatManager) return;
        chatManager.AddChatLog(message);
    }

    private string GenerateRandomString(int length)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
        }

        string returnValue = new string(result);

        if (openedRoomList.Contains(returnValue))
        {
            return GenerateRandomString(length);
        }

        return returnValue;
    }
    
    private GameObject FindRootParent()
    {
        GameObject parentObj = gameObject;

        while (parentObj.transform.parent != null)
        {
            parentObj = parentObj.transform.parent.gameObject;
        }

        return parentObj;
    }
}
