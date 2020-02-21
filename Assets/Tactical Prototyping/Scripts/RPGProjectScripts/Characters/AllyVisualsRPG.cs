using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSCoreFramework;
#if RTSAStarPathfinding
using Pathfinding;
#endif

namespace RPGPrototype
{
    public class AllyVisualsRPG : AllyVisuals
    {
        #region Properties
#if RTSAStarPathfinding
        Seeker mySeeker
        {
            get
            {
                if (_mySeeker == null)
                {
                    _mySeeker = GetComponent<Seeker>();
                }
                return _mySeeker;
            }
        }
        Seeker _mySeeker = null;

        AIPath myAIPath
        {
            get
            {
                if (_myAIPath == null)
                    _myAIPath = GetComponent<AIPath>();

                return _myAIPath;
            }
        }
        AIPath _myAIPath = null;
#endif
        #endregion

        #region Fields
        //[SerializeField]
        //Image myHealthBar;
        //Extra
        bool bUseAStarPath = false;
        #endregion

        #region UnityMessages
        protected override void Start()
        {
            base.Start();
            myEventHandler.OnHealthChanged += OnHealthUpdate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            myEventHandler.OnHealthChanged -= OnHealthUpdate;
        }
        #endregion

        #region Handlers
        protected override void OnAllyInitComponents(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            base.OnAllyInitComponents(_specific, _allFields);
            var _RPGallAllyComps = (AllyComponentsAllCharacterFieldsRPG)_allFields;
            this.bUseAStarPath = _RPGallAllyComps.bUseAStarPath;
        }
        //void OnHealthUpdate(int _current, int _max)
        //{
        //    if(myHealthBar != null)
        //    {
        //        float _healthAsPercentage = ((float)_current / (float)_max);
        //        myHealthBar.fillAmount = _healthAsPercentage;
        //    }
        //}
        #endregion

//        protected override void UpdateWaypointRenderer()
//        {
//#if RTSAStarPathfinding
//            if (bUseAStarPath == false)
//            {
//                base.UpdateWaypointRenderer();
//            }
//            else
//            {
//                if (bHasSwitched || myAIPath == null || mySeeker == null ||
//                myAIPath.hasPath == false || myEventHandler.bIsAIMoving) return;

//                if (waypointRenderer != null && waypointRenderer.enabled == false)
//                {
//                    waypointRenderer.enabled = true;
//                }
//                else if (waypointRenderer == null)
//                {
//                    waypointRenderer = this.gameObject.AddComponent<LineRenderer>();
//                    if (waypointRendererMaterial != null)
//                        waypointRenderer.material = waypointRendererMaterial;

//                    waypointRenderer.startWidth = waypointStartWidth;
//                    waypointRenderer.endWidth = waypointEndWidth;
//                    waypointRenderer.startColor = waypointStartColor;
//                    waypointRenderer.endColor = waypointEndColor;
//                }

//                var path = mySeeker.GetCurrentPath();

//                waypointRenderer.positionCount = path.path.Count;

//                for (int i = 0; i < path.path.Count; i++)
//                {
//                    waypointRenderer.SetPosition(i, (Vector3)path.path[i].position);
//                }
//            }
//#else
//            base.UpdateWaypointRenderer();
//#endif
//        }
    }
}