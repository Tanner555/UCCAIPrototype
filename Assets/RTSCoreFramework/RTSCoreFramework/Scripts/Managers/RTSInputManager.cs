using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;

namespace RTSCoreFramework
{
    public class RTSInputManager : InputManager
    {
        #region Properties       
        RTSCamRaycaster raycaster
        {
            get { return RTSCamRaycaster.thisInstance; }
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

        new public static RTSInputManager thisInstance
        {
            get { return InputManager.thisInstance as RTSInputManager; }
        }
        #endregion

        #region Fields
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
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
        }
        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        #endregion

        #region InputSetup
        protected override void InputSetup()
        {
            base.InputSetup();
            //if (Input.GetKeyDown(KeyCode.I))
            //    CallInventoryToggle();
            if (Input.GetKeyDown(KeyCode.B))
                CallIGBPIToggle();
            if (UiIsEnabled) return;
            //All Input That Shouldn't Happen When 
            //Ui is Enabled
            if (Input.GetKeyDown(KeyCode.Keypad1))
                CallPossessAllyAdd();
            if (Input.GetKeyDown(KeyCode.Keypad3))
                CallPossessAllySubtract();
            if (Input.GetKeyDown(KeyCode.C))
                CallCoverToggle();
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //    CallSelectNextWeapon();
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //    CallSelectPrevWeapon();
            if (Input.GetKeyDown(KeyCode.R))
                CallTryReload();
            if (Input.GetKeyDown(KeyCode.Space))
                CallToggleIsInPauseControl();
            if (Input.GetKeyDown(KeyCode.LeftShift))
                CallSprintToggle();

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
        void CallPossessAllyAdd() { gamemode.GeneralInCommand.PossessAllyAdd(); }
        void CallPossessAllySubtract() { gamemode.GeneralInCommand.PossessAllySubtract(); }
        //void CallSelectPrevWeapon() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnSwitchToPrevItem(); }
        //void CallSelectNextWeapon() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnSwitchToNextItem(); }
        void CallTryFire() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnTryUseWeapon(); }
        void CallTryReload() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnTryReload(); }
        void CallCoverToggle() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallOnTryCrouch(); }
        void CallSprintToggle() { gamemode.GeneralInCommand.AllyInCommand.allyEventHandler.CallEventToggleIsSprinting(); }

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