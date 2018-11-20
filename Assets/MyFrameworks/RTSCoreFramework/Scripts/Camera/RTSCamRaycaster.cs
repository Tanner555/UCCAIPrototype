using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTSCoreFramework
{
    #region Public Structs
    /// <summary>
    /// Used to tell subs what type of object the Mouse Cursor is hitting
    /// </summary>
    public enum rtsHitType
    {
        Ally, Enemy, Cover, Walkable, Unwalkable, Unknown
    }

    public struct RTSRayCastDataObject
    {
        public rtsHitType _hitType;
        public RaycastHit _rayHit;
    }
    #endregion

    public class RTSCamRaycaster : MonoBehaviour
    {
        #region Properties
        RTSUiMaster uimaster
        {
            get { return RTSUiMaster.thisInstance; }
        }

        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        public static RTSCamRaycaster thisInstance { get; protected set; }

        bool noMoreChecking
        {
            get
            {
                return gamemode.GeneralInCommand == null ||
gamemode.GeneralInCommand.PartyMembers.Count <= 0;
            }
        }

        private string AllyTag
        {
            get
            {
                if (string.IsNullOrEmpty(__allyTag))
                    __allyTag = gamemode.AllyTag;

                return __allyTag;
            }
        }
        private string CoverTag
        {
            get
            {
                if (string.IsNullOrEmpty(__coverTag))
                    __coverTag = gamemode.CoverTag;

                return __coverTag;
            }
        }
        //Ally Layer that excludes CurrentPlayer
        private LayerMask sightNoCurrentPlayerLayer
        {
            get
            {
                if (__sightNoCurrentPlayerLayers == -1)
                    __sightNoCurrentPlayerLayers = gamemode.SightNoCurrentPlayerLayers;

                return __sightNoCurrentPlayerLayers;
            }
        }

        #endregion

        #region Fields
        //Tags
        private string __allyTag = "";
        private string __coverTag = "";

        private LayerMask __sightNoCurrentPlayerLayers = -1;

        //Method Fields
        private float maxRaycastDepth = 100f; // Hard coded value
        private Ray ray;
        private RaycastHit rayHit;
        private rtsHitType rayHitType = rtsHitType.Unknown;
        private rtsHitType rayHitTypeLastFrame = rtsHitType.Unknown;
        private GameObject gObject, gObjectRoot = null;
        private GameObject gObjectLastFrame = null;
        private string hitTag = "";
        private AllyMember hitAlly = null;

        //For event initialization checking
        bool hasStarted = false;

        //Call OnMouseCursorChange Every Iteration also
        //RTSTimer MyCursorChangeTimer = new RTSTimer(0.2f);
        #endregion

        #region UnityMessages
        private void OnEnable()
        {
            if (thisInstance == null)
                thisInstance = this;
            else if (hasStarted == false)
            {
                Debug.LogError("More than one RTS_CamRaycaster in scene!");
            }

            if (hasStarted == true)
            {
                SubToEvents();
            }

            //MyCursorChangeTimer.StartTimer();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }

        private void Start()
        {
            if (hasStarted == false)
            {
                SubToEvents();
            }
            if (gamemode == null)
            {
                Debug.LogError("GameMode is Null!");
                Destroy(this);
            }
            if (uimaster == null)
            {
                Debug.LogError("UIMaster is Null!");
                Destroy(this);
            }

            hasStarted = true;
        }

        // Update is called once per frame
        void Update()
        {
            //Makes Sure Code is Valid Before Running
            if (TimeToReturn()) return;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rayHit, maxRaycastDepth, sightNoCurrentPlayerLayer))
            {
                gObject = rayHit.collider.gameObject;
                gObjectRoot = rayHit.collider.gameObject.transform.root.gameObject;
                rayHitType = GetHitType();
                //if (rayHitType != rayHitTypeLastFrame || 
                //    MyCursorChangeTimer.IsTimerFinished())
                if(rayHitType != rayHitTypeLastFrame)
                {
                    //Timer created issues, retracted for now
                    //Update Cursor Change Every So Often To Resolve Cursor Bugs
                    //MyCursorChangeTimer.StartTimer();
                    //Layer has Changed
                    gamemaster.CallEventOnMouseCursorChange(rayHitType, rayHit);
                }
                gObjectLastFrame = gObject;
                rayHitTypeLastFrame = rayHitType;
            }
            else
            {
                if (gObjectLastFrame != null)
                {
                    gamemaster.CallEventOnMouseCursorChange(rtsHitType.Unknown, rayHit);
                }
                gObjectLastFrame = null;
                rayHitTypeLastFrame = rtsHitType.Unknown;
            }

        }
        #endregion

        #region RayFunctions
        public RTSRayCastDataObject GetRaycastInfo()
        {
            return new RTSRayCastDataObject { _hitType = rayHitType, _rayHit = rayHit };
        }

        rtsHitType GetHitType()
        {
            hitTag = gObjectRoot.tag;
            if (hitTag == AllyTag) return CheckAllyObject(gObjectRoot);
            else if (hitTag == CoverTag) return rtsHitType.Cover;
            else return CheckIfHitIsWalkable();
        }

        rtsHitType CheckAllyObject(GameObject gObjectRoot)
        {
            hitAlly = gObjectRoot.GetComponent<AllyMember>();
            if (hitAlly == null) return rtsHitType.Unknown;
            return gamemode.AllyIsGenCommanderMember(hitAlly) ?
                rtsHitType.Ally : rtsHitType.Enemy;
        }

        rtsHitType CheckIfHitIsWalkable()
        {
            return gamemode.isSurfaceReachableForAllyInCommand(rayHit) ?
                rtsHitType.Walkable : rtsHitType.Unwalkable;
        }

        bool TimeToReturn()
        {
            if (gamemode == null)
            {
                Debug.LogError("GameMode is Null!");
                Destroy(this);
                return true;
            }
            if (uimaster == null)
            {
                Debug.LogError("UIMaster is Null!");
                Destroy(this);
                return true;
            }
            if (uimaster.isUiAlreadyInUse) return true; 
            if (noMoreChecking) return true;
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject()) return true;
            return false;
        }
        #endregion

        #region Handlers
        void OnUIToggleChanged(bool _isEnabled)
        {
            //Change Layer If UI is toggled to false
            //This will fix the mouse cursor not having accurate image
            gamemaster.CallEventOnMouseCursorChange(rayHitType, rayHit);
        }

        void DestroyRaycaster()
        {
            Destroy(this);
        }
        #endregion

        #region Initialization
        void SubToEvents()
        {
            uimaster.EventAnyUIToggle += OnUIToggleChanged;
            gamemaster.GameOverEvent += DestroyRaycaster;
        }

        void UnsubFromEvents()
        {
            uimaster.EventAnyUIToggle -= OnUIToggleChanged;
            gamemaster.GameOverEvent -= DestroyRaycaster;
        }
        #endregion
    }
}