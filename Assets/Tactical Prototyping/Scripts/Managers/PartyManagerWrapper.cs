using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class PartyManagerWrapper : PartyManager
    {
        #region UnityMessages
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
        #endregion

    }
}