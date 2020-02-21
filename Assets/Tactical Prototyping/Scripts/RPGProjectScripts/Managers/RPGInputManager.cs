using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using RPG.Characters;

namespace RPGPrototype
{
    public class RPGInputManager /*: RTSInputManager*/
    {
        #region Fields
        //List<int> NumberKeys = new List<int>
        //{
        //    0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        //};

        //List<string> NumberKeyNames = new List<string>
        //{
        //    "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        //};
        #endregion

        #region Properties
        protected RPGGameMaster gamemaster
        {
            get { return RPGGameMaster.thisInstance; }
        }
        #endregion

        #region InputSetup
        //protected override void InputSetup()
        //{
        //    base.InputSetup();
        //    foreach (int _key in NumberKeys)
        //    {
        //        if (Input.GetKeyDown(_key.ToString()))
        //        {
        //            CallOnNumberKeyPress(_key);
        //        }
        //    }
            
        //}
        #endregion

        #region InputCalls
        //void CallOnNumberKeyPress(int _index)
        //{
        //    gameMaster.CallOnNumberKeyPress(_index);
        //}
        #endregion
    }
}