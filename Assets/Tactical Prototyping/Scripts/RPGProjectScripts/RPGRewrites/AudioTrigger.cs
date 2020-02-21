﻿using UnityEngine;
using BaseFramework;
using RTSCoreFramework;

namespace RPGPrototype
{
    public class AudioTrigger : MonoBehaviour
    {
        #region SerializedFields
        // Serialized
        [SerializeField] AudioClip clip;
        [SerializeField] int layerFilter = 11;
        [SerializeField] float playerDistanceThreshold = 5f;
        [SerializeField] bool isOneTimeOnly = true;
        #endregion

        #region OtherFields
        // Private members
        bool hasPlayed = false;
        AudioSource audioSource;
        float repeatRate = 0.2f;

        GameObject player;
        #endregion

        #region Properties
        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }
        #endregion

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = clip;
        }

        void SE_CheckForPlay()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= playerDistanceThreshold)
            {
                RequestPlayAudioClip();
            }
        }

        private void OnEnable()
        {
            gamemaster.OnAllySwitch += OnAllySwitch;
        }

        private void OnDisable()
        {
            gamemaster.OnAllySwitch -= OnAllySwitch;

            if (IsInvoking("SE_CheckForPlay"))
            {
                CancelInvoke("SE_CheckForPlay");
            }
        }

        void RequestPlayAudioClip()
        {
            if (isOneTimeOnly && hasPlayed)
            {
                if (IsInvoking("SE_CheckForPlay"))
                {
                    CancelInvoke("SE_CheckForPlay");
                }
                return;
            }
            else if (audioSource.isPlaying == false)
            {
                audioSource.Play();
                hasPlayed = true;
            }
        }

        void OnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            if (_party.bIsCurrentPlayerCommander)
            {
                player = _toSet.gameObject;
                if(IsInvoking("SE_CheckForPlay") == false) { 
                    InvokeRepeating("SE_CheckForPlay", 0.1f, repeatRate);
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 255f, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, playerDistanceThreshold);
        }
    }
}