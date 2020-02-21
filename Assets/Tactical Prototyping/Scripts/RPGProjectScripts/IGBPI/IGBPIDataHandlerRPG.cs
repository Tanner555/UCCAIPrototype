using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RTSCoreFramework;
using BaseFramework;
using System;

namespace RPGPrototype
{
    public class IGBPIDataHandlerRPG : IGBPI_DataHandler
    {
        #region ConditionDictionary
        Dictionary<string, IGBPI_Condition> _appendedConditionDictionary;

        public override Dictionary<string, IGBPI_Condition> IGBPI_Conditions
        {
            get
            {
                if (_appendedConditionDictionary == null || _appendedConditionDictionary.Count <= 0)
                {
                    _appendedConditionDictionary = base.IGBPI_Conditions.AddRange(new Dictionary<string, IGBPI_Condition>
                    {

                    });
                }
                return _appendedConditionDictionary;
            }
        }
        #endregion

        #region ActionDictionary
        Dictionary<string, RTSActionItem> _appendedActionDictionary;

        public override Dictionary<string, RTSActionItem> IGBPI_Actions
        {
            get
            {
                if (_appendedActionDictionary == null || _appendedActionDictionary.Count <= 0)
                {
                    _appendedActionDictionary = base.IGBPI_Actions.AddRange(new Dictionary<string, RTSActionItem>
                    {
                        { "Self: Area of Effect", new RTSActionItem((_self, _ai, _target) =>
                        { _ai.TryStartSpecialAbility(typeof(AreaOfEffectConfigRPG)); },
                        (_self, _ai, _target) => _self.CanUseAbility(typeof(AreaOfEffectConfigRPG)),
                        ActionFilters.Abilities, (_self, _ai, _target) => { })},
                        { "Self: Heal", new RTSActionItem((_self, _ai, _target) =>
                        { _ai.TryStartSpecialAbility(typeof(SelfHealConfigRPG)); },
                        (_self, _ai, _target) => _self.CanUseAbility(typeof(SelfHealConfigRPG)),
                        ActionFilters.Abilities, (_self, _ai, _target) => { })}
                    });
                }
                return _appendedActionDictionary;
            }
        }
        #endregion
    }
}