using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;

namespace RTSCoreFramework
{
    #region RTSTPCWeaponPositionClass
    [System.Serializable]
    public class RTSTPCWeaponPositionClass
    {
        [Tooltip("Not Needed For Data, For Easy Editor Access Only")]
        [SerializeField] public string AddableItemName;
        [Tooltip("The Third Person Controller ItemType reference")]
        [SerializeField] public ItemType m_ItemType;
        [Tooltip("The local position of the item")]
        [SerializeField] public Vector3 m_LocalPosition;
        [Tooltip("The local rotation of the item")]
        [SerializeField] public Vector3 m_LocalRotation;

    }
    #endregion

    [CreateAssetMenu(menuName = "RTSPrototype/RTSWeaponPositionsData")]
    public class RTSTPCWeaponPositionObject : ScriptableObject
    {
        public List<RTSTPCWeaponPositionClass> WeaponPositionsAndRotations = new List<RTSTPCWeaponPositionClass>();
    }
}