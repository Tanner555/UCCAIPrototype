using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;

namespace RPGPrototype
{
    public class RPGGameMode : RTSGameMode
    {
        #region Properties
        //Static GameMode Instance For Easy Access
        [HideInInspector]
        public static new RPGGameMode thisInstance
        {
            get { return (RPGGameMode)GameMode.thisInstance; }
        }
        #endregion

        #region LayersAndTags
        [Header("Layers For AI and Raycasting")]
        [SerializeField]
        private LayerMask walkableLayers;

        public LayerMask WalkableLayers => walkableLayers;
        #endregion
    }
}