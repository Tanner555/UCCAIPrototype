using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype
{
    #region RTSCharacterSpawnerSettingsClass
    public enum RTSCharacterSpawnerSettingsType
    {
        RPGCharacterWStandardController = 0,
        UltimateCharacterController = 1
    };

    [System.Serializable]
    public class RTSCharacterSpawnerSettings
    {
        public RTSCharacterSpawnerSettingsType CharacterSpawnerSettingsType;
        
        //HardCoded For Now, Consider Using More Modular Approach
        public List<System.Type> GetComponentsToAdd()
        {
            switch (CharacterSpawnerSettingsType)
            {
                case RTSCharacterSpawnerSettingsType.RPGCharacterWStandardController:
                    return new List<System.Type> { typeof(RPGCharacter), typeof(RPGWeaponSystem) };
                case RTSCharacterSpawnerSettingsType.UltimateCharacterController:
                    return new List<System.Type> { typeof(RTSItemAndControlHandler) };
                default:
                    return null;
            }
        }
    }
    #endregion

    [CreateAssetMenu(menuName = "RTSPrototype/Spawner/RTSCharacterSpawnerSettings")]
    public class RTSCharacterSpawnerSettingsObject : ScriptableObject
    {
        [SerializeField]
        public List<RTSCharacterSpawnerSettings> CharacterSpawnerSettingsList;
    }
}