using System;
using Game;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviourPun
    {
        [Header("Debug / Networking")]
        [SerializeField] bool isOffline;
        
        [Space]
        [Header("References")]
        [SerializeField] TMP_Text nameText;
        [SerializeField] Animator animator;
        [SerializeField] Transform model;
        [SerializeField] InputManager inputManager;
        [SerializeField] Rigidbody rb;
        [SerializeField] Camera cam;
        
        [Space]
        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float jumpForce = 5f;
        [SerializeField] float gravity = 9.81f;
        [SerializeField] float leanMax = 10f;
        [SerializeField] float mouseSensitivity = 100f;
        
        [Space]
        [Header("Ground Check")]
        [SerializeField] float groundDistance = 0.4f;
        [SerializeField] Transform groundCheck;
        [SerializeField] LayerMask groundMask;

        Vector3 _moveDirection;
        Vector2 _lookDirection;
        private Vector3 _originalBodyRot;
        
        private PlayerData _playerData;
        NetworkPlayer _networkPlayer;
        
        bool jump;
        float jumpTimeCounter;
        
        Transform _transform;
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private void Start()
        {
            nameText.text = photonView.Owner.NickName;
            if (photonView.IsMine && !isOffline)
            {
                int id = (int)photonView.InstantiationData[0];
                if (id > 0) _networkPlayer = PhotonView.Find((int)photonView.InstantiationData[0]).GetComponent<NetworkPlayer>();
            }
            if (!photonView.IsMine && !isOffline)
            {
                Destroy(inputManager);
                Destroy(this);
                cam.gameObject.SetActive(false);
            }
            else
            {
                inputManager.OnMoveAction += OnMove;
                inputManager.OnLookAction += OnLook;
                inputManager.OnJumpAction += OnJump;
                inputManager.OnFireAction += OnFire;
                _transform = transform;
                _originalBodyRot = model.localEulerAngles;
            }
        }
        
        void Update()
        {
            UpdateAnimation();
            
            if (!photonView.IsMine && !isOffline) return;
            Look();
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine && !isOffline) return;
            Move();
        }

        void UpdateAnimation()
        {
            // Leaning to the direction of movement [forwards, backwards, left, right]
            Vector3 lean =  _originalBodyRot;
            lean.x += _moveDirection.z * leanMax;
            lean.z -= _moveDirection.x * leanMax;
            
            // Smoothly lean to the direction of movement
            model.localEulerAngles = Quaternion.Lerp(model.localRotation, Quaternion.Euler(lean), Time.deltaTime * 10f).eulerAngles;
            
            // idle if no movement
            Vector3 velocity = rb.velocity;
            if (velocity.magnitude < 0.1f)
            {
                animator.SetBool(IsMoving, false);
            }
            else
            {
                animator.SetBool(IsMoving, true);
            }
        }
 
        void OnJump(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Started)
            {
                jump = true;
            }else if (phase == InputActionPhase.Canceled)
            {
                jump = false;
            }
        }
        
        void OnMove(InputActionPhase phase, Vector2 value)
        {
            _moveDirection = new Vector3(value.x, 0, value.y);
        }
        
        void OnLook(InputActionPhase phase, Vector2 value)
        {
            _lookDirection = value;
        }
        
        void OnFire(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Started)
            {
                Fire();
            }
        }

        void Move()
        {
            Vector3 velocity;
            velocity = transform.right * _moveDirection.x * moveSpeed + transform.forward * _moveDirection.z * moveSpeed;
            
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
            if (velocity.magnitude < 0.1f)
            {
                rb.angularVelocity = Vector3.zero;
            }
            if (jump && IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.down * gravity);
        }
        
        float _xRotation = 0f;
        
        void Look()
        {
            if (_lookDirection == Vector2.zero) return;
            float mouseX = _lookDirection.x * mouseSensitivity * Time.deltaTime;
            float mouseY = _lookDirection.y * mouseSensitivity * Time.deltaTime;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -60f, 60f);
            
            cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            _transform.Rotate(Vector3.up * mouseX);
        }

        void Fire()
        {
            // TODO: Slap animation
            animator.Play("Hit");
        }

        [PunRPC]
        void Slap()
        {
            animator.Play("Hit");
            if (photonView.IsMine)
            {
                Physics.SphereCast(cam.transform.position, 0.5f, cam.transform.forward, out RaycastHit hit, 2f);
                if (hit.collider != null)
                {
                    NetworkPlayer player = hit.collider.gameObject.GetComponent<NetworkPlayer>();
                    if (player != null && player.photonView.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        GameManager.instance.SetTag(player.photonView.Owner.ActorNumber);
                    }
                }
            }
        }

        bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }

        public void Die()
        {
            model.gameObject.SetActive(false);
            rb.isKinematic = true;
        }
        
        public void Respawn()
        {
            model.gameObject.SetActive(true);
            rb.isKinematic = false;
        }

        public void SetCamera(bool state)
        {
            cam.gameObject.SetActive(state);
        }
    }
}