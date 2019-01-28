using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace RTSCoreFramework
{
    public class RTSCharacterStatsMonitor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Properties
        bool AllCompsAreValid
        {
            get
            {
                return UiCharacterPortrait && UiHealthSlider && UiAbilitySlider
                  && CurrentHealthText && MaxHealthText && CurrentAbilityText
                  && MaxAbilityText && CharacterNameText && UiCharacterPortraitPanel;
            }
        }

        //UiTarget Props
        AllyEventHandler uiTargetHandler
        {
            get { return uiTarget.allyEventHandler; }
        }

        public bool bUiTargetIsSet { get; protected set; }

        //Easy Access Image Props
        Image StatPanelImage
        {
            get
            {
                if (_StatPanelImage == null)
                    _StatPanelImage = GetComponent<Image>();

                return _StatPanelImage;
            }
        }
        Image _StatPanelImage = null;

        Image PortraitPanelImage
        {
            get
            {
                if (_PortraitPanelImage == null)
                    _PortraitPanelImage = UiCharacterPortraitPanel.GetComponent<Image>();

                return _PortraitPanelImage;
            }
        }
        Image _PortraitPanelImage = null;

        Slider UiHealthSliderComponent
        {
            get
            {
                if (_UiHealthSliderComponent == null)
                    _UiHealthSliderComponent = UiHealthSlider.GetComponent<Slider>();

                return _UiHealthSliderComponent;
            }
        }
        private Slider _UiHealthSliderComponent = null;

        Slider UiAbilitySliderComponent
        {
            get
            {
                if (_UiAbilitySliderComponent == null)
                    _UiAbilitySliderComponent = UiAbilitySlider.GetComponent<Slider>();

                return _UiAbilitySliderComponent;
            }
        }
        private Slider _UiAbilitySliderComponent = null;

        Slider UiActiveTimeSliderComponent
        {
            get
            {
                if (_UiActiveTimeSliderComponent == null)
                    _UiActiveTimeSliderComponent = UiActiveTimeSlider.GetComponent<Slider>();

                return _UiActiveTimeSliderComponent;
            }
        }
        private Slider _UiActiveTimeSliderComponent = null;
        #endregion

        #region Fields
        //Ui Gameobjects
        [Header("Ui GameObjects")]
        [SerializeField]
        Image UiCharacterPortraitPanel;
        [SerializeField]
        Image UiCharacterPortrait;
        [SerializeField]
        GameObject UiHealthSlider;
        [SerializeField]
        GameObject UiAbilitySlider;
        [SerializeField]
        GameObject UiActiveTimeSlider;
        [SerializeField]
        TextMeshProUGUI CurrentHealthText;
        [SerializeField]
        TextMeshProUGUI MaxHealthText;
        [SerializeField]
        TextMeshProUGUI CurrentAbilityText;
        [SerializeField]
        TextMeshProUGUI MaxAbilityText;
        [SerializeField]
        TextMeshProUGUI CharacterNameText;
        //Ui Target Info
        AllyMember uiTarget = null;
        //Colors
        [Header("Colors")]
        [SerializeField] Color HighlightColor;
        [SerializeField] Color SelectedColor;
        //Not Serialized Colors Used to Reference Start Color
        Color NormalStatPanelColor;
        Color NormalPortraitPanelColor;
        //hover info fields
        bool bIsHighlighted = false;
        float updateActiveTimeSliderRate = 0.2f;
        #endregion

        #region UnityMessages
        private void OnEnable()
        {
            if (AllCompsAreValid)
            {
                NormalStatPanelColor = StatPanelImage.color;
                NormalPortraitPanelColor = PortraitPanelImage.color;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (uiTarget != null &&
                uiTargetHandler && !uiTarget.bIsCurrentPlayer)
            {
                uiTargetHandler.CallEventOnHoverOver();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (uiTarget != null && uiTargetHandler &&
                !uiTarget.bIsCurrentPlayer)
            {
                uiTargetHandler.CallEventOnHoverLeave();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool _leftClick = eventData.button ==
                PointerEventData.InputButton.Left;
            if (_leftClick && uiTarget != null && uiTargetHandler &&
                !uiTarget.bIsCurrentPlayer &&
                uiTarget.partyManager != null)
            {
                uiTarget.partyManager.SetAllyInCommand(uiTarget);
                uiTargetHandler.CallEventOnHoverLeave();
            }
        }
        #endregion

        #region HookingUiTarget-Initialization
        public void HookAllyCharacter(AllyMember _targetToSet)
        {
            if (AllCompsAreValid == false)
            {
                Debug.LogError(@"Cannot Hook Character Because
                Not All Components Are Set In The Inspector");
                return;
            }
            if (_targetToSet != null)
            {
                SetupUITargetHandlers(uiTarget, _targetToSet);
                uiTarget = _targetToSet;
                //Set Character Portrait
                if (uiTarget.CharacterPortrait != null)
                {
                    UiCharacterPortrait.sprite = uiTarget.CharacterPortrait;
                }
                if (uiTarget.bIsCurrentPlayer)
                {
                    SetToSelectedColor();
                }
            }
        }

        protected virtual void SetupUITargetHandlers(AllyMember _previousTarget, AllyMember _currentTarget)
        {
            if (_previousTarget != null && _previousTarget.bAllyIsUiTarget)
            {
                UnsubscribeFromUiTargetHandlers(_previousTarget);
            }
            if (_currentTarget != null && !_currentTarget.bAllyIsUiTarget)
            {
                SubscribeToUiTargetHandlers(_currentTarget);
                TransferCharacterStatsToText(_currentTarget);
                StartServices();
            }
        }

        protected virtual void TransferCharacterStatsToText(AllyMember _ally)
        {
            CharacterNameText.text = _ally.CharacterName;
        }
        #endregion

        #region HookingUiTarget-Subscribe/Desubscribe
        protected virtual void SubscribeToUiTargetHandlers(AllyMember _target)
        {
            if (_target == null) return;
            var _handler = _target.allyEventHandler;
            SetAllyIsUiTarget(_target, true);
            //Sub to Current UiTarget Handlers
            _handler.OnHealthChanged += UiTargetHandle_OnHealthChanged;
            _handler.OnStaminaChanged += UiTargetHandle_OnStaminaChanged;
            _handler.OnActiveTimeChanged += UiTargetHandle_OnActiveTimeChanged;
            _handler.EventAllyDied += UiTargetHandle_OnAllyDeath;
            _handler.EventSetAsCommander += UiTargetHandle_SetAsCommander;
            _handler.EventSwitchingFromCom += UiTargetHandle_SwitchFromCommander;
            _handler.OnHoverOver += UiTargetHandle_OnHoverOver;
            _handler.OnHoverLeave += UiTargetHandle_OnHoverLeave;
            //Notify Character Actions
            _handler.EventCommandAttackEnemy += UiTargetHandle_Attacking;
            _handler.OnTryUseWeapon += UiTargetHandle_Attacking;
            _handler.EventStopTargettingEnemy += UiTargetHandle_Nothing;
            _handler.EventCommandMove += UiTargetHandle_CommandMove;
            _handler.EventFinishedMoving += UiTargetHandle_Nothing;
        }

        protected virtual void UnsubscribeFromUiTargetHandlers(AllyMember _target)
        {
            if (_target == null) return;
            var _handler = _target.allyEventHandler;
            SetAllyIsUiTarget(_target, false);
            //Unsub From Previous UiTarget Handlers
            _handler.OnHealthChanged -= UiTargetHandle_OnHealthChanged;
            _handler.OnStaminaChanged -= UiTargetHandle_OnStaminaChanged;
            _handler.OnActiveTimeChanged -= UiTargetHandle_OnActiveTimeChanged;
            _handler.EventAllyDied -= UiTargetHandle_OnAllyDeath;
            _handler.EventSetAsCommander -= UiTargetHandle_SetAsCommander;
            _handler.EventSwitchingFromCom -= UiTargetHandle_SwitchFromCommander;
            _handler.OnHoverOver -= UiTargetHandle_OnHoverOver;
            _handler.OnHoverLeave -= UiTargetHandle_OnHoverLeave;
            //Notify Character Actions
            _handler.EventCommandAttackEnemy -= UiTargetHandle_Attacking;
            _handler.OnTryUseWeapon -= UiTargetHandle_Attacking;
            _handler.EventStopTargettingEnemy -= UiTargetHandle_Nothing;
            _handler.EventCommandMove -= UiTargetHandle_CommandMove;
            _handler.EventFinishedMoving -= UiTargetHandle_Nothing;
        }

        void SetAllyIsUiTarget(AllyMember _target, bool _isTarget)
        {
            if (_target == null) return;
            var _handler = _target.allyEventHandler;
            _handler.SetAllyIsUiTarget(_isTarget);
            bUiTargetIsSet = _isTarget;
        }
        #endregion

        #region UITargetHandlers
        protected virtual void UiTargetHandle_OnHealthChanged(int _current, int _max)
        {
            if (AllCompsAreValid == false) return;
            CurrentHealthText.text = _current.ToString();
            MaxHealthText.text = _max.ToString();
            Slider _slider = UiHealthSliderComponent;
            if (_slider != null)
            {
                _slider.maxValue = _max;
                _slider.value = _current;
            }
        }

        protected virtual void UiTargetHandle_OnStaminaChanged(int _current, int _max)
        {
            if (AllCompsAreValid == false) return;
            CurrentAbilityText.text = _current.ToString();
            MaxAbilityText.text = _max.ToString();
            Slider _slider = UiAbilitySliderComponent;
            if (_slider != null)
            {
                _slider.maxValue = _max;
                _slider.value = _current;
            }
        }

        protected virtual void UiTargetHandle_OnActiveTimeChanged(int _current, int _max)
        {
            if (AllCompsAreValid == false) return;
            Slider _slider = UiActiveTimeSliderComponent;
            if (_slider != null)
            {
                _slider.maxValue = _max;
                _slider.value = _current;
            }
        }

        protected virtual void UiTargetHandle_OnAllyDeath()
        {
            if (uiTarget != null && uiTarget.bAllyIsUiTarget)
            {
                UnsubscribeFromUiTargetHandlers(uiTarget);
                StopServices();
            }
        }

        protected virtual void UiTargetHandle_SetAsCommander()
        {
            SetToSelectedColor();
        }

        protected virtual void UiTargetHandle_SwitchFromCommander()
        {
            SetToNormalColor();
        }

        protected virtual void UiTargetHandle_OnHoverOver()
        {
            if (bIsHighlighted == false && uiTarget != null &&
                !uiTarget.bIsCurrentPlayer)
            {
                SetToHighlightColor();
            }
        }

        protected virtual void UiTargetHandle_OnHoverLeave()
        {
            if (bIsHighlighted && uiTarget != null &&
                !uiTarget.bIsCurrentPlayer)
            {
                SetToNormalColor();
            }
        }

        //Notify Character Actions
        protected virtual void UiTargetHandle_Attacking(AllyMember ally)
        {
            CharacterNameText.text = $"{uiTarget.CharacterName}: Attacking";
        }

        protected virtual void UiTargetHandle_Attacking()
        {
            CharacterNameText.text = $"{uiTarget.CharacterName}: Attacking";
        }

        protected virtual void UiTargetHandle_CommandMove(Vector3 _location)
        {
            if (uiTargetHandler.bIsAttacking) return;
            CharacterNameText.text = $"{uiTarget.CharacterName}: Moving";
        }

        protected virtual void UiTargetHandle_Nothing()
        {
            //Only Set to Nothing if Ally is Not Attacking
            if (uiTargetHandler.bIsAttacking) return;
            CharacterNameText.text = $"{uiTarget.CharacterName}";
        }
        #endregion

        #region Services
        protected virtual void StartServices()
        {

        }

        protected virtual void StopServices()
        {
            
        }
        #endregion

        #region Helpers
        protected virtual void SetToHighlightColor()
        {
            if (AllCompsAreValid && PortraitPanelImage != null)
            {
                bIsHighlighted = true;
                PortraitPanelImage.color = HighlightColor;
            }
        }

        protected virtual void SetToSelectedColor()
        {
            if (AllCompsAreValid && PortraitPanelImage != null)
            {
                bIsHighlighted = false;
                PortraitPanelImage.color = SelectedColor;
            }
        }

        protected virtual void SetToNormalColor()
        {
            if (AllCompsAreValid && PortraitPanelImage != null)
            {
                bIsHighlighted = false;
                PortraitPanelImage.color = NormalPortraitPanelColor;
            }
        }
        #endregion
    }
}