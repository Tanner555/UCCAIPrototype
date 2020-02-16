using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using UnityEngine.InputSystem;

namespace RTSPrototype
{
    public class RTSInputManagerWrapper : InputManager, MyRTSInputActions.IGameplayActions
    {
        #region Properties    
        RTSCamRaycaster raycaster
        {
            get { return RTSCamRaycaster.thisInstance; }
        }

        //Movement Properties
        public float HorizontalMovement { get; protected set; }
        public float ForwardMovement { get; protected set; }

        //Mouse Setup - Scrolling
        protected bool bScrollAxisIsPositive
        {
            get { return scrollInputAxisValue >= 0.0f; }
        }

        #endregion

        #region OverrideAndHideProperties
        new protected RTSUiMaster uiMaster { get { return RTSUiMaster.thisInstance; } }

        new protected RTSUiManager uiManager
        {
            get { return RTSUiManager.thisInstance; }
        }

        new protected RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        new protected RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        new public static RTSInputManagerWrapper thisInstance
        {
            get { return InputManager.thisInstance as RTSInputManagerWrapper; }
        }
        #endregion

        #region BaseFields
        //Handles Right Mouse Down Input
        [Header("Right Mouse Down Config")]
        public float RMHeldThreshold = 0.15f;
        protected bool isRMHeldDown = false;
        protected bool isRMHeldPastThreshold = false;
        protected float RMCurrentTimer = 5f;
        //Handles Left Mouse Down Input
        [Header("Left Mouse Down Config")]
        public float LMHeldThreshold = 0.15f;
        protected bool isLMHeldDown = false;
        protected bool isLMHeldPastThreshold = false;
        protected float LMCurrentTimer = 5f;
        //Handles Mouse ScrollWheel Input
        //Scroll Input
        protected string scrollInputName = "Mouse ScrollWheel";
        protected float scrollInputAxisValue = 0.0f;
        protected bool bScrollWasPreviouslyPositive = false;
        protected bool bScrollIsCurrentlyPositive = false;
        //Scroll Timer Handling
        protected bool isScrolling = false;
        //Used to Fix First Scroll Not Working Issue
        protected bool bBeganScrolling = false;
        //Stop Scroll Functionality
        [Header("Mouse ScrollWheel Config")]
        public float scrollStoppedThreshold = 0.15f;
        protected bool isNotScrollingPastThreshold = false;
        protected float noScrollCurrentTimer = 5f;
        #endregion

        #region Fields
        //[Header("Input Action Asset Fields")]
        //[SerializeField]
        protected MyRTSInputActions MyInputActions;
        //[SerializeField]
        //protected string GameplayActionMapName = "Gameplay";
        //[SerializeField]
        //protected string PauseGameActionName = "PauseGame";

        //Handles Multi Unit Selection
        [Header("Selection Config")]
        [SerializeField]
        private RectTransform SelectionImage;
        Vector3 selectionStartPos;
        Vector3 selectionEndPos;
        //Sprinting Setup
        private bool isSprinting = false;
        //private AllyMoveSpeed setupMoveSpeed;
        private AllyMember setupSprintAlly = null;
        #endregion

        #region UnityMessages
        protected override void Start()
        {
            base.Start();            
            MyInputActions.Gameplay.SetCallbacks(this);
        }

        protected override void OnEnable()
        {
            MyInputActions = new MyRTSInputActions();
            MyInputActions.Enable();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            MyInputActions.Disable();
            base.OnDisable();            
        }
        #endregion

        #region InputHandlers
        public void OnNumberKeys(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                int _parsedKey = -1;
                if(int.TryParse(context.control.name, out _parsedKey))
                {
                    CallOnNumberKeyPress(_parsedKey);
                }                
            }
        }

        public void OnPauseGame(InputAction.CallbackContext context)
        {            
            if (context.performed) CallMenuToggle();
        }

        public void OnIGBPIMenuToggle(InputAction.CallbackContext context)
        {
            if (context.performed) CallIGBPIToggle();
        }

        public void OnCoverToggle(InputAction.CallbackContext context)
        {
            if (context.performed && UiIsEnabled == false)
                CallCoverToggle();
        }

        public void OnTryReload(InputAction.CallbackContext context)
        {
            if (context.performed && UiIsEnabled == false)
                CallTryReload();
        }

        public void OnTogglePauseControlMode(InputAction.CallbackContext context)
        {
            if (context.performed && UiIsEnabled == false)
                CallToggleIsInPauseControl();
        }

        public void OnHorizontalMovement(InputAction.CallbackContext context)
        {
            HorizontalMovement = context.ReadValue<float>();
        }

        public void OnForwardMovement(InputAction.CallbackContext context)
        {
            ForwardMovement = context.ReadValue<float>();
        }

        public void OnPossessAllyAdd(InputAction.CallbackContext context)
        {
            if (context.performed && UiIsEnabled == false)
                CallPossessAllyAdd();
        }

        public void OnPossessAllySubtract(InputAction.CallbackContext context)
        {
            if (context.performed && UiIsEnabled == false)
                CallPossessAllySubtract();
        }
        #endregion

        #region Handlers
        protected override void OnUpdateHandler()
        {
            base.OnUpdateHandler();
            LeftMouseDownSetup();
            RightMouseDownSetup();
            StopMouseScrollWheelSetup();
        }

        protected override void HandleGamePaused(bool _isPaused)
        {
            base.HandleGamePaused(_isPaused);
            if (_isPaused)
            {
                ResetMouseSetup();
            }
        }

        protected override void HandleUiActiveSelf(bool _state)
        {
            base.HandleUiActiveSelf(_state);
            if (_state == true)
            {
                ResetMouseSetup();
            }
        }

        protected override void HandleUiActiveSelf()
        {
            base.HandleUiActiveSelf();
            if (uiMaster.isUiAlreadyInUse)
            {
                ResetMouseSetup();
            }
        }
        #endregion

        #region MouseSetup
        void LeftMouseDownSetup()
        {
            if (UiIsEnabled) return;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (isRMHeldDown) return;
                if (isLMHeldDown == false)
                {
                    isLMHeldDown = true;
                    LMCurrentTimer = CurrentGameTime + LMHeldThreshold;
                }

                if (CurrentGameTime > LMCurrentTimer)
                {
                    //Calls Every Update
                    //CreateSelectionSquare();
                    if (isLMHeldPastThreshold == false)
                    {
                        //OnMouseDown Code Goes Here
                        isLMHeldPastThreshold = true;
                        gamemaster.CallEventHoldingLeftMouseDown(true);
                    }
                }
            }
            else
            {
                if (isLMHeldDown == true)
                {
                    isLMHeldDown = false;
                    if (isLMHeldPastThreshold == true)
                    {
                        //When MouseDown Code Exits
                        isLMHeldPastThreshold = false;
                        gamemaster.CallEventHoldingLeftMouseDown(false);
                    }
                    else
                    {
                        //Mouse Button Was Let Go Before the Threshold
                        //Call the Click Event
                        gamemaster.CallEventOnLeftClick();
                    }
                }
            }
        }

        void RightMouseDownSetup()
        {
            if (UiIsEnabled) return;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (isLMHeldDown) return;
                if (isRMHeldDown == false)
                {
                    isRMHeldDown = true;
                    RMCurrentTimer = CurrentGameTime + RMHeldThreshold;
                }

                if (CurrentGameTime > RMCurrentTimer)
                {
                    if (isRMHeldPastThreshold == false)
                    {
                        //OnMouseDown Code Goes Here
                        isRMHeldPastThreshold = true;
                        gamemaster.CallEventHoldingRightMouseDown(true);
                    }
                }
            }
            else
            {
                if (isRMHeldDown == true)
                {
                    isRMHeldDown = false;
                    if (isRMHeldPastThreshold == true)
                    {
                        //When MouseDown Code Exits
                        isRMHeldPastThreshold = false;
                        gamemaster.CallEventHoldingRightMouseDown(false);
                    }
                    else
                    {
                        //Mouse Button Was Let Go Before the Threshold
                        //Call the Click Event
                        gamemaster.CallEventOnRightClick();
                    }
                }
            }

        }

        void StopMouseScrollWheelSetup()
        {
            if (UiIsEnabled) return;
            scrollInputAxisValue = Input.GetAxis(scrollInputName);
            if (Mathf.Abs(scrollInputAxisValue) < 0.05f)
            {
                //Not Using ScrollWheel, See if Holding '+,-' Keys
                if (Input.GetKey(KeyCode.KeypadPlus))
                {
                    scrollInputAxisValue = 1.0f;
                }
                else if (Input.GetKey(KeyCode.KeypadMinus))
                {
                    scrollInputAxisValue = -1.0f;
                }
            }
            if (Mathf.Abs(scrollInputAxisValue) > 0.05f)
            {
                if (isLMHeldDown) return;
                bScrollIsCurrentlyPositive = bScrollAxisIsPositive;

                //Fixes First Scroll Not Working Issue
                if (bBeganScrolling == false)
                {
                    bBeganScrolling = true;
                    gamemaster.CallEventEnableCameraZoom(true, bScrollAxisIsPositive);
                }

                if (bScrollWasPreviouslyPositive != bScrollIsCurrentlyPositive)
                {
                    gamemaster.CallEventEnableCameraZoom(true, bScrollAxisIsPositive);
                    bScrollWasPreviouslyPositive = bScrollAxisIsPositive;
                }

                if (isScrolling == false)
                {
                    isScrolling = true;
                    if (isNotScrollingPastThreshold == true)
                    {
                        //When ScrollWheel Code Starts
                        isNotScrollingPastThreshold = false;
                        gamemaster.CallEventEnableCameraZoom(true, bScrollAxisIsPositive);
                        bScrollWasPreviouslyPositive = bScrollAxisIsPositive;
                    }
                    else
                    {
                        //Scroll Wheel Started Before the Stop Threshold
                        //Do nothing for now
                    }
                }
            }
            else
            {
                if (isScrolling == true)
                {
                    isScrolling = false;
                    noScrollCurrentTimer = CurrentGameTime + scrollStoppedThreshold;
                }

                if (CurrentGameTime > noScrollCurrentTimer)
                {
                    if (isNotScrollingPastThreshold == false)
                    {
                        //OnScrollWheel Stopping Code Goes Here
                        isNotScrollingPastThreshold = true;
                        gamemaster.CallEventEnableCameraZoom(false, bScrollAxisIsPositive);
                    }
                }
            }
        }

        #endregion

        #region MouseHelpers
        /// <summary>
        /// Used Whenever Mouse Setup Needs to be disabled,
        /// Such as when a UI Menu (Pause Menu) is Active
        /// </summary>
        void ResetMouseSetup()
        {
            if (isRMHeldPastThreshold)
            {
                isRMHeldPastThreshold = false;
                gamemaster.CallEventHoldingRightMouseDown(false);
            }
            if (isLMHeldPastThreshold)
            {
                isLMHeldPastThreshold = false;
                gamemaster.CallEventHoldingLeftMouseDown(false);
            }

            isLMHeldDown = false;
            isRMHeldDown = false;
            //Reset Scrolling
            isScrolling = false;
            isNotScrollingPastThreshold = true;
            bBeganScrolling = false;
            noScrollCurrentTimer = 0.0f;
            gamemaster.CallEventEnableCameraZoom(false, bScrollAxisIsPositive);
        }
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
        }

        protected override void UnsubFromEvents()
        {
            base.UnsubFromEvents();
        }
        #endregion

        #region InputSetup
        protected override void InputSetup()
        {
            base.InputSetup();
            //if (Input.GetKeyDown(KeyCode.I))
            //    CallInventoryToggle();
            //if (Input.GetKeyDown(KeyCode.B))
            //    CallIGBPIToggle();
            //if (Input.GetKeyDown(KeyCode.L))
            //    CallLuaEditorToggle();
            
            //if (UiIsEnabled) return;
            //All Input That Shouldn't Happen When 
            //Ui is Enabled
            //if (Input.GetKeyDown(KeyCode.Keypad1))
            //    CallPossessAllyAdd();
            //if (Input.GetKeyDown(KeyCode.Keypad3))
            //    CallPossessAllySubtract();
            //if (Input.GetKeyDown(KeyCode.C))
            //    CallCoverToggle();
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //    CallSelectNextWeapon();
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //    CallSelectPrevWeapon();
            //if (Input.GetKeyDown(KeyCode.R))
            //    CallTryReload();
            //if (Input.GetKeyDown(KeyCode.Space))
            //    CallToggleIsInPauseControl();
            //if (Input.GetKeyDown(KeyCode.LeftShift))
            //    CallSprintToggle();

            //if (Input.GetKey(KeyCode.LeftShift))
            //    SprintingSetup();
            //else
            //    EndSprintingSetup();

        }

        #endregion

        #region UnitSelection

        //void SelectionInitialize()
        //{
        //    if (SelectionImage == null) return;
        //    selectionStartPos = Input.mousePosition;
        //    //RaycastHit _hit;
        //    //if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, Mathf.Infinity))
        //    //{
        //    //    selectionStartPos = _hit.point;
        //    //}

        //}

        //void CreateSelectionSquare()
        //{
        //    //YouTube Vids
        //    //Unity Tutorial - RTS Controls - Selection Box GUI - Part 4 
        //    //https://youtu.be/vsdIhyLKgjc
        //    //Unity Tutorial - RTS Controls - Selection Box Function - Part 5 
        //    //https://youtu.be/ceMyupol6AQ

        //    if (SelectionImage == null) return;

        //    if (!SelectionImage.gameObject.activeSelf)
        //        SelectionImage.gameObject.SetActive(true);

        //    selectionEndPos = Input.mousePosition;
        //    selectionEndPos.z = 0f;
        //    Vector3 _squareStart = selectionStartPos;
        //    //Vector3 _squareStart = Camera.main.WorldToScreenPoint(selectionStartPos);
        //    _squareStart.z = 0f;
        //    Vector3 _center = (_squareStart + selectionEndPos) / 2f;

        //    SelectionImage.position = _center;

        //    float _sizeX = Mathf.Abs(_squareStart.x - selectionEndPos.x);
        //    float _sizeY = Mathf.Abs(_squareStart.y - selectionEndPos.y);
        //    SelectionImage.sizeDelta = new Vector2(_sizeX, _sizeY);

        //}

        //void StopSelectionSquare()
        //{
        //    if (SelectionImage == null) return;

        //    if (SelectionImage.gameObject.activeSelf)
        //        SelectionImage.gameObject.SetActive(false);
        //}

        #endregion

        #region InputCalls
        //void CallInventoryToggle() { uiMaster.CallEventInventoryUIToggle(); }
        void CallToggleIsInPauseControl() { gamemaster.CallOnTogglebIsInPauseControlMode(); }
        void CallIGBPIToggle() { uiMaster.CallEventIGBPIToggle(); }
        void CallLuaEditorToggle() { uiMaster.CallEventLuaEditorToggle(); }
        void CallPossessAllyAdd() { gamemode.GeneralInCommand.PossessAllyAdd(); }
        void CallPossessAllySubtract() { gamemode.GeneralInCommand.PossessAllySubtract(); }
        //void CallSelectPrevWeapon() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnSwitchToPrevItem(); }
        //void CallSelectNextWeapon() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnSwitchToNextItem(); }
        //void CallTryFire() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnTryUseWeapon(); }
        void CallTryReload() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnTryReload(); }
        void CallCoverToggle() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnTryCrouch(); }

        //void CallSprintToggle() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallEventToggleIsSprinting(); }

        #region Testing JobQueues
        //List<GameObject> TestGObjects;
        //JobGroupHandler.JobGroupObject myGroup = null;
        //int _loopCounter = 0;
        //int _testCounter = 0;
        //string _key = "TestingQueue";
        //string _deleteKey = "DeleteObjects";
        //[Header("Testing")]
        //public GameObject TestQueuePrefab;
        //public int _iterCount = 1000;

        //private void Start()
        //{
        //    TestGObjects = new List<GameObject>();
        //}

        //void TestQueue()
        //{
        //    for (int i = 0; i < _iterCount; i++)
        //    {
        //        string _gName = "";
        //        if (_loopCounter > _iterCount)
        //            _gName = $"New Object: {i + _loopCounter}";
        //        else
        //            _gName = $"New Object: {i}";

        //        if (TestQueuePrefab != null)
        //        {
        //            GameObject _gObject = GameObject.Instantiate(TestQueuePrefab, this.transform) as GameObject;
        //            _gObject.name = _gName;
        //            TestGObjects.Add(_gObject);
        //        }
        //        _loopCounter++;
        //    }
        //    _testCounter++;
        //    //Testing Framework
        //    Action<GameObject> _job = (_gObject) =>
        //    {
        //        var _br = _gObject.GetComponent<Rigidbody>() != null;
        //        var _bc = _gObject.GetComponent<BoxCollider>() != null;
        //        var _bmr = _gObject.GetComponent<MeshRenderer>() != null;
        //        var _bmf = _gObject.GetComponent<MeshFilter>() != null;
        //        if (!_br && !_bc && !_bmr && !_bmf)
        //        {
        //            var _rb = _gObject.AddComponent<Rigidbody>();
        //            _rb.mass = 1000;
        //            _rb.isKinematic = true;
        //            var _c = _gObject.AddComponent<BoxCollider>();
        //            _c.isTrigger = true;
        //            _gObject.AddComponent<MeshRenderer>();
        //            _gObject.AddComponent<MeshFilter>();
        //        }
        //    };

        //    JobGroupHandler.JobGroupObject _manager = null;
        //    if (JobGroupHandler.LibraryContainsKey(_key))
        //    {
        //        _manager = JobGroupHandler.GetJobGroup(_key);
        //    }
        //    else
        //    {
        //        _manager = JobGroupHandler.CreateJobGroupForArray<GameObject>(TestGObjects, _job, 10, _key);
        //    }
        //    _manager.StartJobQueue(10);


        //    foreach (var _gObject in TestGObjects)
        //    {
        //        var _br = _gObject.GetComponent<Rigidbody>() != null;
        //        var _bc = _gObject.GetComponent<BoxCollider>() != null;
        //        var _bmr = _gObject.GetComponent<MeshRenderer>() != null;
        //        var _bmf = _gObject.GetComponent<MeshFilter>() != null;
        //        if (!_br && !_bc && !_bmr && !_bmf)
        //        {
        //            var _rb = _gObject.AddComponent<Rigidbody>();
        //            _rb.mass = 1000;
        //            _rb.isKinematic = true;
        //            var _c = _gObject.AddComponent<BoxCollider>();
        //            _c.isTrigger = true;
        //            _gObject.AddComponent<MeshRenderer>();
        //            _gObject.AddComponent<MeshFilter>();
        //            _gObject.name += "AND" + _gObject.name;
        //        }
        //    }


        //    Action _job = () =>
        //    {
        //        for (int i = 0; i < 1; i++)
        //        {
        //            //var _g = new GameObject($"New Object: {i}");
        //            //GameObject _gObject = GameObject.Instantiate(_g, null) as GameObject;
        //            TestGObjects.Add(this.gameObject);
        //        }
        //    };
        //    var _group = JobGroupHandler.CreateNewJobGroup(_job, 15000, "Hello", false);
        //    await Task.Delay(10);
        //    _group.StartJobQueue();


        //    for (int i = 0; i < TestGObjects.Count; i++)
        //    {
        //        TestGObjects[i].name = $"Hello Object {i}";
        //    }

        //    for (int i = 0; i < TestNames.Count; i++)
        //    {
        //        TestNames[i] = "Hello There";
        //    }
        //    if (myGroup == null)
        //    {
        //        List<string> names = new List<string>();
        //        Action _job = () =>
        //        {
        //            //var tempNames = names;
        //            //foreach (var _name in tempNames)
        //            //{

        //            //}
        //        };
        //        string _key = "testKey";
        //        myGroup = JobGroupHandler.CreateNewJobGroup(_job, 500, _key, false);
        //    }
        //    if (myGroup.GroupExists && myGroup.HasStartedDequeue == false)
        //    {
        //        await Task.Delay(1000);
        //        myGroup.StartJobQueue();
        //    }
        //}
        #endregion

        #endregion
    }
}