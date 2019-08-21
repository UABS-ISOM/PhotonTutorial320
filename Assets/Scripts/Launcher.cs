﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime; //added to use MonoBehaviourPunCallbacks

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The max number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4; //initial value that can be changed in the Unity inspector

        #endregion

        #region Private Fields

        // Client's version number -- we separate users from each other bty gameVersion based on "gamebreaking" version features
        string gameVersion = "1"; //re: see Semantic Versioning for versioning choice and connotations

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks
        //we are going to override the OnConnectedToMaster() and OnDisconnected() PUN callbacks here
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinRandomRoom(); //we then implement OnJoinRandomRoomFailed() PUN callback if no room exists so we can create one using PhotonNetwork.CreateRoom()
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause); //super helpful since we are exposing the given error message!
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinRanfomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayersPerRoom }); //? might be accepting json format property vals

        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Not this client is in a room.");
        }
        #endregion

        #region MonoBehaviour Callbacks




        void Awake()
        {
            // Critical - makes sure we can use the PhotoenNetwork.LoadLevel() on the master client and that all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }



        // Start is called before the first frame update
        void Start()
        {
            //Connect(); we dont need this if we are using a button to start this
        }
        #endregion



        #region Public methods

        public void Connect()
        {
            if (PhotonNetwork.IsConnected) //check if connected or not, join random room if we are, else we attempt connect to the Photon Online Server
            {
                PhotonNetwork.JoinRandomRoom(); //if this fails Photon will call OnJoinRandomFailed() RPC, if we implement this we can choose how to handle the response
            }
            else
            {
                // have to connect to Photon Online Server first
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings(); // this method is the starting point to connect to Photon Cloud

            }
        }

        #endregion
    }
}

