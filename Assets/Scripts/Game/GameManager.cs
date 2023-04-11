using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Player;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [RequireComponent(typeof(PhotonView))]
    public class GameManager : MonoBehaviourPun
    {
        [SerializeField] TMP_Text timerText;
        [SerializeField] TMP_Text gameOverText;
        [SerializeField] TMP_Text roundEndText;
        
        // [SerializeField] Camera spectatorCamera;
        [SerializeField] float roundTime = 30f;
        [SerializeField] List<Transform> spawnPoints = new List<Transform>();
        public static GameManager instance;
        public NetworkPlayer localPlayer;
        
        List<NetworkPlayer> players = new List<NetworkPlayer>();
        private int _index;
        float _roundTimer;
        bool _isGameStarted;
        bool _gameEnded;

        void Awake()
        {
            if (instance)
            {
                Destroy(this);
                return;
            }
            
            instance = this;
        }
        
        void Start()
        {
            localPlayer = NetworkManager.Instance.GetNetworkPlayer();
            players = FindObjectsOfType<NetworkPlayer>().ToList();
            _index = PhotonNetwork.CurrentRoom.Players.Values.ToList().FindIndex(x => x.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
            localPlayer.SpawnPlayer(spawnPoints[_index]);
            StartGame();
        }

        private void Update()
        {
            if (!_isGameStarted) return;
            _roundTimer += Time.deltaTime;
            if (_roundTimer >= roundTime)
            {
                EndRound();
            }
            
            timerText.text = Mathf.RoundToInt(roundTime - _roundTimer).ToString();
        }

        void StartGame()
        {
            // Get a random player;
            NetworkPlayer randomPlayer = players[Random.Range(0, players.Count)];
            SetTag(randomPlayer.photonView.Owner.ActorNumber);
            _isGameStarted = true;
        }

        void EndRound()
        {
            _isGameStarted = false;
            StartCoroutine(RoundEnd());
        }
        
        bool isGameEnd = false;

        IEnumerator RoundEnd()
        {
            foreach (var player in players)
            {
                if (player.HasTheTag())
                {
                    player.Die();
                    if (player.photonView.IsMine)
                    {
                        roundEndText.text = "You are Eliminated!";
                        roundEndText.color = Color.red;
                    }
                    else
                    {
                        roundEndText.text = "Player " + player.photonView.Owner.ActorNumber + " is Eliminated!";
                        roundEndText.color = Color.green;
                    }
                }
            }

            if (localPlayer.IsDead())
            {
                foreach (var player in players)
                {
                    if (player.Spectate()) break;
                }
            }

            if (players.Count(x => !x.IsDead()) <= 1)
            {
                if (localPlayer.IsDead())
                {
                    gameOverText.text = "You Lost!";
                    gameOverText.color = Color.red;
                }
                else
                {
                    gameOverText.text = "You Won!";
                    gameOverText.color = Color.green;
                }

                _gameEnded = true;
            }

            yield return new WaitForSeconds(3f);
            if (_gameEnded)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                roundEndText.text = "";
                StartGame();
            }
        }

        public void SetTag(int an)
        {
            NetworkPlayer networkPlayer = players.Find(x => x.photonView.Owner.ActorNumber == an);
            if (networkPlayer == null || !networkPlayer.CanBeTagged()) return;
            photonView.RPC("RPC_SetTag", RpcTarget.All, an);
        }

        IEnumerator SetTag()
        {
            roundEndText.color = Color.red;
            roundEndText.text = "You got tagged";
            yield return new WaitForSeconds(3f);
            roundEndText.text = "";
        }
        
        [PunRPC]
        void RPC_SetTag(int playerId)
        {
            foreach (var player in players)
            {
                player.SetHasTheTag(player.photonView.Owner.ActorNumber == playerId);
            }
            
            if (localPlayer.photonView.Owner.ActorNumber == playerId)
            {
                StartCoroutine(SetTag());
            }
        }

        public void Spectate()
        {
            foreach (var player in players)
            {
                if (!player.IsDead())
                {
                    
                }
            }
        }


    }
}