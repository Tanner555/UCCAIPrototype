using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace BaseFramework
{
    /// <summary>
    /// Simplified The AddRagdoll Method Inside TPC CharacterBuilder
    /// To Be Useful For Any Model With an Animator
    /// </summary>
    [CustomEditor(typeof(SimpleRagdollAdder))]
    public class SimpleRagdollAdderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SimpleRagdollAdder myRagdollAdder = (SimpleRagdollAdder)target;
            if(GUILayout.Button("Add Ragdoll To Character"))
            {
                if (myRagdollAdder.myCharacter == null)
                {
                    Debug.Log("Please add character");
                    return;
                }
                else if (myRagdollAdder.myCharacter.GetComponent<Animator>() == null)
                {
                    Debug.Log("Please add animator");
                    return;
                }
                else
                {
                    Debug.Log("Building Ragdoll...");
                    BuildRagdoll();
                }
            }
        }

        private void BuildRagdoll()
        {
            SimpleRagdollAdder myRagdollAdder = (SimpleRagdollAdder)target;
            var myAnimator = myRagdollAdder.myCharacter.GetComponent<Animator>();

            var myRagdollBuilderT = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
            var windows = Resources.FindObjectsOfTypeAll(myRagdollBuilderT);
            // Open the Ragdoll Builder if it isn't already opened.
            if (windows == null || windows.Length == 0)
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
                windows = Resources.FindObjectsOfTypeAll(myRagdollBuilderT);
            }

            if (windows != null && windows.Length > 0)
            {
                var myRagdollWindow = windows[0] as ScriptableWizard;
                SetValueByReflection(myRagdollWindow, "pelvis", myAnimator.GetBoneTransform(HumanBodyBones.Hips));
                SetValueByReflection(myRagdollWindow, "leftHips", myAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
                SetValueByReflection(myRagdollWindow, "leftKnee", myAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
                SetValueByReflection(myRagdollWindow, "leftFoot", myAnimator.GetBoneTransform(HumanBodyBones.LeftFoot));
                SetValueByReflection(myRagdollWindow, "rightHips", myAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
                SetValueByReflection(myRagdollWindow, "rightKnee", myAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
                SetValueByReflection(myRagdollWindow, "rightFoot", myAnimator.GetBoneTransform(HumanBodyBones.RightFoot));
                SetValueByReflection(myRagdollWindow, "leftArm", myAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
                SetValueByReflection(myRagdollWindow, "leftElbow", myAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
                SetValueByReflection(myRagdollWindow, "rightArm", myAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm));
                SetValueByReflection(myRagdollWindow, "rightElbow", myAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm));
                SetValueByReflection(myRagdollWindow, "middleSpine", myAnimator.GetBoneTransform(HumanBodyBones.Spine));
                SetValueByReflection(myRagdollWindow, "head", myAnimator.GetBoneTransform(HumanBodyBones.Head));

                var method = myRagdollWindow.GetType().GetMethod("CheckConsistency", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    myRagdollWindow.errorString = (string)method.Invoke(myRagdollWindow, null);
                    myRagdollWindow.isValid = string.IsNullOrEmpty(myRagdollWindow.errorString);
                }
            }
        }

        private void SetValueByReflection(ScriptableWizard obj, string name, object value)
        {
            if (value == null)
            {
                return;
            }

            var field = obj.GetType().GetField(name);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}