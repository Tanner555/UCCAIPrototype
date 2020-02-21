using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RPGPrototype
{
    [CreateAssetMenu(menuName = "RTSPrototype/RPGAllyComponentSetupData")]
    public class RPGAllyComponentSetupObject : ScriptableObject
    {
        [Header("Fields That All Allies Will Use")]
        public AllyComponentsAllCharacterFieldsRPG AllyComponentSetupFields;
    }
}