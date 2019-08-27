using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class GameManagerScript : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public static GameManagerScript Instance;

        [Tooltip("The prefab to use for representing the player")]  // need this for VR modification
        public GameObject playerPrefab;
        #endregion

        #region Photon Callbacks

        // called for all players (other than current)
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom(){0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom(){0}", other.NickName); //seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }


        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        #endregion

        #region Public Methods

        //abstracted so we can add more behaviour and features
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Private Methods
        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            Instance = this;

            // player instantiation 
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in the GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().name); //change from obselete Application.loadedLevelName to SceneManager.GetActiveScene.name
                                                                                                                      // we're in a room, spawn a character for the local player. it gets synced by sing PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0); //we can swap this out here for VR capable model
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for", SceneManagerHelper.ActiveSceneName);
                }
                
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
