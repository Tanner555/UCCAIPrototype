using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RPGPrototype
{
    public class AllyMemberRPG : AllyMember
    {
        #region Fields

        #endregion

        #region Properties
        public override bool bIsCarryingMeleeWeapon => true;

        //TODO: Fix Self: Attack Targetted Enemy False Conditions
        //Only Setting to fix IGBPI Bug, Due to Self: Attack Targetted Enemy False Conditions
        public override int CurrentEquipedAmmo => 200;

        public override int AllyHealth
        {
            get { return allyStatController.Stat_Health; }
            protected set { allyStatController.Stat_Health = value; }
        }

        public override int AllyMaxHealth
        {
            get { return allyStatController.Stat_MaxHealth; }
        }

        public override int AllyStamina
        {
            get { return allyStatController.Stat_Stamina; }
            protected set { allyStatController.Stat_Stamina = value; }
        }

        public override int AllyMaxStamina
        {
            get { return allyStatController.Stat_MaxStamina; }
        }

        public override string CharacterName
        {
            get
            {
                return allyStatController.Stat_CharacterName;
            }
        }

        public override ECharacterType CharacterType
        {
            get
            {
                return allyStatController.Stat_CharacterType;
            }
        }

        public override Sprite CharacterPortrait
        {
            get
            {
                return allyStatController.Stat_CharacterPortrait;
            }
        }

        //public AllyMemberWrapper enemyTargetWrapper
        //{
        //    get { return aiControllerWrapper.currentTargettedEnemyWrapper; }
        //}

        #endregion

        #region Health
        public float healthAsPercentage { get { return AllyHealth / AllyMaxHealth; } }

        #endregion

        #region UnityMessages
        protected override void OnDelayStart()
        {
            base.OnDelayStart();

        }
        #endregion

        #region Handlers
        protected override void InitializeAlly(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            AllyFaction = _specific.AllyFaction;
            GeneralCommander = _specific.GeneralCommander;
            if (_allFields.bBuildLOSChildObject)
            {
                LOSTransform = _specific.LOSChildObjectTransform;
            }

            if (gamemode == null)
            {
                Debug.LogError("No gamemode on ai player!");
            }

            //Adding To PartyMan Is Delayed Because AllyStatController
            //Is Not Added Before AllyMember
            StartCoroutine(InitializeAllyCoroutine());
            //Create Overrideable Late Start to Accommodate 
            //Assets Having Long StartUp 
            Invoke("OnDelayStart", 0.5f);
        }

        IEnumerator InitializeAllyCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            //Add Ally to PartyMembers Rather than Finding them
            //To Make Spawning Easier
            if (partyManager != null) partyManager.AddPartyMember(this);

            StartServices();
        }

        #endregion

        #region Getters
        public override int GetDamageRate()
        {
            return allyStatController.CalculateDamageRate();
        }
        #endregion
    }
}