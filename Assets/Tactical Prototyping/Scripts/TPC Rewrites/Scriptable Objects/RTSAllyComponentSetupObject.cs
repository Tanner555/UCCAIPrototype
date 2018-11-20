using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype
{
    [CreateAssetMenu(menuName = "RTSPrototype/RTSAllyComponentSetupData")]
    public class RTSAllyComponentSetupObject : ScriptableObject
    {
        [Header("Fields That All Allies Will Use")]
        public RTSAllyComponentsAllCharacterFieldsWrapper AllyComponentSetupFields;
    }
}