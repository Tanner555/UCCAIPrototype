using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSCoreFramework
{
    public class AllySpecialAbilities : MonoBehaviour
    {
        #region Properties
        protected RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        protected AllyEventHandler eventhandler
        {
            get
            {
                if (_eventhandler == null)
                    _eventhandler = GetComponent<AllyEventHandler>();

                return _eventhandler;
            }
        }
        AllyEventHandler _eventhandler = null;
        protected AllyMember allymember
        {
            get
            {
                if (_allymember == null)
                    _allymember = GetComponent<AllyMember>();

                return _allymember;
            }
        }
        AllyMember _allymember = null;

        protected virtual bool bIsDead
        {
            get
            {
                return allymember == null ||
                allymember.IsAlive == false;
            }
        }

        //Stamina
        protected virtual int AllyStamina
        {
            get { return allymember.AllyStamina; }
        }
        protected virtual int AllyMaxStamina
        {
            get { return allymember.AllyMaxStamina; }
        }
        protected virtual float energyAsPercent { get { return AllyStamina / AllyMaxStamina; } }

        //AudioSource
        protected virtual AudioSource audioSource
        {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                    if (_audioSource == null)
                        _audioSource = gameObject.AddComponent<AudioSource>();

                }
                return _audioSource;
            }
        }
        private AudioSource _audioSource = null;
        #endregion

        #region Fields
        [SerializeField] public AbilityConfig[] abilities;
        //Once per second
        protected float addStaminaRepeatRate = 1f;
        protected int regenPointsPerSecond = 10;
        [SerializeField] public AudioClip outOfEnergy;
        
        /// <summary>
        /// Allows me to store a behavior on this script
        /// instead of depending on the config for behavior reference
        /// </summary>
        protected Dictionary<AbilityConfig, AbilityBehaviour> AbilityDictionary = new Dictionary<AbilityConfig, AbilityBehaviour>();
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            eventhandler.EventAllyDied += OnAllyDeath;
            eventhandler.OnTrySpecialAbility += HandleOnTrySpecialAbility;
            eventhandler.InitializeAllyComponents += InitializeSpecialAbilties;
            gamemaster.OnNumberKeyPress += OnKeyPress;
        }

        protected virtual void OnDisable()
        {
            eventhandler.EventAllyDied -= OnAllyDeath;
            eventhandler.OnTrySpecialAbility -= HandleOnTrySpecialAbility;
            eventhandler.InitializeAllyComponents -= InitializeSpecialAbilties;
            gamemaster.OnNumberKeyPress -= OnKeyPress;
        }
        #endregion

        #region AbilitiesAndEnergy
        public virtual void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var _config = abilities[abilityIndex];
            var _behaviour = AbilityDictionary[_config];
            //Getting Ready To Queue Action
            var energyCost = _config.GetEnergyCost();
            if (energyCost <= AllyStamina &&
                _behaviour.CanUseAbility())
            {
                eventhandler.CallOnAddActionItemToQueue(
                    new RTSActionItem(
                        _ally => AttemptSpecialAbility(_config, _behaviour), 
                        _ally => energyCost <= AllyStamina && _behaviour.CanUseAbility(),
                        ActionFilters.Abilities, true, true, true, false, _ally => true,
                        _ally => true, _ally => { }
                    ));
            }
        }

        public virtual void AttemptSpecialAbility(AbilityConfig _config, AbilityBehaviour _behaviour, GameObject target = null)
        {
            var energyCost = _config.GetEnergyCost();

            if (energyCost <= AllyStamina)
            {
                if (_behaviour.CanUseAbility())
                {
                    eventhandler.CallOnActiveTimeBarDepletion();
                    ConsumeEnergy(energyCost);
                    _behaviour.Use(target);
                }
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public virtual int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public virtual void ConsumeEnergy(float amount)
        {
            allymember.AllyDrainStamina((int)amount);
        }
        #endregion

        #region Services
        protected virtual void SE_AddEnergyPoints()
        {
            allymember.AllyRegainStamina(regenPointsPerSecond);
        }
        #endregion

        #region Handlers
        protected virtual void InitializeSpecialAbilties(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            abilities = _allFields.specialAbilitiesArray;
            outOfEnergy = _allFields.outOfEnergySoundClip;
            InitializeAbilityDictionary();
            InvokeRepeating("SE_AddEnergyPoints", 1f, addStaminaRepeatRate);
        }

        protected virtual void HandleOnTrySpecialAbility(System.Type _type)
        {
            foreach (var _config in AbilityDictionary.Keys)
            {
                if(_config.GetType().Equals(_type))
                {
                    AttemptSpecialAbility(_config, AbilityDictionary[_config]);
                }
            }
        }

        protected virtual void OnKeyPress(int _key)
        {
            if (bIsDead) return;

            //This fixes a bug with retrieving the wrong ability index
            int _index = _key - 1;
            if (_index < 0 ||
                _index > GetNumberOfAbilities() - 1 ||
                allymember.bIsCurrentPlayer == false) return;

            AttemptSpecialAbility(_index);
        }

        protected virtual void OnAllyDeath()
        {
            CancelInvoke();
        }
        #endregion

        #region TemporaryCode
        /// <summary>
        /// Temporarily Used to Mute Errors relating to animation events.
        /// </summary>
        public void Hit()
        {

        }
        #endregion

        #region DictionaryBehavior
        public virtual AbilityBehaviour AddAbilityBehaviorFromConfig(AbilityConfig _config, GameObject objectToattachTo)
        {
            AbilityBehaviour _behaviourComponent =
                _config.AddBehaviourComponent(objectToattachTo);
            _behaviourComponent.SetConfig(_config);
            return _behaviourComponent;
        }

        protected virtual void InitializeAbilityDictionary()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                AbilityDictionary.Add(
                    abilities[abilityIndex],
                    AddAbilityBehaviorFromConfig(
                        abilities[abilityIndex], this.gameObject
                    ));
            }
            allymember.UpdateAbilityDictionary(AbilityDictionary);
        }
        #endregion
    }
}