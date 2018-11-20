using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    [System.Serializable]
    public struct PartyStats
    {
        public string name;

        [Tooltip("Used to Identify the Commander")]
        public RTSGameMode.ECommanders Commander;

        public int healthPotionAmount;
    }

    [CreateAssetMenu(menuName = "RTSPrototype/PartyStatsData")]
    public class PartyStatsData : ScriptableObject
    {
        [Header("Party Stats")]
        [SerializeField]
        public List<PartyStats> PartyStatList;
    }
}
