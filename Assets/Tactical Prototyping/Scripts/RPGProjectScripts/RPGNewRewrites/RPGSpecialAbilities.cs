using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;
using RTSCoreFramework;

namespace RPGPrototype.OLDAbilities
{
    public class RPGSpecialAbilities : MonoBehaviour
    {
        #region Properties
        RPGGameMaster gamemaster
        {
            get { return RPGGameMaster.thisInstance; }
        }

        AllyEventHandlerRPG eventhandler
        {
            get
            {
                if (_eventhandler == null)
                    _eventhandler = GetComponent<AllyEventHandlerRPG>();

                return _eventhandler;
            }
        }
        AllyEventHandlerRPG _eventhandler = null;
        AllyMemberRPG allymember
        {
            get
            {
                if (_allymember == null)
                    _allymember = GetComponent<AllyMemberRPG>();

                return _allymember;
            }
        }
        AllyMemberRPG _allymember = null;

        bool bIsDead
        {
            get
            {
                return allymember == null ||
                allymember.IsAlive == false;
            }
        }

        //Stamina
        int AllyStamina
        {
            get { return allymember.AllyStamina; }
        }
        int AllyMaxStamina
        {
            get { return allymember.AllyMaxStamina; }
        }
        float energyAsPercent { get { return AllyStamina / AllyMaxStamina; } }
        #endregion

        #region Fields
        [SerializeField] AbilityConfigOLD[] abilities;
        [SerializeField] Image energyBar;
        //Once per second
        float addStaminaRepeatRate = 1f;
        int regenPointsPerSecond = 10;
        [SerializeField] AudioClip outOfEnergy;

        AudioSource audioSource;

        /// <summary>
        /// Allows me to store a behavior on this script
        /// instead of depending on the config for behavior reference
        /// </summary>
        Dictionary<AbilityConfigOLD, AbilityBehaviourOLD> AbilityDictionary = new Dictionary<AbilityConfigOLD, AbilityBehaviourOLD>();
        #endregion

        #region UnityMessages
        // Use this for initialization
        private void OnEnable()
        {
            eventhandler.EventAllyDied += OnAllyDeath;
            eventhandler.InitializeAllyComponents += OnInitializeAllyComponents;
            gamemaster.OnNumberKeyPress += OnKeyPress;
        }

        private void OnDisable()
        {
            eventhandler.EventAllyDied -= OnAllyDeath;
            eventhandler.InitializeAllyComponents -= OnInitializeAllyComponents;
            gamemaster.OnNumberKeyPress -= OnKeyPress;
        }
        #endregion

        #region AbilitiesAndEnergy
        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyComponent = GetComponent<RPGSpecialAbilities>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyCost <= AllyStamina)
            {
                ConsumeEnergy(energyCost);
                AbilityDictionary[abilities[abilityIndex]].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public void ConsumeEnergy(float amount)
        {
            allymember.AllyDrainStamina((int)amount);
        }
        #endregion

        #region Services
        void SE_AddEnergyPoints()
        {
            allymember.AllyRegainStamina(regenPointsPerSecond);
        }
        #endregion

        #region Handlers
        private void OnInitializeAllyComponents(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps)
        {
            var _RPGallAllyComps = (AllyComponentsAllCharacterFieldsRPG)_allAllyComps;
            if (_specificComps.bBuildCharacterCompletely)
            {
                var _rpgCharAttr = _RPGallAllyComps.bUseAStarPath == false ?
                    ((AllyComponentSpecificFieldsRPG)_specificComps).RPGCharacterAttributesObject :
                    ((AllyComponentSpecificFieldsRPG)_specificComps).ASTAR_RPGCharacterAttributesObject;
                this.abilities = _rpgCharAttr.abilities;
                if(_rpgCharAttr.energyBar != null)
                {
                    this.energyBar = _rpgCharAttr.energyBar;
                }
                if(_rpgCharAttr.outOfEnergy != null)
                {
                    this.outOfEnergy = _rpgCharAttr.outOfEnergy;
                }
            }
            audioSource = GetComponent<AudioSource>();
            InitializeAbilityDictionary();
            InvokeRepeating("SE_AddEnergyPoints", 1f, addStaminaRepeatRate);
        }

        void OnKeyPress(int _key)
        {
            if (bIsDead) return;

            if (_key == 0 ||
                _key >= GetNumberOfAbilities() ||
                allymember.bIsCurrentPlayer == false) return;

            AttemptSpecialAbility(_key);
        }

        void OnAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            CancelInvoke();
        }
        #endregion

        #region DictionaryBehavior
        public AbilityBehaviourOLD AddAbilityBehaviorFromConfig(AbilityConfigOLD _config, GameObject objectToattachTo)
        {
            AbilityBehaviourOLD _behaviourComponent = 
                _config.AddBehaviourComponent(objectToattachTo);
            _behaviourComponent.SetConfig(_config);
            return _behaviourComponent;
        }

        void InitializeAbilityDictionary()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                AbilityDictionary.Add(
                    abilities[abilityIndex],
                    AddAbilityBehaviorFromConfig(
                        abilities[abilityIndex], this.gameObject
                    ));
            }
        }
        #endregion
    }
}