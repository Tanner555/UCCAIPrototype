using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace RTSCoreFramework
{
    public class RTSWeaponStatsUI : RTSUITargetRegister, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region UIGameObjects
        [Header("Primary Weapon UI")]
        [SerializeField]
        private GameObject PrimaryWeaponGameObject;
        [SerializeField]
        private Image PrimaryWeaponUIImage;
        [SerializeField]
        private TextMeshProUGUI PrimaryLoadedText;
        [SerializeField]
        private TextMeshProUGUI PrimaryUnloadedText;
        [Header("Secondary Weapon UI")]
        [SerializeField]
        private GameObject SecondaryWeaponGameObject;
        [SerializeField]
        private Image SecondaryWeaponUIImage;
        [SerializeField]
        private TextMeshProUGUI SecondaryLoadedText;
        [SerializeField]
        private TextMeshProUGUI SecondaryUnloadedText;
        #endregion

        #region Fields
        //Colors
        Color currentColor;
        [Header("Highlight Color")]
        [SerializeField]
        Color hoverColor = Color.gray;
        [Header("Equipped/UnEquipped Colors")]
        [SerializeField] Color EquippedColor;
        [SerializeField] Color UnequippedColor;
        //Equip Change Info
        Vector3 EquippedScale = new Vector3(2, 2, 1);
        Vector3 UnequippedScale = new Vector3(1, 1, 1);
        #endregion

        #region Properties
        Image WeaponStatsUiImage
        {
            get
            {
                if (_weaponStatsUiImage == null)
                    _weaponStatsUiImage = GetComponent<Image>();

                return _weaponStatsUiImage;
            }
        }
        Image _weaponStatsUiImage = null;

        bool bIsUiTargetHoldingPrimary
        {
            get { return uiTargetHandler.MyEquippedType == 
                    EEquipType.Primary; }
        }
        #endregion

        #region UnityMessages
        protected override void OnEnable()
        {
            base.OnEnable();
            if (WeaponStatsUiImage != null)
                currentColor = WeaponStatsUiImage.color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (WeaponStatsUiImage != null)
            {
                WeaponStatsUiImage.color = hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (WeaponStatsUiImage != null)
            {
                WeaponStatsUiImage.color = currentColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(currentUiTarget != null && bHasRegisteredTarget)
            {
                currentUiTarget.allyEventHandler.CallToggleEquippedWeapon();
            }
        }
        #endregion

        #region GameMasterHandlers/RegisterUiTarget
        protected override void OnRegisterUiTarget(AllyMember _target, AllyEventHandler _handler, PartyManager _party)
        {
            base.OnRegisterUiTarget(_target, _handler, _party);
            _handler.OnAmmoChanged += OnAmmoChanged;
            _handler.OnWeaponChanged += OnWeaponChanged;
            UpdateWeaponUiGameObjects(uiTargetHandler.MyEquippedType);
        }

        protected override void OnDeregisterUiTarget(AllyMember _target, AllyEventHandler _handler)
        {
            base.OnDeregisterUiTarget(_target, _handler);
            _handler.OnAmmoChanged -= OnAmmoChanged;
            _handler.OnWeaponChanged -= OnWeaponChanged;
        }

        #endregion

        #region UiTargetHandlers
        void OnAmmoChanged(int _loaded, int _max)
        {
            if (currentUiTarget == null || 
                uiTargetHandler == null || 
                bHasRegisteredTarget == false) return;

            PrimaryLoadedText.text = uiTargetHandler.PrimaryLoadedAmmoAmount.ToString();
            PrimaryUnloadedText.text = uiTargetHandler.PrimaryUnloadedAmmoAmount.ToString();
            SecondaryLoadedText.text = uiTargetHandler.SecondaryLoadedAmmoAmount.ToString();
            SecondaryUnloadedText.text = uiTargetHandler.SecondaryUnloadedAmmoAmount.ToString();
        }

        void OnWeaponChanged(EEquipType _eType, EWeaponType _weaponType, EWeaponUsage _wUsage, bool _equipped)
        {
            if (_equipped)
            {
                UpdateWeaponUiGameObjects(_eType);
            }
        }
        #endregion

        #region HelperMethods
        void UpdateWeaponUiGameObjects(EEquipType _eType)
        {
            Sprite _equippedIcon = uiTargetHandler.MyEquippedWeaponIcon;
            Sprite _unequippedIcon = uiTargetHandler.MyUnequippedWeaponIcon;
            if (_eType == EEquipType.Primary)
            {
                PrimaryWeaponGameObject.transform.SetSiblingIndex(1);
                SecondaryWeaponGameObject.transform.SetSiblingIndex(0);
                PrimaryWeaponGameObject.transform.localScale = EquippedScale;
                SecondaryWeaponGameObject.transform.localScale = UnequippedScale;

                if (_equippedIcon && _unequippedIcon)
                {
                    PrimaryWeaponUIImage.sprite = _equippedIcon;
                    PrimaryWeaponUIImage.color = EquippedColor;
                    SecondaryWeaponUIImage.sprite = _unequippedIcon;
                    SecondaryWeaponUIImage.color = UnequippedColor;
                }
            }
            else
            {
                PrimaryWeaponGameObject.transform.SetSiblingIndex(0);
                SecondaryWeaponGameObject.transform.SetSiblingIndex(1);
                PrimaryWeaponGameObject.transform.localScale = UnequippedScale;
                SecondaryWeaponGameObject.transform.localScale = EquippedScale;
                if (_equippedIcon && _unequippedIcon)
                {
                    SecondaryWeaponUIImage.sprite = _equippedIcon;
                    SecondaryWeaponUIImage.color = EquippedColor;
                    PrimaryWeaponUIImage.sprite = _unequippedIcon;
                    PrimaryWeaponUIImage.color = UnequippedColor;
                }
            }
            
        }
        #endregion

    }
}