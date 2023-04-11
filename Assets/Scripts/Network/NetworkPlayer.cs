using System.IO;
using Photon.Pun;
using Player;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    PlayerData _playerData;
    [SerializeField] float cooldown = 5;
    PlayerController _playerController;
    private float _timer;
    
    bool _hasTheTag = false;
    bool _canBeTagged = true;
    private bool _isDead;
    
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        if (photonView.IsMine)
        {
            _playerData = NetworkManager.Instance.GetPlayerData();
        }
    }

    public void SpawnPlayer(Transform spawnPoint)
    {
        if (photonView.IsMine)
        {
            if (_playerController)
            {
                _playerController.Respawn();
            }
            else
            {
                GameObject playerObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { photonView.ViewID, _playerData.skinHat, _playerData.skinMaterial });
                _playerController = playerObject.GetComponent<PlayerController>();
            }
        }
    }
    
    void Update()
    {
        if (photonView.IsMine)
        {
            if (_hasTheTag)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _canBeTagged = true;
                    photonView.RPC("RPC_CanBeTagged", RpcTarget.Others);
                }
            }
        }
    }

    [PunRPC]
    void RPC_CanBeTagged()
    {
        _canBeTagged = true;
    }
    
    public bool HasTheTag()
    {
        return _hasTheTag;
    }
    
    public void SetHasTheTag(bool hasTheTag)
    {
        if (hasTheTag)
        {
            _timer = cooldown;
            _canBeTagged = false;
        }
        _hasTheTag = hasTheTag;
    }
    
    public bool CanBeTagged()
    {
        return _canBeTagged;
    }

    public void Die()
    {
        _isDead = true;
        _playerController.Die();
    }
    
    public bool IsDead()
    {
        return _isDead;
    }

    public bool Spectate()
    {
        if (!_isDead)
        {
            return false;
        }
        
        _playerController.SetCamera(true);
        return true;
    }
}