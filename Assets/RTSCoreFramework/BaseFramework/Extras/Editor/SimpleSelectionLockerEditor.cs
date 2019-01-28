using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BaseFramework
{
    [CustomEditor(typeof(SimpleSelectionLocker))]
    public class SimpleSelectionLockerEditor : Editor
    {
        #region PropsAndFields
        protected bool lockSelection = false;

        private SimpleSelectionLocker mySelectionTarget
        {
            get
            {
                if (_mySelectionTarget == null)
                    _mySelectionTarget = (SimpleSelectionLocker)target;

                return _mySelectionTarget;
            }
        }
        private SimpleSelectionLocker _mySelectionTarget = null;

        protected GameObject mySelection
        {
            get
            {
                return mySelectionTarget != null ? mySelectionTarget.mySelection : null;
            }
        }
        #endregion

        #region UnityMessages
        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += this.DrawMySceneGUI;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.DrawMySceneGUI;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Label($"Locking Selection To GameObject: {lockSelection}");
            //SimpleRagdollAdder myRagdollAdder = (SimpleRagdollAdder)target;
            if (GUILayout.Button("Toggle Lock Selection To Object"))
            {
                SimpleSelectionLocker _mySelectionLocker = (SimpleSelectionLocker)target;
                if (_mySelectionLocker.mySelection == null)
                {
                    Debug.Log("Please Add Selection Object");
                    return;
                }
                else
                {
                    ToggleLockSelection();
                }
            }
        }
        #endregion

        #region Handlers
        void DrawMySceneGUI(SceneView sceneView)
        {
            if (lockSelection)
            {
                if (mySelection == null)
                {
                    Debug.Log("Cannot Lock Scene To None");
                    ToggleLockSelection();
                    return;
                }
                else if (Selection.activeGameObject != mySelection)
                {
                    Debug.Log("You clicked on " + Selection.activeGameObject);
                    Selection.activeGameObject = mySelection;
                }
            }
        }
        #endregion

        #region OldCode
        //bool running = false, runningOld = false;
        //GameObject mySelection;

        //void OnSceneGUI(/*SceneView sceneView*/)
        //{
        //    if (running)
        //    {
        //        if (Selection.activeGameObject != mySelection)
        //        {
        //            Debug.Log("You clicked on " + Selection.activeGameObject);
        //            Selection.activeGameObject = mySelection;
        //        }
        //    }
        //}

        //void OnGUI()
        //{
        //    //Debug.Log("OnGUI");
        //    //if (Selection.transforms.Length == 1)
        //    //{
        //    //    running = GUI.Toggle(new Rect(3, 3, 150, 20), running, "Lock " + Selection.activeGameObject.name);
        //    //    if (runningOld != running)
        //    //    {
        //    //        runningOld = running;
        //    //        mySelection = Selection.activeGameObject;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    GUI.Label(new Rect(3, 3, 200, 20), "Select a single GameObject");
        //    //}
        //}

        //void OnFocus()
        //{
        //    Debug.Log("OnFocus");
        //    // Remove delegate listener if it has previously
        //    // been assigned.
        //    SceneView.onSceneGUIDelegate -= this.DrawMySceneGUI;
        //    // Add (or re-add) the delegate.
        //    //SceneView.onSceneGUIDelegate += this.DrawMySceneGUI;
        //}
        #endregion

        private void ToggleLockSelection()
        {
            lockSelection = !lockSelection;
        }
    }
}
