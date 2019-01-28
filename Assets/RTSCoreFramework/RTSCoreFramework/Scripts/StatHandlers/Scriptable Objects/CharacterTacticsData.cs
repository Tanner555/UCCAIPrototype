using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    [System.Serializable]
    public struct IGBPIPanelValue
    {
        public string order;
        public string condition;
        public string action;

        public IGBPIPanelValue(string order, string condition, string action)
        {
            this.order = order;
            this.condition = condition;
            this.action = action;
        }
    }

    [System.Serializable]
    public struct CharacterTactics
    {
        [Tooltip("Only Used For Element Naming, Not Used For Logic")]
        public string CharacterName;
        public ECharacterType CharacterType;
        public List<IGBPIPanelValue> Tactics;
    }

    [CreateAssetMenu(menuName = "RTSPrototype/CharacterTacticsData")]
    public class CharacterTacticsData : ScriptableObject
    {
        public List<CharacterTactics> CharacterTacticsList = new List<CharacterTactics>();
    }
}