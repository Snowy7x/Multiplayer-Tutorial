using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerItem : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    [HideInInspector] public int playerId;
    
    public void Init(int id, string playerName)
    {
        playerId = id;
        playerNameText.text = playerName;
    }
}
