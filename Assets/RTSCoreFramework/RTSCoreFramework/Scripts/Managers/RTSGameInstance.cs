using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaseFramework;

namespace RTSCoreFramework
{
    public class RTSGameInstance : GameInstance
    {
        #region OverrideAndHideProperties
        new public static RTSGameInstance thisInstance
        {
            get { return GameInstance.thisInstance as RTSGameInstance; }
        }
        #endregion

        #region UnityMessages
        // Use this for initialization
        protected override void OnEnable()
        {
            base.OnEnable();            
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        #endregion

    }
}