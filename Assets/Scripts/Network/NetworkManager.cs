using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    RoomInfo[] _rooms;
    PlayerData _playerData;
    NetworkPlayer _networkPlayer;
    
    void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this);
        _playerData = new PlayerData(0, 0);
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }
    
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    
    
    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }
    
    public override void OnJoinedLobby()
    {
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenMenu("Lobby");
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        base.OnJoinedLobby();
    }
    
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Loading");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _rooms = roomList.FindAll(x => x.RemovedFromList == false).ToArray();
    }
    

    public override void OnLeftRoom()
    {
        _networkPlayer = null;
        PhotonNetwork.LoadLevel(0);
        base.OnLeftRoom();
    }

    #endregion
 
    public RoomInfo[] GetRooms()
    {
        return _rooms;
    }
    
    public void SetPlayerData(PlayerData playerData)
    {
        _playerData = playerData;
    }
    
    public void SetMatIndex(int matIndex)
    {
        _playerData.skinMaterial = matIndex;
    }
    
    public void SetHatIndex(int hatIndex)
    {
        _playerData.skinHat = hatIndex;
    }
    
    public PlayerData GetPlayerData()
    {
        return _playerData;
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void SetUsername(string user)
    {
        PhotonNetwork.NickName = user;
        PlayerPrefs.SetString("Username", user);
    }
    
    public void SetNetworkPlayer(NetworkPlayer networkPlayer)
    {
        _networkPlayer = networkPlayer;
    }

    public NetworkPlayer GetNetworkPlayer()
    {
        return _networkPlayer;
    }

}
