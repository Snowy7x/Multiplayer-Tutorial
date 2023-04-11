using System;
using Photon.Realtime;
using TMPro;
    using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField roomNameInput;
    
    [SerializeField] Transform roomListContent;
    [SerializeField] LobbyMenuItem lobbyMenuItemPrefab;
    
    [SerializeField] float roomListUpdateInterval = 1f;
    RoomInfo[] _rooms;

    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            MenuManager.Instance.OpenMenu("Loading");
            usernameInput.text = PlayerPrefs.GetString("Username");
            NetworkManager.Instance.SetUsername(usernameInput.text);
            NetworkManager.Instance.ConnectToServer();
            Debug.Log("Username is not empty");
        }
        else
        {
            MenuManager.Instance.OpenMenu("Login");
            Debug.Log("Username is empty");
        }
        
        if (roomListContent != null && lobbyMenuItemPrefab != null)
        {
            InvokeRepeating(nameof(UpdateRoomList), 0f, roomListUpdateInterval);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(UpdateRoomList));
    }
    
    private void OnDisable()
    {
        CancelInvoke(nameof(UpdateRoomList));
    }
    
    public void ConnectToServer()
    {
        if (!string.IsNullOrEmpty(usernameInput.text))
        {
            PlayerPrefs.SetString("Username", usernameInput.text);
            NetworkManager.Instance.SetUsername(usernameInput.text);
            NetworkManager.Instance.ConnectToServer();
        }
        else
        {
            Debug.Log("Username is empty");
        }
    }
    
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            MenuManager.Instance.OpenMenu("Loading");
            NetworkManager.Instance.CreateRoom(roomNameInput.text);
        }
        else
        {
            Debug.Log("Room name is empty");
        }
    }
    
    public void JoinRandomRoom()
    {
        MenuManager.Instance.OpenMenu("Loading");
        NetworkManager.Instance.JoinRandomRoom();
    }
    
    public void UpdateRoomList()
    {
        RoomInfo[] newRooms = NetworkManager.Instance.GetRooms();
        if (newRooms != _rooms)
        {
            _rooms = newRooms;            
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (RoomInfo room in _rooms)
            {
                LobbyMenuItem lobbyMenuItem = Instantiate(lobbyMenuItemPrefab, roomListContent);
                lobbyMenuItem.Init(room);
            }
        }
    }

    public void Login()
    {
        NetworkManager.Instance.SetUsername(usernameInput.text);
        MenuManager.Instance.OpenMenu("Loading");
        NetworkManager.Instance.ConnectToServer();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
