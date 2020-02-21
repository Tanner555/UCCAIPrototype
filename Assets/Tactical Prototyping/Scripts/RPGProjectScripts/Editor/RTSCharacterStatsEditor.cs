using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace RPGPrototype
{
    public class RTSCharacterStatsEditor : EditorWindow
    {
        [MenuItem("RPGPrototype/CharacterStatsEditor")]
        public static void ShowCharacterStatsEditor()
        {
            RTSCharacterStatsEditor _window = GetWindow<RTSCharacterStatsEditor>();
            _window.minSize = new Vector2(450, 450);
            _window.titleContent = new GUIContent("CharacterStatsEditor");
        }

        #region UnityMessages
        private void OnEnable()
        {
            var _root = this.rootVisualElement;
            // Create the hierarchy from XML and apply styles from USS.
            var _uxml = Resources.Load("RTSInspector/CharacterStatsEditor") as VisualTreeAsset;
            //var _uss = Resources.Load("RTSStyles/CharacterStatsEditorStyles") as StyleSheet;
            _uxml.CloneTree(_root);
            //_root.styleSheets.Add(_uss);


        }
        #endregion


        #region Commented Code
        void ReferenceMethod_DontUse()
        {
            var _root = this.rootVisualElement;

            var _uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("RTSInspector/CharacterStatsEditor.uxml");
            var _uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("RTSStyles/CharacterStatsEditor.uss");
            //Get References To Elements of Interest
            var _display = _root.Q<Label>("display");
            var _slider = _root.Q<SliderInt>();

            //Register For Events
            _slider.RegisterCallback<ChangeEvent<int>>(_evt =>
            {
                _display.text = _evt.newValue.ToString();
                var _step = _root.resolvedStyle.width / 100;
                var _halfWidth = _display.resolvedStyle.width / 2;
                _display.style.left = ((_evt.newValue - 1) * _step) - _halfWidth;
            });
        }
        #endregion
    }
}