using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using RPG.Characters;

namespace RPGPrototype
{
    public class RPGGameMaster : RTSGameMaster
    {
        #region Properties
        public static new RPGGameMaster thisInstance
        {
            get
            {
                return (RPGGameMaster)GameMaster.thisInstance;
            }
        }
        #endregion

        #region Delegates
        public delegate void OnMouseOverEnemy(EnemyAI enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        public delegate void OnMouseOverVoice(Voice voice);
        public event OnMouseOverVoice onMouseOverVoice;

        public delegate void OnMouseOverTerrain(Vector3 destination);
        public event OnMouseOverTerrain onMouseOverPotentiallyWalkable;

        //Custom
        //public delegate void OneIntParamHandler(int _number);
        //public event OneIntParamHandler OnNumberKeyPress;
        #endregion

        #region Calls
        public void CallonMouseOverEnemy(EnemyAI enemy)
        {
            if (onMouseOverEnemy != null) onMouseOverEnemy(enemy);
        }

        public void CallonMouseOverVoice(Voice voice)
        {
            if (onMouseOverVoice != null) onMouseOverVoice(voice);
        }

        public void CallonMouseOverPotentiallyWalkable(Vector3 _destination)
        {
            if (onMouseOverPotentiallyWalkable != null) onMouseOverPotentiallyWalkable(_destination);
        }

        //Custom
        //public void CallOnNumberKeyPress(int _number)
        //{
        //    if (OnNumberKeyPress != null) OnNumberKeyPress(_number);
        //}

        protected override void ToggleGamePauseTimeScale()
        {
            //base.ToggleGamePauseTimeScale();
        }

        protected override void TogglePauseControlModeTimeScale()
        {
            //base.TogglePauseControlModeTimeScale();
        }
        #endregion
    }
}