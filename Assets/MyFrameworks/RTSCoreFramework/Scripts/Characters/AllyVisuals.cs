using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RTSCoreFramework
{
    public class AllyVisuals : MonoBehaviour
    {
        #region PropsAndFields
        [Header("Ally Highlighting")]
        public Color selAllyColor;
        public Color selEnemyColor;
        protected GameObject AllyIndicatorSpotlightInstance = null;
        public Light SelectionLight
        {
            get
            {
                if (_SelectionLight == null)
                    _SelectionLight = AllyIndicatorSpotlightInstance.GetComponent<Light>();

                return _SelectionLight;
            }
        }
        private Light _SelectionLight = null;
        [Header("Ally Waypoint Navigation")]
        public Material waypointRendererMaterial;
        protected float waypointStartWidth = 0.05f;
        protected float waypointEndWidth = 0.05f;
        protected Color waypointStartColor = Color.yellow;
        protected Color waypointEndColor = Color.yellow;
        protected LineRenderer waypointRenderer;
        [Header("Ally Damage Effects")]
        public GameObject BloodParticles;
        [SerializeField]
        Image myHealthBar;
        [SerializeField]
        Image myActiveTimeBar;

        protected RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        protected AllyMember thisAlly
        {
            get
            {
                if (_thisAlly == null)
                    _thisAlly = GetComponent<AllyMember>();

                return _thisAlly;
            }
        }
        private AllyMember _thisAlly = null;

        protected AllyEventHandler myEventHandler
        {
            get
            {
                if(_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandler>();

                return _myEventHandler;
            }
        }
        private AllyEventHandler _myEventHandler = null;

        protected RTSUiMaster uiMaster { get { return RTSUiMaster.thisInstance; } }

        protected bool friend
        {
            get { return thisAlly.bIsInGeneralCommanderParty; }
        }

        //NavMesh used for Waypoint Rendering
        protected NavMeshAgent myNavMesh
        {
            get
            {
                if (_myNavMesh == null)
                    _myNavMesh = GetComponent<NavMeshAgent>();

                return _myNavMesh;
            }
        }
        NavMeshAgent _myNavMesh = null;

        protected bool cameraIsMoving = false;

        protected bool bHasSwitched = false;

        protected float waypointUpdateRate = 0.5f;
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            myEventHandler.OnHoverOver += OnCursEnter;
            myEventHandler.OnHoverLeave += OnCursExit;
            myEventHandler.EventAllyDied += HandleDeath;
            myEventHandler.EventCommandMove += SetupWaypointRenderer;
            myEventHandler.EventTogglebIsFreeMoving += CheckToDisableWaypointRenderer;
            myEventHandler.EventFinishedMoving += DisableWaypointRenderer;
            myEventHandler.EventPartySwitching += OnPartySwitch;
            myEventHandler.EventCommandAttackEnemy += OnCmdAttackEnemy;
            myEventHandler.EventCommandAttackEnemy += DisableWaypointRenderer;
            myEventHandler.OnAllyTakeDamage += SpawnBloodParticles;
            myEventHandler.OnHealthChanged += OnHealthUpdate;
            myEventHandler.OnActiveTimeChanged += OnActiveTimeBarUpdate;
            myEventHandler.InitializeAllyComponents += OnAllyInitComponents;
            gamemaster.GameOverEvent += HandleGameOver;
            gamemaster.EventHoldingRightMouseDown += HandleCameraMovement;
            uiMaster.EventAnyUIToggle += HandleUIEnable;
        }

        protected virtual void OnDisable()
        {
            myEventHandler.OnHoverOver -= OnCursEnter;
            myEventHandler.OnHoverLeave -= OnCursExit;
            myEventHandler.EventAllyDied -= HandleDeath;
            myEventHandler.EventCommandMove -= SetupWaypointRenderer;
            myEventHandler.EventTogglebIsFreeMoving -= CheckToDisableWaypointRenderer;
            myEventHandler.EventFinishedMoving -= DisableWaypointRenderer;
            myEventHandler.EventPartySwitching -= OnPartySwitch;
            myEventHandler.EventCommandAttackEnemy -= OnCmdAttackEnemy;
            myEventHandler.EventCommandAttackEnemy -= DisableWaypointRenderer;
            myEventHandler.OnAllyTakeDamage -= SpawnBloodParticles;
            myEventHandler.OnHealthChanged -= OnHealthUpdate;
            myEventHandler.OnActiveTimeChanged -= OnActiveTimeBarUpdate;
            myEventHandler.InitializeAllyComponents -= OnAllyInitComponents;
            gamemaster.GameOverEvent -= HandleGameOver;
            gamemaster.EventHoldingRightMouseDown -= HandleCameraMovement;
            uiMaster.EventAnyUIToggle -= HandleUIEnable;
        }
        // Use this for initialization
        protected virtual void Start()
        {
            SelectionLight.enabled = false;
        }
        #endregion

        #region Handlers-CursorHoverandExit
        void OnCursEnter()
        {
            if (cameraIsMoving ||
                thisAlly.bIsCurrentPlayer) return;

            SelectionLight.enabled = true;
            if (friend)
            {
                SelectionLight.color = selAllyColor;
            }
            else
            {
                SelectionLight.color = selEnemyColor;
            }
        }

        void OnCursExit()
        {
            if (cameraIsMoving) return;
            SelectionLight.enabled = false;
        }

        #endregion

        #region Handlers
        protected virtual void OnAllyInitComponents(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {    
            selAllyColor = _allFields.AllyHighlightColor;
            selEnemyColor = _allFields.EnemyHighlightColor;
            AllyIndicatorSpotlightInstance = _specific.AllyIndicatorSpotlightInstance;
            waypointRendererMaterial = _allFields.WaypointRendererMaterial;
            BloodParticles = _allFields.BloodParticles;
            myHealthBar = _specific.EnemyHealthBarImage;
            myActiveTimeBar = _specific.EnemyActiveBarImage;
        }

        protected virtual void OnHealthUpdate(int _current, int _max)
        {
            if (myHealthBar != null && myHealthBar.enabled)
            {
                float _healthAsPercentage = ((float)_current / (float)_max);
                myHealthBar.fillAmount = _healthAsPercentage;
            }
        }

        protected virtual void OnActiveTimeBarUpdate(int _current, int _max)
        {
            if(myActiveTimeBar != null && myActiveTimeBar.enabled)
            {
                float _activeTimeAsPercentage = ((float)_current / (float)_max);
                myActiveTimeBar.fillAmount = _activeTimeAsPercentage;
            }
        }

        protected virtual void SpawnBloodParticles(int amount, Vector3 position, Vector3 force, AllyMember _instigator, GameObject hitGameObject, Collider hitCollider)
        {
            if (BloodParticles == null) return;
            GameObject.Instantiate(BloodParticles, position, Quaternion.identity);
        }

        protected virtual void SetupWaypointRenderer(Vector3 _point)
        {
            if (IsInvoking("UpdateWaypointRenderer"))
            {
                CancelInvoke("UpdateWaypointRenderer");
            }
            InvokeRepeating("UpdateWaypointRenderer", 0.1f, waypointUpdateRate);
        }

        protected virtual void DisableWaypointRenderer()
        {
            if (IsInvoking("UpdateWaypointRenderer"))
                CancelInvoke("UpdateWaypointRenderer");

            if (waypointRenderer != null)
            {
                waypointRenderer.enabled = false;
            }
        }

        protected virtual void DisableWaypointRenderer(AllyMember _ally)
        {
            if (waypointRenderer != null)
            {
                waypointRenderer.enabled = false;
            }
        }

        protected virtual void CheckToDisableWaypointRenderer(bool _isFreeMoving)
        {
            //If Free Moving, Need to disable waypoint renderer
            if (_isFreeMoving)
            {
                DisableWaypointRenderer();
            }
        }

        protected virtual void OnPartySwitch()
        {
            DisableWaypointRenderer();
            bHasSwitched = true;
            Invoke("SetbHasSwitchedToFalse", 0.2f);
        }

        protected virtual void OnCmdAttackEnemy(AllyMember _ally)
        {
            DisableWaypointRenderer();
            bHasSwitched = true;
            Invoke("SetbHasSwitchedToFalse", 0.2f);
        }

        protected virtual void SetbHasSwitchedToFalse()
        {
            bHasSwitched = false;
        }

        protected virtual void HandleDeath()
        {
            DestroyOnDeath();
        }

        protected virtual void HandleGameOver()
        {
            DestroyOnDeath();
        }

        protected virtual void DestroyOnDeath()
        {
            if (SelectionLight != null)
            {
                SelectionLight.enabled = true;
                Destroy(SelectionLight);
            }
            if (waypointRenderer != null)
            {
                waypointRenderer.enabled = true;
                Destroy(waypointRenderer);
            }
            if(myHealthBar != null)
            {
                myHealthBar.enabled = true;
                Destroy(myHealthBar);
            }
            if(myActiveTimeBar != null)
            {
                myActiveTimeBar.enabled = true;
                Destroy(myActiveTimeBar);
            }
            Destroy(this);
        }

        protected virtual void HandleCameraMovement(bool _isMoving)
        {
            if (SelectionLight == null) return;
            cameraIsMoving = _isMoving;
            SelectionLight.enabled = false;
        }

        protected virtual void HandleUIEnable(bool _enabled)
        {
            if (_enabled && SelectionLight != null && SelectionLight.enabled)
            {
                SelectionLight.enabled = false;
            }
        }
        #endregion

        #region Helpers
        protected virtual void UpdateWaypointRenderer()
        {
            if (bHasSwitched || myNavMesh == null ||
                myNavMesh.path == null ||
                myEventHandler.bIsAIMoving)
                return;

            if (waypointRenderer != null && waypointRenderer.enabled == false)
            {
                waypointRenderer.enabled = true;
            }
            else if (waypointRenderer == null)
            {
                waypointRenderer = this.gameObject.AddComponent<LineRenderer>();
                if (waypointRendererMaterial != null)
                    waypointRenderer.material = waypointRendererMaterial;

                waypointRenderer.startWidth = waypointStartWidth;
                waypointRenderer.endWidth = waypointEndWidth;
                waypointRenderer.startColor = waypointStartColor;
                waypointRenderer.endColor = waypointEndColor;
            }

            var path = myNavMesh.path;

            waypointRenderer.positionCount = path.corners.Length;

            for (int i = 0; i < path.corners.Length; i++)
            {
                waypointRenderer.SetPosition(i, path.corners[i]);
            }
        }
        #endregion

    }
}