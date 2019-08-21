
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviour
    {
        #region Private Serializable Fields


        #endregion

        #region Private Fields

        // Client's version number -- we separate users from each other bty gameVersion based on "gamebreaking" version features
        string gameVersion = "1"; //re: see Semantic Versioning for versioning choice and connotations

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
            Connect();
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

