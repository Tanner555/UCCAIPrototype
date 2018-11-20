using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using BaseFramework;

namespace RTSCoreFramework
{
    public class RTSGameMaster : GameMaster
    {
        #region Properties
        public RTSCamRaycaster rayCaster { get { return RTSCamRaycaster.thisInstance; } }

        #endregion

        #region OverrideAndHideProperties
        new public RTSGameInstance gameInstance
        {
            get { return RTSGameInstance.thisInstance; }
        }

        new public RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        new RTSUiMaster uiMaster
        {
            get { return RTSUiMaster.thisInstance; }
        }

        new public static RTSGameMaster thisInstance
        {
            get { return GameMaster.thisInstance as RTSGameMaster; }
        }
        #endregion

        #region Fields
        public bool isInventoryUIOn;

        #endregion

        #region UnityMessages
        protected override void OnEnable()
        {
            base.OnEnable();

        }

        protected override void Start()
        {
            base.Start();

        }

        protected override void OnDisable()
        {
            base.OnDisable();

        }

        protected override void Update()
        {
            base.Update();
        }
        #endregion

        #region Events and Delegates
        public event GameManagerEventHandler EventAllEnemiesAreDead;
        public event GameManagerEventHandler EventUpdateCharacterStats;
        //public event OneBoolArgsHandler EventEnableSelectionBox;

        //Used by CamRaycaster to broadcast mouse hit type
        public delegate void RtsHitTypeAndRayCastHitHandler(rtsHitType hitType, RaycastHit hit);
        public event RtsHitTypeAndRayCastHitHandler OnMouseCursorChange;
        public event RtsHitTypeAndRayCastHitHandler OnLeftClickSendHit;
        public event RtsHitTypeAndRayCastHitHandler OnRightClickSendHit;

        public delegate void AllyMemberHandler(AllyMember ally);
        //public event AllyMemberHandler OnHoverOverAlly;
        //public event AllyMemberHandler OnHoverOverEnemy;
        //public event AllyMemberHandler OnHoverLeaveAlly;
        //public event AllyMemberHandler OnHoverLeaveEnemy;
        public event AllyMemberHandler OnLeftClickAlly;
        public event AllyMemberHandler OnLeftClickEnemy;
        public event AllyMemberHandler OnRightClickAlly;
        public event AllyMemberHandler OnRightClickEnemy;

        public delegate void AllySwitchHandler(PartyManager _party, AllyMember _toSet, AllyMember _current);
        public event AllySwitchHandler OnAllySwitch;

        public delegate void UiTargetHookHandler(AllyMember _target, AllyEventHandler _eventHandler, PartyManager _party);
        public event UiTargetHookHandler OnRegisterUiTarget;
        public event UiTargetHookHandler OnDeregisterUiTarget;

        #endregion

        #region EventCalls      
        public override void CallEventGameOver()
        {
            EnableRayCaster(false);
            base.CallEventGameOver();
        }

        public virtual void CallEventUpdateCharacterStats()
        {
            if (EventUpdateCharacterStats != null) EventUpdateCharacterStats();
        }

        public void CallEventAllEnemiesAreDead()
        {
            if (EventAllEnemiesAreDead != null) EventAllEnemiesAreDead();
        }

        public override void CallEventAllObjectivesCompleted()
        {
            EnableRayCaster(false);
            base.CallEventAllObjectivesCompleted();
        }

        /// <summary>
        /// Called Before The AllyInCommand has been set by RTSGameMaster
        /// </summary>
        /// <param name="_party"></param>
        /// <param name="_toSet"></param>
        /// <param name="_current"></param>
        public void CallOnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            if (OnAllySwitch != null)
                OnAllySwitch(_party, _toSet, _current);
        }

        public void CallOnRegisterUiTarget(AllyMember _target, AllyEventHandler _eventHandler, PartyManager _party)
        {
            if (OnRegisterUiTarget != null)
                OnRegisterUiTarget(_target, _eventHandler, _party);
        }

        public void CallOnDeregisterUiTarget(AllyMember _target, AllyEventHandler _eventHandler, PartyManager _party)
        {
            if (OnDeregisterUiTarget != null)
                OnDeregisterUiTarget(_target, _eventHandler, _party);
        }
        #endregion

        #region EventCalls-MouseCursorAndClickHandlers
        public void CallEventOnMouseCursorChange(rtsHitType hitType, RaycastHit hit)
        {
            bool isNull = hit.collider == null || hit.collider.gameObject == null ||
            hit.collider.gameObject.transform.root.gameObject == null;
            if (isNull) hitType = rtsHitType.Unknown;
            if (OnMouseCursorChange != null)
            {
                OnMouseCursorChange(hitType, hit);
            }

            bool _notAlly = hitType != rtsHitType.Ally && hitType != rtsHitType.Enemy;

            if (gamemode.hasPrevHighAlly && _notAlly)
            {
                gamemode.hasPrevHighAlly = false;
                //TODO: RTSPrototype See if OnMouseCursor Change Should Return if PrevHighAlly is Null
                if (gamemode.prevHighAlly == null) return;
                //if (OnHoverLeaveAlly != null) OnHoverLeaveAlly(gamemode.prevHighAlly);
                gamemode.prevHighAlly.allyEventHandler.CallEventOnHoverLeave(hitType, hit);
            }

            GameObject hitObjectRoot = null;
            if (hitType != rtsHitType.Unknown)
            {
                hitObjectRoot = hit.collider.gameObject.transform.root.gameObject;
            }

            switch (hitType)
            {
                case rtsHitType.Ally:
                    AllyMember _ally = hitObjectRoot.GetComponent<AllyMember>();
                    if (_ally == null) return;
                    gamemode.hasPrevHighAlly = true;
                    //if (OnHoverOverAlly != null) OnHoverOverAlly(_ally);
                    _ally.allyEventHandler.CallEventOnHoverOver(hitType, hit);
                    gamemode.prevHighAlly = _ally;
                    break;
                case rtsHitType.Enemy:
                    AllyMember _enemy = hitObjectRoot.GetComponent<AllyMember>();
                    if (_enemy == null) return;
                    gamemode.hasPrevHighAlly = true;
                    //if (OnHoverOverAlly != null) OnHoverOverAlly(_enemy);
                    _enemy.allyEventHandler.CallEventOnHoverOver(hitType, hit);
                    gamemode.prevHighAlly = _enemy;
                    break;
                case rtsHitType.Cover:
                    break;
                case rtsHitType.Walkable:
                    break;
                case rtsHitType.Unwalkable:
                    break;
                case rtsHitType.Unknown:
                    break;
                default:
                    break;
            }
        }

        public override void CallEventOnLeftClick()
        {
            base.CallEventOnLeftClick();
            if (rayCaster != null && rayCaster.enabled == true &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                var _info = rayCaster.GetRaycastInfo();
                CallEventOnLeftClickSendHit(_info._hitType, _info._rayHit);
            }
        }

        private void CallEventOnLeftClickSendHit(rtsHitType hitType, RaycastHit hit)
        {
            bool isNull = hit.collider == null || hit.collider.gameObject == null ||
            hit.collider.gameObject.transform.root.gameObject == null;
            if (isNull) hitType = rtsHitType.Unknown;
            if (OnLeftClickSendHit != null)
            {
                OnLeftClickSendHit(hitType, hit);
            }
            GameObject hitObjectRoot = null;
            if (hitType != rtsHitType.Unknown)
            {
                hitObjectRoot = hit.collider.gameObject.transform.root.gameObject;
            }

            switch (hitType)
            {
                case rtsHitType.Ally:
                    AllyMember _ally = hitObjectRoot.GetComponent<AllyMember>();
                    if (_ally == null) return;
                    if (OnLeftClickAlly != null) OnLeftClickAlly(_ally);
                    break;
                case rtsHitType.Enemy:
                    AllyMember _enemy = hitObjectRoot.GetComponent<AllyMember>();
                    if (_enemy == null) return;
                    if (OnLeftClickEnemy != null) OnLeftClickEnemy(_enemy);
                    break;
                case rtsHitType.Cover:
                    break;
                case rtsHitType.Walkable:
                    break;
                case rtsHitType.Unwalkable:
                    break;
                case rtsHitType.Unknown:
                    break;
                default:
                    break;
            }
        }

        public override void CallEventOnRightClick()
        {
            base.CallEventOnRightClick();
            if (rayCaster != null && rayCaster.enabled == true &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                var _info = rayCaster.GetRaycastInfo();
                CallEventOnRightClickSendHit(_info._hitType, _info._rayHit);
            }
        }

        private void CallEventOnRightClickSendHit(rtsHitType hitType, RaycastHit hit)
        {
            bool isNull = hit.collider == null || hit.collider.gameObject == null ||
            hit.collider.gameObject.transform.root.gameObject == null;
            if (isNull) hitType = rtsHitType.Unknown;
            if (OnRightClickSendHit != null)
            {
                OnRightClickSendHit(hitType, hit);
            }
            GameObject hitObjectRoot = null;
            if (hitType != rtsHitType.Unknown)
            {
                hitObjectRoot = hit.collider.gameObject.transform.root.gameObject;
            }

            switch (hitType)
            {
                case rtsHitType.Ally:
                    AllyMember _ally = hitObjectRoot.GetComponent<AllyMember>();
                    if (_ally == null) return;
                    if (OnRightClickAlly != null) OnRightClickAlly(_ally);
                    break;
                case rtsHitType.Enemy:
                    AllyMember _enemy = hitObjectRoot.GetComponent<AllyMember>();
                    if (_enemy == null) return;
                    if (OnRightClickEnemy != null) OnRightClickEnemy(_enemy);
                    break;
                case rtsHitType.Cover:
                    break;
                case rtsHitType.Walkable:
                    break;
                case rtsHitType.Unwalkable:
                    break;
                case rtsHitType.Unknown:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Handlers
        void HandleAnyUIToggle(bool _enable)
        {
            CallOnToggleIsGamePaused(_enable);
        }
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            uiMaster.EventAnyUIToggle += HandleAnyUIToggle;
        }

        protected override void UnsubFromEvents()
        {
            base.UnsubFromEvents();
            uiMaster.EventAnyUIToggle -= HandleAnyUIToggle;
        }
        #endregion

        #region Helpers
        void EnableRayCaster(bool _enable)
        {
            if (rayCaster != null) rayCaster.enabled = _enable;
        }
        #endregion

    }
}