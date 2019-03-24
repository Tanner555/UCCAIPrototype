using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSPrototype
{
    public class TestLuaScripting : MonoBehaviour
    {
        #region Fields-Props
        public Button RunButton;
        public Button CancelButton;
        public InputField CodeInputField;

        bool bAllCompsAreValid =>
            RunButton != null && CancelButton != null && CodeInputField != null;
        #endregion

        #region UnityMessages
        // Start is called before the first frame update
        void Start()
        {
            if (bAllCompsAreValid)
            {
                RunButton.onClick.AddListener(OnRunButton);
                CancelButton.onClick.AddListener(OnCancelButton);
            }
            else
            {
                Debug.Log("TestLua: Not All Comps Are Set.");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region Handlers
        void OnRunButton()
        {
            Debug.Log("Pressed Run Button");
        }

        void OnCancelButton()
        {
            Debug.Log("Pressed Cancel Button");
        }
        #endregion
    }
}