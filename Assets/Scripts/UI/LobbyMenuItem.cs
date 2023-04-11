
    using Photon.Realtime;
    using TMPro;
    using UnityEngine;

    public class LobbyMenuItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text roomPlayersText;
        
        RoomInfo info;

        public void Init(RoomInfo info)
        {
            
            roomNameText.text = info.Name;
            roomPlayersText.text = info.PlayerCount + "/" + info.MaxPlayers;
        }
        
        public void JoinRoom()
        {
            NetworkManager.Instance.JoinRoom(info.Name);
        }
    }
