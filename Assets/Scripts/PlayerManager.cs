using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;
        #endregion

        #region Private Fields

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        // True, when user is firing
        bool IsFiring;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
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
                ProcessInputs();
            }

            // trigger beams acrive state
            if (beams != null && IsFiring != beams.activeSelf)
            {
                beams.SetActive(IsFiring);
            }
        }

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


    }

}
