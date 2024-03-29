﻿using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    // use IPunObservable to explicitly observe and synch beam firing
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields
        [Tooltip("Local Player Instance. Use this to know if the local player is represented in the Scene.")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        // holds reference to Player UI prefab to allow for instantiation
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;
        #endregion

        #region Private Fields

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        // True, when user is firing
        bool IsFiring;
        #endregion


        #region Private Methods
#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif


        #endregion
        #region MonoBehaviour Callbacks
        void Start()
        {
            // for UI instantiation
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.",this);
            }

            // end of UI instantiation
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork component on playerPrefab", this);
            }

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }



        void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            DontDestroyOnLoad(this.gameObject);

            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>MIssing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }


        }

        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
             
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f;
        }


        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }

            Health -= 0.1f * Time.deltaTime;
        } 


        void Update()
        {
            // only process if we are the local player
            if (photonView.IsMine)
            {
                if (Health <= 0f)
                {
                    GameManagerScript.Instance.LeaveRoom();
                }
                
            }
            if (photonView.IsMine)
            {
                ProcessInputs();
            }

            // trigger beams acrive state
            if (beams != null && IsFiring != beams.activeSelf)
            {
                beams.SetActive(IsFiring);
            }
        }

#if !UNITY_5_4_OR_NEWER
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
            // need to re instantiate Player Ui prefab for when the player is leaving and joing anew level/room
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            

            
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        #endregion

        #region Custom

        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }



#endregion

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // we own this player, send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }
        #endregion


    }

}
