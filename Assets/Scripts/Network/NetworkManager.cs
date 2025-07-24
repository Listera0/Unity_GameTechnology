using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private RadioSelectButton hostPanel;
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private GameObject roomObjectPrefab;
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private bool Active;

    private List<String> openedRoomList;

    void Awake()
    {
        if (createButton) createButton.onClick.AddListener(() => OnClickCreateButton());
        if (joinButton) joinButton.onClick.AddListener(() => OnClickJoinButton());

        openedRoomList = new List<string>();
    }

    void Start()
    {
        if(Active) PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully joined the server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Successfully joined the lobby.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(string.Format("Successfully joined the room({0}).", openedRoomList[hostPanel.GetSelectIndex()]));
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count > hostPanel.transform.childCount)
        {
            for (int i = hostPanel.transform.childCount; i < roomList.Count; i++)
            {
                CreateRoom(roomList[i].Name);
            }
        }
        else if (roomList.Count < hostPanel.transform.childCount)
        {
            for (int i = hostPanel.transform.childCount - 1; i >= roomList.Count; i++)
            {
                RemoveRoom(i);
            }
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            hostPanel.transform.GetChild(i).Find("RoomNameText(TMP)").GetComponent<TextMeshProUGUI>().text = roomList[i].Name;
        }
    }

    private void OnClickCreateButton()
    {
        if (!roomObjectPrefab) return;

        string roomName = GenerateRandomString(10);
        RoomOptions roomOptions = new RoomOptions();

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        chatPanel.SetActive(true);
        openedRoomList.Add(roomName);
    }

    private void OnClickJoinButton()
    {
        if (openedRoomList.Count != 0 && hostPanel)
        {
            PhotonNetwork.JoinRoom(openedRoomList[hostPanel.GetSelectIndex()]);
            chatPanel.SetActive(true);
        }
    }

    private void GetStateCurrentNetwork()
    {
        print(PhotonNetwork.NetworkClientState.ToString());
    }

    private void CreateRoom(string roomName)
    {
        openedRoomList.Add(roomName);
        GameObject newRoomObj = Instantiate(roomObjectPrefab, hostPanel.transform);
        newRoomObj.transform.Find("RoomNameText(TMP)").GetComponent<TextMeshProUGUI>().text = roomName;
        hostPanel.AddSelectObject(newRoomObj);
    }

    private void RemoveRoom(int index)
    {
        openedRoomList.RemoveAt(index);
        Destroy(hostPanel.transform.GetChild(index).gameObject);
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
}
