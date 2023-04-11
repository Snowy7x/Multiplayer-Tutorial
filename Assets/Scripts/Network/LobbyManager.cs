using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    
    [SerializeField] NetworkPlayer networkPlayerPrefab;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject characterObject;
    [SerializeField] GameObject loadingScreen;
    
    [SerializeField] private LobbyPlayerItem playerListPrefab;
    [SerializeField] private Transform playerListContent;
    
    List<LobbyPlayerItem> playerListItems = new List<LobbyPlayerItem>();

    private void Start()
    {
        if (!PhotonNetwork.IsConnected || NetworkManager.Instance == null)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
        
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            LobbyPlayerItem lobbyPlayerItem = Instantiate(playerListPrefab, playerListContent);
            lobbyPlayerItem.Init(player.ActorNumber, player.NickName);
            
            playerListItems.Add(lobbyPlayerItem);
        }

        if (networkPlayerPrefab != null)
        {
            NetworkPlayer networkPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", networkPlayerPrefab.name), Vector3.zero, Quaternion.identity).GetComponent<NetworkPlayer>();
            NetworkManager.Instance.SetNetworkPlayer(networkPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        int index = playerListItems.FindIndex(x => x.playerId == newPlayer.ActorNumber);
        if (index != -1)
        {
            playerListItems[index].Init(newPlayer.ActorNumber, newPlayer.NickName);
            return;
        }
        LobbyPlayerItem lobbyPlayerItem = Instantiate(playerListPrefab, playerListContent);
        lobbyPlayerItem.Init(newPlayer.ActorNumber, newPlayer.NickName);
        
        playerListItems.Add(lobbyPlayerItem);
        
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = playerListItems.FindIndex(x => x.playerId == otherPlayer.ActorNumber);
        
        if (index != -1)
        {
            Destroy(playerListItems[index].gameObject);
            playerListItems.RemoveAt(index);
        }
        
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void StartGame()
    {
        NetworkManager.Instance.StartGame();
    }
    
    public void LeaveRoom()
    {
        characterObject.SetActive(false);
        loadingScreen.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
