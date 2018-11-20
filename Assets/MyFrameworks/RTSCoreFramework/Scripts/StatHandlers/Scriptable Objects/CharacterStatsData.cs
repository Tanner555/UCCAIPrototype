using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public enum ECharacterType
    {
        //Characters
        //Allies
        BlueSillyPants = 0, BrownSillyPants = 1,
        SillyGirlMedic = 2,
        //Villians
        EvilAssaultVillian1 = 25,

        //Only Used When Character Type Could Not Be Found
        NoCharacterType = -1
    }

    [System.Serializable]
    public struct CharacterStats
    {
        public string name;
        [Tooltip("Used to Identify a Character")]
        public ECharacterType CharacterType;
        [Tooltip("Used to Spawn a Character")]
        public GameObject CharacterPrefab;
        public Sprite CharacterPortrait;

        [Header("Health Stats")]
        public int MaxHealth;
        public int Health;

        [Header("Stamina Stats")]
        public int MaxStamina;
        public int Stamina;

        [Header("Weapon Equipped")]
        public EEquipType EquippedWeapon;

        [Header("Weapon Stats")]
        public EWeaponType PrimaryWeapon;
        public EWeaponType SecondaryWeapon;
    }

    /// <summary>
    /// Used For Parsing Into XML Data
    /// </summary>
    [System.Serializable]
    public struct CharacterStatsSimple
    {
        public string name;
        [Tooltip("Used to Identify a Character")]
        public ECharacterType CharacterType;

        [Header("Health Stats")]
        public int MaxHealth;
        public int Health;

        [Header("Stamina Stats")]
        public int MaxStamina;
        public int Stamina;

        [Header("Weapon Equipped")]
        public EEquipType EquippedWeapon;

        [Header("Weapon Stats")]
        public EWeaponType PrimaryWeapon;
        public EWeaponType SecondaryWeapon;
    }

    /// <summary>
    /// Used For Important Editor Related Data - Prefab, Sprite, Etc..
    /// </summary>
    [System.Serializable]
    public struct CharacterStatsNonPersistent
    {
        public string name;
        [Tooltip("Used to Identify a Character")]
        public ECharacterType CharacterType;
        [Tooltip("Used to Spawn a Character")]
        public GameObject CharacterPrefab;
        public Sprite CharacterPortrait;
    }

    [CreateAssetMenu(menuName = "RTSPrototype/CharacterStatsData")]
    public class CharacterStatsData : ScriptableObject
    {
        [Header("Character Stats")]
        [SerializeField]
        public List<CharacterStatsNonPersistent> CharacterStatList;
    }
}
